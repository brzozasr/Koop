using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Koop.Extensions;
using Koop.Models;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private IMapper _mapper;

        public ReportController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,Koty,Paczkers")]
        [HttpGet("Packers/{daysBack:int}")]
        public async Task<IActionResult> ReportPackers([FromRoute] int daysBack)
        {
            try
            {
                var result = await _uow.Repository<FnListForPacker>()
                    .ExecuteSql("SELECT * FROM list_for_packer({0});", daysBack);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize(Roles = "Admin,Koty,Paczkers")]
        [HttpGet("Packers/Last/Grande")]
        public async Task<IActionResult> ReportPackersLastGrande()
        {
            try
            {
                var lastOrderGrandeId = _uow.Repository<Order>().GetAll()
                    .OrderByDescending(x => x.OrderStopDate)
                    .FirstOrDefault()?.OrderId;

                if (lastOrderGrandeId == Guid.Empty || lastOrderGrandeId == null)
                {
                    return Ok(new {info = "There are no grande orders."});
                }

                var allOrders = await _uow.Repository<OrderView>().GetAll()
                    .Where(x => x.OrderId == lastOrderGrandeId).ToListAsync();

                var lastOrderGrande = allOrders
                    .GroupBy(x => x.ProductId).AsQueryable();

                if (!lastOrderGrande.Any())
                {
                    return Ok(new {info = "There are no orders in the grande order."});
                }

                var tmpDict = new Dictionary<Guid, string>();

                foreach (var group in lastOrderGrande)
                {
                    var groupKey = group.Key;
                    foreach (var groupItem in group)
                    {
                        if (groupKey.HasValue)
                        {
                            if (!tmpDict.ContainsKey((Guid) groupKey))
                            {
                                tmpDict[(Guid) groupKey] = $"{groupItem.BasketName}: {groupItem.Quantity}";
                            }
                            else
                            {
                                tmpDict[(Guid) groupKey] =
                                    $"{tmpDict[(Guid) groupKey]}, {groupItem.BasketName}: {groupItem.Quantity}";
                            }
                        }
                    }
                }

                if (!tmpDict.Any())
                {
                    return Ok(new {info = "There are no orders in the grande order (Empty Dictionary)."});
                }

                var listForPackers = new List<FnListForPacker>();

                foreach (var (productId, baskets) in tmpDict)
                {
                    var productName = allOrders.FirstOrDefault(x => x.ProductId == productId)?.ProductName;
                    listForPackers.Add(new FnListForPacker
                    {
                        ProductName = productName,
                        ProductsInBaskets = baskets
                    });
                }

                return Ok(listForPackers);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize(Roles = "Admin,Koty,Skarbnik")]
        [HttpGet("Orders/Grande/By/Supplier/{supplierId:guid}", Name = "OrdersGrande")]
        [HttpGet("Last/Order/Grande/By/Supplier/{supplierId:guid}", Name = "LastOrderGrande")]
        public async Task<IActionResult> ReportOrdersFromSupplier([FromRoute] Guid supplierId)
        {
            try
            {
                var supplier = await _uow.Repository<Supplier>().GetAll()
                    .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

                if (supplier == null)
                {
                    return Ok(new {info = "There is no such supplier."});
                }

                var productsSupplier = _uow.Repository<Order>().GetAll()
                    .Join(_uow.Repository<OrderedItem>().GetAll(), order => order.OrderId,
                        item => item.OrderId,
                        (order, item) => new
                        {
                            OrderId = order.OrderId,
                            OrderStartDate = order.OrderStartDate,
                            OrderStopDate = order.OrderStopDate,
                            OrderedItemId = item.OrderedItemId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        })
                    .Join(_uow.Repository<Product>().GetAll(),
                        orderItem => orderItem.ProductId,
                        product => product.ProductId,
                        (orderItem, product) => new
                        {
                            OrderId = orderItem.OrderId,
                            OrderStartDate = orderItem.OrderStartDate,
                            OrderStopDate = orderItem.OrderStopDate,
                            OrderedItemId = orderItem.OrderedItemId,
                            ProductId = orderItem.ProductId,
                            Quantity = orderItem.Quantity,
                            ProductName = product.ProductName,
                            Price = product.Price,
                            UnitName = product.Unit.UnitName,
                            SupplierId = product.SupplierId
                        })
                    .Where(x => x.SupplierId == supplierId);

                if (ControllerContext.ActionDescriptor.AttributeRouteInfo?.Name == "LastOrderGrande")
                {
                    var lastOrderGrandeId = _uow.Repository<Order>().GetAll()
                        .OrderByDescending(x => x.OrderStopDate)
                        .FirstOrDefault()?.OrderId;
                    productsSupplier = productsSupplier.Where(x => x.OrderId == lastOrderGrandeId);
                }

                if (!productsSupplier.Any())
                {
                    return Ok(new {info = "The supplier did not sell any products."});
                }

                var groupOrder = productsSupplier
                    .AsEnumerable()
                    .GroupBy(x => x.OrderId);

                var supplierReport = new SupplierReport
                {
                    SupplierId = supplier.SupplierId,
                    SupplierName = supplier.SupplierName,
                    SupplierAbbr = supplier.SupplierAbbr,
                    Email = supplier.Email
                };

                foreach (var group in groupOrder)
                {
                    var groupKey = group.Key;
                    var supplierOrder = new SupplierReportOrder
                    {
                        OrderId = groupKey,
                    };

                    foreach (var groupItem in group)
                    {
                        if (supplierOrder.OrderId == groupItem.OrderId)
                        {
                            supplierOrder.OrderStartDate = groupItem.OrderStartDate;
                            supplierOrder.OrderStopDate = groupItem.OrderStopDate;
                            supplierOrder.SupplierReport = supplierReport;
                        }

                        var supplierItem = new SupplierReportItem
                        {
                            ProductId = groupItem.ProductId,
                            ProductName = groupItem.ProductName,
                            Price = groupItem.Price,
                            Quantity = groupItem.Quantity,
                            UnitName = groupItem.UnitName,
                            SupplierReportOrder = supplierOrder,
                            TotalPrice = Math.Round((decimal) groupItem.Price * groupItem.Quantity, 2,
                                MidpointRounding.AwayFromZero)
                        };

                        supplierOrder.OrderTotalPrice += supplierItem.TotalPrice;

                        if (supplierOrder.SupplierReportItems.AreEquals(supplierItem) is var index && index > -1)
                        {
                            supplierOrder.SupplierReportItems[index].Quantity += groupItem.Quantity;
                            supplierOrder.SupplierReportItems[index].TotalPrice += supplierItem.TotalPrice;
                        }
                        else
                        {
                            supplierOrder.SupplierReportItems.Add(supplierItem);
                        }
                    }

                    supplierReport.SupplierReportOrder.Add(supplierOrder);

                    supplierReport.TotalProfit += supplierOrder.OrderTotalPrice;
                }

                return Ok(supplierReport);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize(Roles = "Admin,Koty,Skarbnik")]
        [HttpGet("Cooperators/Debt")]
        public async Task<IActionResult> ReportCooperatorsDebt()
        {
            try
            {
                var coopDept = await _uow.Repository<User>().GetAll()
                    .Where(x => x.Debt < 0)
                    .OrderBy(x => x.Debt)
                    .ToListAsync();

                if (!coopDept.Any())
                {
                    return Ok(new {info = "There are no indebted cooperators."});
                }

                var coopDebtMap = _mapper.Map<List<CoopDeptReport>>(coopDept);

                return Ok(coopDebtMap);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize(Roles = "Admin,Koty,Skarbnik")]
        [HttpGet("Debts/To/Suppliers")]
        public async Task<IActionResult> DebtsToSuppliers()
        {
            try
            {
                var suppliers = await _uow.Repository<Supplier>().GetAll()
                    .Where(x => x.Receivables > 0)
                    .OrderByDescending(x => x.Receivables)
                    .ToListAsync();

                if (!suppliers.Any())
                {
                    return Ok(new {info = "There are no debts to suppliers."});
                }

                var suppliersMap = _mapper.Map<List<DebtsToSuppliersReport>>(suppliers);

                return Ok(suppliersMap);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize(Roles = "Admin,Koty,Skarbnik")]
        [HttpGet("Order/Grande/{dateStart:datetime?}")]
        public IActionResult OrderGrandeReport([FromRoute] DateTime? dateStart = null)
        {
            try
            {
                DateTime? lastGrandeDate;

                if (dateStart is null)
                {
                    lastGrandeDate = _uow.Repository<Order>().GetAll()
                        .OrderByDescending(x => x.OrderStartDate).FirstOrDefault()?
                        .OrderStartDate;
                }
                else
                {
                    lastGrandeDate = dateStart;
                }


                if (lastGrandeDate is null)
                {
                    return Ok(new {info = "There are no grande orders."});
                }

                var orderGrande = _uow.Repository<Order>().GetAll()
                    .Join(_uow.Repository<OrderedItem>().GetAll(), order => order.OrderId,
                        item => item.OrderId,
                        (order, item) => new
                        {
                            OrderId = order.OrderId,
                            OrderStartDate = order.OrderStartDate,
                            OrderStopDate = order.OrderStopDate,
                            OrderStatus = order.OrderStatus.OrderStatusName,
                            OrderedItemId = item.OrderedItemId,
                            ProductId = item.ProductId,
                            ProductName = item.Product.ProductName,
                            Price = item.Product.Price,
                            Quantity = item.Quantity,
                            UnitName = item.Product.Unit.UnitName,
                            CoopId = item.CoopId,
                            CoopFirstName = item.Coop.FirstName,
                            CoopLastName = item.Coop.LastName,
                            CoopFundValue = item.Coop.Fund.Value,
                            SupplierId = item.Product.Supplier.SupplierId,
                            SupplierName = item.Product.Supplier.SupplierName,
                            SupplierAbbr = item.Product.Supplier.SupplierAbbr
                        })
                    .Where(x => x.OrderStartDate == lastGrandeDate)
                    .AsEnumerable()
                    .GroupBy(x => new
                    {
                        x.OrderId,
                        x.OrderStartDate,
                        x.OrderStopDate,
                        x.OrderStatus
                    }).ToList();

                if (!orderGrande.Any())
                {
                    return Ok(new {info = "There are no orders in the grande order."});
                }

                var grandeOrderReport = new GrandeOrderReport();

                foreach (var group in orderGrande)
                {
                    var groupKey = group.Key;

                    grandeOrderReport.OrderId = groupKey.OrderId;
                    grandeOrderReport.OrderStartDate = groupKey.OrderStartDate;
                    grandeOrderReport.OrderStopDate = groupKey.OrderStopDate;
                    grandeOrderReport.OrderStatus = groupKey.OrderStatus;

                    foreach (var groupItem in group)
                    {
                        var grandeOrderItems = new GrandeOrderItemReport
                        {
                            OrderedItemId = groupItem.OrderedItemId,
                            ProductId = groupItem.ProductId,
                            ProductName = groupItem.ProductName,
                            Price = groupItem.Price,
                            FundPrice = Math.Round(
                                (decimal) groupItem.Price +
                                (decimal) groupItem.Price * ((decimal) groupItem.CoopFundValue / 100), 2,
                                MidpointRounding.AwayFromZero),
                            TotalPrice = Math.Round((decimal) groupItem.Price * groupItem.Quantity, 2,
                                MidpointRounding.AwayFromZero),
                            TotalFundPrice =
                                Math.Round(
                                    (decimal) groupItem.Price + (decimal) groupItem.Price *
                                    ((decimal) groupItem.CoopFundValue / 100), 2, MidpointRounding.AwayFromZero) *
                                groupItem.Quantity,
                            Quantity = groupItem.Quantity,
                            UnitName = groupItem.UnitName,
                            CoopId = groupItem.CoopId,
                            CoopFirstName = groupItem.CoopFirstName,
                            CoopLastName = groupItem.CoopLastName,
                            CoopFundValue = groupItem.CoopFundValue,
                            SupplierId = groupItem.SupplierId,
                            SupplierName = groupItem.SupplierName,
                            SupplierAbbr = groupItem.SupplierAbbr,
                            GrandeOrderReport = grandeOrderReport
                        };

                        grandeOrderReport.TotalGrandePrice += grandeOrderItems.TotalPrice;
                        grandeOrderReport.TotalGrandeFundPrice += grandeOrderItems.TotalFundPrice;

                        grandeOrderReport.GrandeOrderItem.Add(grandeOrderItems);
                    }
                }

                return Ok(grandeOrderReport);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
        
        [Authorize]
        [HttpGet("Order/Grande/StartDates")]
        public IActionResult OrderGrandeDates()
        {
            try
            {
                var lastGrandeDates = _uow.Repository<Order>().GetAll()
                    .OrderByDescending(x => x.OrderStartDate)
                    .Where(x => x.OrderStartDate != null)
                    .Select(x => new {x.OrderStartDate});

                if (lastGrandeDates.Any())
                {
                    return Ok(lastGrandeDates);
                }

                return Ok(new {info = "There are no orders in the grande order."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
        
        [Authorize]
        [HttpGet("Get/Suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            try
            {
                var suppliers = await _uow.Repository<Supplier>().GetAll()
                    .OrderBy(sup => sup.SupplierName)
                    .Where(sn => sn.SupplierName != null)
                    .Select(x => new
                    {
                        x.SupplierId,
                        x.SupplierName,
                        x.SupplierAbbr
                    }).ToListAsync();

                if (suppliers.Any())
                {
                    return Ok(suppliers);
                }

                return Ok(new {info = "There are no suppliers."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
    }
}