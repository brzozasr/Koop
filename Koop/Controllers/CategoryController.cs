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
    public class CategoryController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private IMapper _mapper;

        public CategoryController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        
        [HttpGet("Get/Categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var result = await _uow.Repository<Category>().GetAllAsync();

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new {error = "There are no categories available"});
                }
                return Ok(result.OrderBy(x => x.CategoryName));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("Update/Insert")]
        public async Task<IActionResult> UpdateCategories(CategoryUpdate categoryUpdate)
        {
            try
            {
                if (categoryUpdate.CategoryId == null || categoryUpdate.CategoryId == Guid.Empty)
                {
                    var categoryNew = new Category();
                    var insertCategory = _mapper.Map(categoryUpdate, categoryNew);
                    await _uow.Repository<Category>().AddAsync(insertCategory);
                    await _uow.SaveChangesAsync();
                    var categoryId = insertCategory.CategoryId;

                    if (categoryId != Guid.Empty)
                    {
                        return Ok(new {info = "The new category has been added."});
                    }
                    
                    return BadRequest(new {error = $"Something went wrong, the category \'{categoryUpdate.CategoryName}\' was not added"});
                }
                
                var category = await _uow.Repository<Category>()
                    .GetAll().FirstOrDefaultAsync(x => x.CategoryId == categoryUpdate.CategoryId);

                if (category == null)
                {
                    return BadRequest(new {error = $"The category with ID: {categoryUpdate.CategoryId} is unavailable."});
                }

                _mapper.Map(categoryUpdate, category);
                
                await _uow.SaveChangesAsync();
                return Ok(new {info = "The category has been updated."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
    }
}