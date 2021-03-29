using System;
using System.Data;
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

        [Authorize(Roles = "Admin,Koty")]
        [HttpGet("Packers/{daysBack}")]
        public IActionResult ReportPackers(int daysBack)
        {
           // var result = _koopDbContext.Set<FnListForPacker>().FromSqlRaw("SELECT * FROM list_for_packer({0});", 50).ToList();
           
            var result = _koopDbContext.FnListForPackers.FromSqlRaw("SELECT * FROM list_for_packer({0});", daysBack);
            return Ok(result);
        }
    }
}