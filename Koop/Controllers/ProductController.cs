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
    public class ProductController : Controller
    {
        private IGenericUnitOfWork _uow;
        private IMapper _mapper;
        
        public ProductController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        
        [AllowAnonymous]
        // [Authorize(Roles = "Admin,Koty,PoRo")]
        [HttpPost("Supplier/{supplierId}")]
        public async Task<IActionResult> ProductsBySupplier(Guid supplierId)
        {
            try
            {
                var units = await _uow.Repository<Unit>().GetAllAsync();
                var categories = _uow.Repository<ProductCategory>().GetAll();
                var supplier = _uow.Repository<Supplier>()
                    .GetDetail(s => s.SupplierId == supplierId);
                
                // TODO check the tru condition
                var supplierProducts = _uow.Repository<Product>()
                    .GetAllAsync().Result.Where(p => p.SupplierId == supplierId && p.Magazine == false);

                var supplierMap = _mapper.Map<SupplierProducts>(supplier);
                var supplierProductsMap = _mapper.Map<List<SupplierProductsNode>>(supplierProducts);

                foreach (var product in supplierProductsMap)
                {
                    product.CategoryName = product.SetCategoriesName(categories);
                    product.UnitName = units.FirstOrDefault(u => u.UnitId == product.UnitId)?.UnitName;
                    supplierMap.SupplierProductsList.Add(product);
                }

                if (supplierProducts.Any())
                {
                    return Ok(supplierMap);
                }

                return Ok(new {info = "No products available."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
            
        }
        
        [AllowAnonymous]
        // [Authorize(Roles = "Admin,Koty,PoRo")]
        [HttpPost("Magazine/")]
        public async Task<IActionResult> ProductsInMagazine(Guid supplierId)
        {
            try
            {
                var units = await _uow.Repository<Unit>().GetAllAsync();
                var supplier = _uow.Repository<Supplier>()
                    .GetDetail(s => s.SupplierId == supplierId);
                
                // TODO check the tru condition
                var supplierProducts = _uow.Repository<Product>()
                    .GetAllAsync().Result.Where(p => p.SupplierId == supplierId && p.Magazine == false);

                var supplierMap = _mapper.Map<SupplierProducts>(supplier);
                var supplierProductsMap = _mapper.Map<List<SupplierProductsNode>>(supplierProducts);

                foreach (var product in supplierProductsMap)
                {
                    product.UnitName = units.FirstOrDefault(u => u.UnitId == product.UnitId)?.UnitName;
                    supplierMap.SupplierProductsList.Add(product);
                }

                if (supplierProducts.Any())
                {
                    return Ok(supplierMap);
                }

                return Ok(new {info = "No products available."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
            
        }
    }
}