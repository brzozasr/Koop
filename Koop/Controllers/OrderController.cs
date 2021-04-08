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
        
        // [Authorize(Roles = "Admin,Koty,Paczkers")]
        [HttpGet("order/baskets")]
        public IActionResult BasketName()
        {
            try
            {
                return Ok(_uow.ShopRepository().GetBaskets());
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }

        // [Authorize(Roles = "Admin,Koty")]
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
        
        // [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpGet("order/{orderId}/{status}/")]
        public IActionResult ChangeOrderStatus(Guid orderId, OrderStatuses status)
        {
            try
            {
                Order order = _uow.Repository<Order>().GetDetail(o => o.OrderId == orderId);
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