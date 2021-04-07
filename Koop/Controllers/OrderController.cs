using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Koop.models;
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
                return Ok(_uow.ShopRepository().GetBaskets());
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
                return Ok(_uow.Repository<Order>().GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        //TODO: change status
    }
}