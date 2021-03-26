using System;
using Koop.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Koop.Controllers
{
    public class TestController : Controller
    {
        private IGenericUnitOfWork _uow;

        public TestController(IGenericUnitOfWork genericUnitOfWork)
        {
            _uow = genericUnitOfWork;
        }
        
        public IActionResult Index()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        public IActionResult NoAuth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize]
        public IActionResult Auth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize(Policy = "Szymek")]
        public IActionResult AuthUserName()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize(Policy = "StandardUser")]
        public IActionResult AuthRole()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
    }
}