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
        [HttpGet("supplier/{abbr}")]
        public IActionResult Supplier(string abbr)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetSupplier(abbr));
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,Opro")]
        [HttpGet("supplier/{abbr}/edit")]
        public IActionResult EditSupplier(string abbr)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetSupplier(abbr));
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        // TODO: POST EDIT
        
        
        // TODO: test POST ADD
        // [Authorize(Roles = "Admin,Koty,Opro")]
        // [HttpPost("supplier/add")]
        // public async Task<IActionResult> AddSupplier(SupplierView sup)
        // {
        //     try
        //     {
        //          User opro = _uow.Repository<User>()
        //             .GetDetail(u => u.FirstName == sup.OproFirstName && u.LastName == sup.OproLastName);
        //
        //          if (opro == null)
        //          {
        //             return BadRequest(new {error = "OpRo name is invalid."});
        //          }
        //
        //          Supplier supplier = new Supplier()
        //          {
        //              SupplierAbbr = sup.SupplierAbbr,
        //              // SupplierId = sup.SupplierId,
        //              SupplierName = sup.SupplierName,
        //              Description = sup.Description,
        //              Email = sup.Email,
        //              Phone = sup.Phone,
        //              Picture = sup.Picture,
        //              // OrderClosingDate = sup.OrderClosingDate,
        //              OproId = opro.Id
        //          };
        //          
        //          _uow.Repository<Supplier>().Add(supplier);
        //
        //         await _uow.SaveChangesAsync();
        //         return Ok(new {info = $"The supplier has been added (supplier ABBR: {sup.SupplierAbbr})."});
        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(new {error = e.Message, source = e.Source});
        //     }
        // }
        
        // TODO: test delete 
        [Authorize(Roles = "Admin,Koty")]
        [HttpPost("supplier/{abbr}/delete")]
        public async Task<IActionResult> DeleteSupplier(string abbr)
        {
            try
            {
                var supplier = _uow.Repository<Supplier>()
                    .GetDetail(rec => rec.SupplierAbbr.ToLower() == abbr);
                
                    //TODO : delete all products of supplier
                _uow.Repository<Supplier>().Delete(supplier);
        
                await _uow.SaveChangesAsync();
                return Ok(new {info = $"The supplier has been deleted (supplier ABBR: {supplier.SupplierAbbr})."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        //TODO: change status
    }
}