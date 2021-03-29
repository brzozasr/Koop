using System;
using System.Threading.Tasks;
using AutoMapper;
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
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
    }
}