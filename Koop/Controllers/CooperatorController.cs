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
            try
            {
                var orders = await _uow.Repository<OrderView>().GetAll()
                    .Where(coop => coop.Id == coopId)
                    .ToListAsync();

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
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
    }
}