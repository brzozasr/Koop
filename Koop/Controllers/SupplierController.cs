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
    public class SupplierController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private readonly IMapper _mapper;

        public SupplierController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        
        [AllowAnonymous]
        [HttpGet("allsuppliers")]
        public IActionResult AllSuppliers()
        {
            try
            {
                return Ok(_uow.Repository<SupplierView>().GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [AllowAnonymous]
        [HttpGet("supplier/{supplierId}")]
        public IActionResult Supplier(Guid supplierId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetSupplier(supplierId));
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpGet("supplier/{supplierId}/edit")]
        public IActionResult EditSupplier(Guid supplierId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetSupplier(supplierId));
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpPost("supplier/update")]
        public async Task<IActionResult> UpdateSupplier() //SupplierView sup
        {
            //TEST
            SupplierView sup = new SupplierView()
            {
                SupplierId = Guid.Parse("32594638-c2f2-413b-aedc-6041f9467b20"),
                SupplierAbbr = "TEST",
                SupplierName = "Testowy Supp",
                Description = "Teścik smaczny",
                Email = "abc@abcd.pl",
                Phone = "123234",
                OrderClosingDate = DateTime.Parse("2021-03-24 00:00:00"),
                OproFirstName = "Tadeusz",
                OproLastName = "Batko"
            };
            //test end
            
            try
            {
                User opro = _uow.Repository<User>()
                    .GetDetail(u => u.FirstName == sup.OproFirstName && u.LastName == sup.OproLastName);
        
                if (opro == null)
                {
                    return BadRequest(new {error = "OpRo name is invalid."});
                }

                Supplier supplier = new Supplier()
                {
                    SupplierAbbr = sup.SupplierAbbr,
                    SupplierId = sup.SupplierId,
                    SupplierName = sup.SupplierName,
                    Description = sup.Description,
                    Email = sup.Email,
                    Phone = sup.Phone,
                    Picture = sup.Picture,
                    OrderClosingDate = sup.OrderClosingDate,
                    OproId = opro.Id
                };
                _uow.ShopRepository().UpdateSupplier(supplier);
                
                return Ok(new {info = $"The supplier has been updated (supplier ABBR: {sup.SupplierAbbr})."});

            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        
        [Authorize(Roles = "Admin,Koty,Opro")]
        [HttpPost("supplier/add")]
        public async Task<IActionResult> AddSupplier() //SupplierView sup
        {
            //TEST
            SupplierView sup = new SupplierView()
            {
                SupplierAbbr = "TEST",
                SupplierName = "Testowy Dostawca",
                Description = "Teścik smaczny",
                Email = "abc@abc.pl",
                Phone = "123234",
                OrderClosingDate = DateTime.Parse("2021-03-24 00:00:00"),
                OproFirstName = "Henryk",
                OproLastName = "Sienkiewicz"
            };
            // test end

            try
            {
                 User opro = _uow.Repository<User>()
                    .GetDetail(u => u.FirstName == sup.OproFirstName && u.LastName == sup.OproLastName);
        
                 if (opro == null)
                 {
                    return BadRequest(new {error = "OpRo name is invalid."});
                 }
        
                 Supplier supplier = new Supplier()
                 {
                     SupplierAbbr = sup.SupplierAbbr,
                     // SupplierId = sup.SupplierId,
                     SupplierName = sup.SupplierName,
                     Description = sup.Description,
                     Email = sup.Email,
                     Phone = sup.Phone,
                     Picture = sup.Picture,
                     // OrderClosingDate = sup.OrderClosingDate,
                     OproId = opro.Id
                 };
                 
                 await _uow.Repository<Supplier>().AddAsync(supplier);
        
                 await _uow.SaveChangesAsync();
                 return Ok(new {info = $"The supplier has been added (supplier ABBR: {sup.SupplierAbbr})."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        // [Authorize(Roles = "Admin,Koty,OpRo")]
        // [HttpGet("supplier/{supplierId}/toggleAvail/")]
        // public IActionResult ToggleSupplierAvailability(Guid supplierId)
        // {
        //     try
        //     {
        //         Supplier supplier= _uow.Repository<Supplier>().GetDetail(s => s.SupplierId == supplierId);
        //         _uow.ShopRepository().ToggleSupplierAvailability(supplier);
        //         return Ok(new {info = "The supplier availability has been changed."});
        //
        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(new {error = e.Message, source = e.Source});
        //     }
        // }
        //
        // [Authorize(Roles = "Admin,Koty,OpRo")]
        // [HttpGet("supplier/{supplierId}/toggleBlocked/")]
        // public IActionResult ToggleSupplierBlocked(Guid supplierId)
        // {
        //     try
        //     {
        //         Supplier supplier= _uow.Repository<Supplier>().GetDetail(s => s.SupplierId == supplierId);
        //         _uow.ShopRepository().ToggleSupplierBlocked(supplier);
        //         return Ok(new {info = "The supplier blocked status has been changed."});
        //
        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(new {error = e.Message, source = e.Source});
        //     }
        // }

    }
}