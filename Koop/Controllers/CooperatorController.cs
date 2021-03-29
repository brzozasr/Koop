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
    public class CooperatorController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CooperatorController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [AllowAnonymous]
        // [Authorize(Roles = "Admin,Koty")]
        [HttpPost("{coopId}/Orders")]
        public async Task<IActionResult> OrdersCoopView(Guid coopId)
        {
            var orders = await _uow.Repository<OrderView>().GetAll()
                .Where(coop => coop.Id == coopId)
                .ToListAsync();

            var mapOrders = _mapper.Map<List<CoopOrder>>(orders);
            
            var result = mapOrders.GroupBy(coop =>
                coop.OrderId);
            
            return Ok(result);
        }
    }
}