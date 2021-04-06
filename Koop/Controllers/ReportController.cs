using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Koop.Models;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IGenericUnitOfWork _uow;

        public ReportController(IGenericUnitOfWork uow)
        {
            _uow = uow;
        }
        
        [Authorize(Roles = "Admin,Koty,Paczkers")]
        [HttpGet("Packers/{daysBack}")]
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
                                tmpDict[(Guid) groupKey] = $"{tmpDict[(Guid) groupKey]}, {groupItem.BasketName}: {groupItem.Quantity}";
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

        [AllowAnonymous]
        // [Authorize(Roles = "Admin,Koty,Skarbnik")]
        [HttpGet("Orders/From/Supplier/{supplierId}")]
        public async Task<IActionResult> ReportOrdersFromSupplier([FromRoute] Guid supplierId)
        {
            var supplier = await _uow.Repository<Supplier>().GetAll()
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

            var productsSupplier = _uow.Repository<Order>().GetAll()
                .Join(_uow.Repository<OrderedItem>().GetAll(), order => order.OrderId,
                    item => item.OrderId,
                    (order, item) => new
                    {
                        OrderId = order.OrderId,
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
                        OrderedItemId = orderItem.OrderedItemId,
                        ProductId = orderItem.ProductId,
                        Quantity = orderItem.Quantity,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        UnitName = product.Unit.UnitName,
                        SupplierId = product.SupplierId
                    })
                .Where(x => x.SupplierId == supplierId);

            return Ok(productsSupplier);
        }
    }
}