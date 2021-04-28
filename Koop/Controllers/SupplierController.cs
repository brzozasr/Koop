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
using Microsoft.Extensions.Logging;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private readonly IMapper _mapper;
        // private ILogger _logger;

        public SupplierController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            // _logger = logger;
        }
        
        [AllowAnonymous]
        [HttpGet("allsuppliers")]
        public IActionResult AllSuppliers()
        {
            try
            {
                // _logger.LogInformation("Displayed all suppliers");

                // var suppliers = _uow.Repository<Supplier>().GetAll();
                //
                // List<SupplierViewMap> suppliersMap = new List<SupplierViewMap>();
                //
                // foreach (var sup in suppliers)
                // {
                //     var tempSup = _mapper.Map<SupplierViewMap>(sup);
                //     
                //     tempSup.OproFirstName = _uow.Repository<User>()
                //         .GetDetail(u => u.Id == sup.OproId)?.FirstName;   
                //     tempSup.OproLastName = _uow.Repository<User>()
                //         .GetDetail(u => u.Id == sup.OproId)?.LastName;  
                //     suppliersMap.Add(tempSup);
                // }
                //
                //
                // return Ok(suppliersMap);
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
                var supplier = _uow.ShopRepository().GetSupplier(supplierId);
                
                var supplierMap = _mapper.Map<SupplierViewMap>(supplier);
                supplierMap.OproFirstName = _uow.Repository<User>()
                    .GetDetail(u => u.Id == supplier.OproId)?.FirstName;   
                supplierMap.OproLastName = _uow.Repository<User>()
                    .GetDetail(u => u.Id == supplier.OproId)?.LastName;  
                return Ok(supplierMap);
        
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
        
        // [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpPost("supplier/update")]
        public IActionResult UpdateSupplier([FromBody] SupplierViewMap sup) 
        {
            try
            {
                
                Guid? oproId = _uow.Repository<User>()
                    .GetDetail(u => u.FirstName == sup.OproFirstName && u.LastName == sup.OproLastName)?.Id;
        
                if (oproId == null)
                {
                    return BadRequest(new {error = "OpRo name is invalid."});
                }

                sup.OproId = (Guid) oproId;
                var supplierMap = _mapper.Map<Supplier>(sup);

                _uow.ShopRepository().UpdateSupplier(supplierMap);
                
                return Ok(new {info = $"The supplier has been updated (supplier ABBR: {sup.SupplierAbbr})."});

            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        
        [Authorize(Roles = "Admin,Koty,Opro")]
        [HttpPost("supplier/add")]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierViewMap sup)
        {
            try
            {
                Guid? oproId = _uow.Repository<User>()
                    .GetDetail(u => u.FirstName == sup.OproFirstName && u.LastName == sup.OproLastName)?.Id;
        
                if (oproId == null)
                {
                    return BadRequest(new {error = "OpRo name is invalid."});
                }

                sup.OproId = (Guid) oproId;
                 
                 var supplierMap = _mapper.Map<Supplier>(sup);

                 await _uow.Repository<Supplier>().AddAsync(supplierMap);
        
                 await _uow.SaveChangesAsync();
                 return Ok(new {info = $"The supplier has been added (supplier ABBR: {sup.SupplierAbbr})."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpGet("supplier/{supplierId}/toggleAvail/")]
        public IActionResult ToggleSupplierAvailability(Guid supplierId)
        {
            try
            {
                Supplier supplier= _uow.Repository<Supplier>().GetDetail(s => s.SupplierId == supplierId);
                _uow.ShopRepository().ToggleSupplierAvailability(supplier);
                return Ok(new {info = "The supplier availability has been changed."});
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpGet("supplier/{supplierId}/toggleBlocked/")]
        public IActionResult ToggleSupplierBlocked(Guid supplierId)
        {
            try
            {
                Supplier supplier= _uow.Repository<Supplier>().GetDetail(s => s.SupplierId == supplierId);
                _uow.ShopRepository().ToggleSupplierBlocked(supplier);
                return Ok(new {info = "The supplier blocked status has been changed."});
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }

    }
}