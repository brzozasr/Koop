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

        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpPost("By/Supplier/{supplierId}")]
        public async Task<IActionResult> ProductsBySupplier([FromRoute] Guid supplierId)
        {
            try
            {
                var units = await _uow.Repository<Unit>().GetAllAsync();
                var categories = _uow.Repository<ProductCategory>().GetAll();
                var supplier = _uow.Repository<Supplier>()
                    .GetDetail(s => s.SupplierId == supplierId);

                if (supplier == null)
                {
                    return Ok(new {info = "There is no such supplier."});
                }

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
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpPost("In/Stock")]
        public async Task<IActionResult> ProductsInStock()
        {
            try
            {
                var units = await _uow.Repository<Unit>().GetAllAsync();
                var categories = _uow.Repository<ProductCategory>().GetAll();
                var suppliers = _uow.Repository<Supplier>().GetAll();
                var products = _uow.Repository<Product>()
                    .GetAll().Where(p => p.Magazine);


                var stockStatusMap = _mapper.Map<List<StockStatus>>(products)
                    .OrderBy(s => s.StockSupplierId);

                if (products.Any())
                {
                    foreach (var stockStatus in stockStatusMap)
                    {
                        var supplier = suppliers.FirstOrDefault(s => s.SupplierId == stockStatus.SupplierId);
                        stockStatus.StockSupplierId = supplier?.SupplierId ?? Guid.Empty;
                        stockStatus.SupplierName = supplier?.SupplierName;
                        stockStatus.SupplierAbbr = supplier?.SupplierAbbr;
                        stockStatus.CategoryName = stockStatus.SetCategoriesName(categories);
                        stockStatus.UnitName = units.FirstOrDefault(u => u.UnitId == stockStatus.UnitId)?.UnitName;
                    }

                    return Ok(stockStatusMap);
                }

                return Ok(new {info = "No products available."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpPost("In/Stock/Update/Quantity/By/Id")]
        public async Task<IActionResult> ProductsInStockUpdate(
            [FromBody] ProductsQuantityUpdate stockQuantityUpdate)
        {
            try
            {
                var product = await _uow.Repository<Product>()
                    .GetAll().FirstOrDefaultAsync(p => p.ProductId == stockQuantityUpdate.ProductId && p.Magazine);

                if (product == null)
                {
                    return BadRequest(new {error = "Selected product does not exist or does not have it in stock."});
                }
                
                var isNotNegativeAmountInMagazine = stockQuantityUpdate.AmountInMagazine >= 0;
                var isNotNegativeAmountMax = stockQuantityUpdate.AmountMax >= 0;
                var areCorrectAmounts = stockQuantityUpdate.AmountMax >= stockQuantityUpdate.AmountInMagazine;

                if (!isNotNegativeAmountInMagazine)
                {
                    return BadRequest(new
                        {error = "The entered 'Amount In Magazine' must be greater or equal than 0."});
                }

                if (!isNotNegativeAmountMax)
                {
                    return BadRequest(new {error = "The entered 'Amount Max' must be greater or equal than 0."});
                }

                if (!areCorrectAmounts)
                {
                    return BadRequest(new {error = "The 'Amount In Magazine' must not be greater than 'Amount Max'."});
                }

                var quantitiesMap = _mapper.Map(stockQuantityUpdate, product);

                if (quantitiesMap == null)
                {
                    return BadRequest(new {error = "Product quantities have not been updated."});
                }

                await _uow.SaveChangesAsync();
                return Ok(new {info = "The quantity of the product has been changed."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
    }
}