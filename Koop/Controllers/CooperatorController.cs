using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Koop.Models;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Koop.Models.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CooperatorController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CooperatorController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        
        [Authorize(Roles = "Admin,Koty,Opro")]
        [HttpGet("allNames")]
        public IActionResult AllCooperatorsNames()
        {
            try
            {
                List<CooperantName> namesMap = new List<CooperantName>();
                var users = _uow.Repository<User>().GetAll();
                foreach (var user in users)
                {
                    namesMap.Add(_mapper.Map<CooperantName>(user));
                }
                
                return Ok(namesMap);
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }

        [Authorize(Roles = "Admin,Koty")]
        [HttpPost("Update/OrderItem/Quantity")]
        public async Task<IActionResult> UpdateOrderItem([FromBody] OrderedItemQuantityUpdate orderItem)
        {
            try
            {
                if (orderItem.Quantity > 0)
                {
                    var order = _uow.Repository<OrderedItem>()
                        .GetDetail(rec => rec.OrderedItemId == orderItem.OrderedItemId);

                    if (order == null)
                    {
                        return Ok(
                            new
                            {
                                info = $"There is no product ordered with the given ID: {orderItem.OrderedItemId}."
                            });
                    }

                    order.Quantity = orderItem.Quantity;

                    await _uow.SaveChangesAsync();
                    return Ok(
                        new
                        {
                            info =
                                $"The quantity of the ordered product has been changed to {orderItem.Quantity} (order ID: {orderItem.OrderedItemId})."
                        });
                }

                return BadRequest(new {error = "The entered quantity must be greater than 0."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize(Roles = "Admin,Koty")]
        [HttpDelete("Delete/OrderItem/{orderItemId:guid}")]
        public async Task<IActionResult> DeleteOrderItem([FromRoute] Guid orderItemId)
        {
            try
            {
                var order = _uow.Repository<OrderedItem>()
                    .GetDetail(rec => rec.OrderedItemId == orderItemId);

                if (order == null)
                {
                    return Ok(
                        new
                        {
                            info = $"There is no product ordered with the given ID: {orderItemId}."
                        });
                }

                _uow.Repository<OrderedItem>().Delete(order);

                await _uow.SaveChangesAsync();
                return Ok(new {info = $"The ordered item has been deleted (order ID: {orderItemId})."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize(Roles = "Admin,Koty")]
        [HttpGet("{coopId:guid}/Last/Order/Grande")]
        public async Task<IActionResult> LastOrderGrandeCoop([FromRoute] Guid coopId)
        {
            try
            {
                var lastOrderGrandeId = _uow.Repository<Order>().GetAll()
                    .OrderByDescending(x => x.OrderStartDate)
                    .FirstOrDefault()?.OrderId;
                
                if (lastOrderGrandeId == null)
                {
                    return Ok(
                        new
                        {
                            info = "There are not the orders grande available."
                        });
                }

                var orders = await _uow.Repository<OrderView>().GetAll()
                    .Where(coop => coop.Id == coopId && coop.OrderId == lastOrderGrandeId 
                    && coop.OrderStatusName == OrderStatuses.Zamknięte.ToString())
                    .ToListAsync();


                if (orders == null || orders.Count == 0)
                {
                    return Ok(
                        new
                        {
                            info = $"The cooperator with ID {coopId} does not have any orders."
                        });
                }

                var coopOrders = _mapper.Map<List<CoopOrder>>(orders);
                var coopOrderNodes = _mapper.Map<List<CoopOrderNode>>(orders);

                // Remove duplicates
                var selectedOrders = coopOrders.GroupBy(x => x.OrderId).Select(y => y.First());

                HashSet<Guid> tmpList = new HashSet<Guid>();
                decimal orderTotalPrice = 0;
                decimal orderTotalFundPrice = 0;

                foreach (var coopOrder in selectedOrders)
                {
                    foreach (var coopOrderNode in coopOrderNodes)
                    {
                        if (coopOrder.OrderId.HasValue && !tmpList.Contains(coopOrder.OrderId.Value))
                        {
                            if (coopOrder.OrderId == coopOrderNode.OrderId)
                            {
                                if (coopOrderNode.TotalPrice.HasValue)
                                {
                                    orderTotalPrice += (decimal) coopOrderNode.TotalPrice.Value;
                                }

                                if (coopOrderNode.TotalFundPrice.HasValue)
                                {
                                    orderTotalFundPrice += (decimal) coopOrderNode.TotalFundPrice.Value;
                                }

                                coopOrder.CoopOrderNode.Add(coopOrderNode);
                            }
                        }
                    }

                    coopOrder.OrderTotalPrice = Math.Round(orderTotalPrice, 2, MidpointRounding.AwayFromZero);
                    coopOrder.OrderTotalFundPrice = Math.Round(orderTotalFundPrice, 2, MidpointRounding.AwayFromZero);
                    orderTotalPrice = 0;
                    orderTotalFundPrice = 0;

                    if (coopOrder.OrderId.HasValue)
                    {
                        tmpList.Add(coopOrder.OrderId.Value);
                    }
                }

                tmpList.Clear();

                return Ok(selectedOrders);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize]
        [HttpGet("{coopId:guid}/Orders/Grande")]
        public async Task<IActionResult> OrdersCoopHistory([FromRoute] Guid coopId)
        {
            try
            {
                var orders = await _uow.Repository<OrderView>().GetAll()
                    .Where(coop => coop.Id == coopId && 
                                   coop.OrderStatusName == OrderStatuses.Zamknięte.ToString())
                    .ToListAsync();

                if (orders == null || orders.Count == 0)
                {
                    return Ok(
                        new
                        {
                            info = $"The cooperator with ID {coopId} does not have any orders."
                        });
                }

                var coopOrders = _mapper.Map<List<CoopOrder>>(orders);
                var coopOrderNodes = _mapper.Map<List<CoopOrderNode>>(orders);

                // Remove duplicates
                var selectedOrders = coopOrders.GroupBy(x => x.OrderId).Select(y => y.First());

                HashSet<Guid> tmpList = new HashSet<Guid>();
                decimal orderTotalPrice = 0;
                decimal orderTotalFundPrice = 0;

                foreach (var coopOrder in selectedOrders)
                {
                    foreach (var coopOrderNode in coopOrderNodes)
                    {
                        if (coopOrder.OrderId.HasValue && !tmpList.Contains(coopOrder.OrderId.Value))
                        {
                            if (coopOrder.OrderId == coopOrderNode.OrderId)
                            {
                                if (coopOrderNode.TotalPrice.HasValue)
                                {
                                    orderTotalPrice += (decimal) coopOrderNode.TotalPrice.Value;
                                }

                                if (coopOrderNode.TotalFundPrice.HasValue)
                                {
                                    orderTotalFundPrice += (decimal) coopOrderNode.TotalFundPrice.Value;
                                }

                                coopOrder.CoopOrderNode.Add(coopOrderNode);
                            }
                        }
                    }

                    coopOrder.OrderTotalPrice = Math.Round(orderTotalPrice, 2, MidpointRounding.AwayFromZero);
                    coopOrder.OrderTotalFundPrice = Math.Round(orderTotalFundPrice, 2, MidpointRounding.AwayFromZero);
                    orderTotalPrice = 0;
                    orderTotalFundPrice = 0;

                    if (coopOrder.OrderId.HasValue)
                    {
                        tmpList.Add(coopOrder.OrderId.Value);
                    }
                }

                tmpList.Clear();

                return Ok(selectedOrders);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize]
        [HttpGet("Get/Cooperators")]
        public async Task<IActionResult> GetCooperators()
        {
            try
            {
                var cooperators = await _uow.Repository<User>().GetAll()
                    .OrderBy(u => u.LastName)
                    .Where(u => u.LastName != null)
                    .Select(x => new CoopNames
                    {
                        Id = x.Id,
                        LastName = x.LastName,
                        FirstName = x.FirstName
                    }).ToListAsync();

                if (cooperators.Any())
                {
                    return Ok(cooperators);
                }

                return Ok(new {info = "There are no cooperators."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
    }
}