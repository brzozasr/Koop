using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Koop.Models;
using Koop.Models.ModelView;
using Koop.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private KoopDbContext _koopDbContext;

        public ReportController(IGenericUnitOfWork uow, KoopDbContext koopDbContext)
        {
            _uow = uow;
            _koopDbContext = koopDbContext;
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