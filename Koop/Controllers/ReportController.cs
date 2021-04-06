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
        public async Task<IActionResult> ReportPackers(int daysBack)
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

                var allOrders = await _uow.Repository<OrderView>().GetAllAsync();

                var lastOrderGrande = allOrders
                    .Where(x => x.OrderId == lastOrderGrandeId)
                    .GroupBy(x => x.ProductId).AsQueryable();

                if (!lastOrderGrande.Any())
                {
                    return Ok(new {info = "There are no orders in the grande order."});
                }

                var tmpDict = new Dictionary<string, string>();

                foreach (var group in lastOrderGrande)
                {
                    var groupKey = group.Key;
                    foreach (var groupItem in group)
                    {
                        if (groupKey.HasValue)
                        {
                            if (!tmpDict.ContainsKey(groupItem.ProductName))
                            {
                                tmpDict[groupItem.ProductName] = $"{groupItem.BasketName}: {groupItem.Quantity}";
                            }
                            else
                            {
                                tmpDict[groupItem.ProductName] = $"{tmpDict[groupItem.ProductName]}, {groupItem.BasketName}: {groupItem.Quantity}";
                            }
                        }
                    }
                }

                if (!tmpDict.Any())
                {
                    return Ok(new {info = "There are no orders in the grande order (Empty Dictionary)."});
                }

                var listForPackers = new List<FnListForPacker>();

                foreach (var (name, value) in tmpDict)
                {
                    listForPackers.Add(new FnListForPacker
                    {
                        ProductName = name,
                        ProductsInBaskets = value
                    });
                }

                return Ok(listForPackers);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
    }
}