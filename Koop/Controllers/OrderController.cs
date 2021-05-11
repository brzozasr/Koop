using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Koop.models;
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
    public class OrderController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private readonly IMapper _mapper;

        public OrderController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        
        [Authorize(Roles = "Admin,Koty,Paczkers")]
        [HttpGet("order/baskets")]
        public IActionResult BasketName()
        {
            try
            {
                var baskets = _uow.ShopRepository().GetBaskets();
                if (baskets != null && baskets.Any())
                {
                    return Ok(_uow.ShopRepository().GetBaskets());
                }
                return BadRequest(new {error = "Baskets are empty. No order is ready."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty")]
        [HttpPost("order/add")]
        public IActionResult AddOrder([FromBody] OrderGrandeHistoryView order)
        {
            try
            {
                order.OrderStatusId = _uow.Repository<OrderStatus>()
                    .GetDetail(os => os.OrderStatusName == order.OrderStatusName)?.OrderStatusId;
                
                var orderMap = _mapper.Map<Order>(order);

                _uow.Repository<Order>().Add(orderMap);
        
                _uow.SaveChanges();
                return Ok(new {info = $"The order has been added (order Id: {orderMap.OrderId})."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }

        [Authorize(Roles = "Admin,Koty")]
        [HttpGet("bigorders")]
        public IActionResult BigOrders()
        {
            try
            {
                return Ok(_uow.Repository<OrderGrandeHistoryView>().GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [HttpGet("statuses")]
        public IActionResult Statuses()
        {
            try
            {
                List<string> listOfStatuses = OrderStatuses.GetNames(typeof(OrderStatuses)).ToList();
                
                return Ok(listOfStatuses);
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpGet("order/{orderId}/{status}")]
        public IActionResult ChangeOrderStatus(Guid orderId, OrderStatuses status)
        {
            try
            {
                // only one order can be open
                if (status == OrderStatuses.Otwarte)
                {
                    var orderOpen = _uow.Repository<Order>().GetAll().Where(o => o.OrderStatus.OrderStatusName == OrderStatuses.Otwarte.ToString()).ToList();

                    if (orderOpen.Count > 0 )
                    {
                        return BadRequest(new {error = "There is already an open order!"});
                    }
                    
                    _uow.ShopRepository().ClearBaskets();
                }
                
                
                Order order = _uow.Repository<Order>().GetDetail(o => o.OrderId == orderId);
                
                // you can close or cancel only an open order
                if (OrderStatuses.Zamknięte == status || OrderStatuses.Anulowane == status)
                {
                    string currentStatusName = _uow.Repository<OrderStatus>()
                        .GetDetail(s => s.OrderStatusId == order.OrderStatusId).OrderStatusName;
                    bool currentStatusOpen = (OrderStatuses.Otwarte.ToString() == currentStatusName);

                    if (!currentStatusOpen)
                    {
                        return BadRequest(new {error = "This order must be open first."});
                    }

                    if (OrderStatuses.Zamknięte == status)
                    {
                        _uow.ShopRepository().AssignBaskets(orderId);
                    }
                }

                _uow.ShopRepository().ChangeOrderStatus(order, status);
                return Ok(new {info = "The order status has been changed."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }    
    }
}