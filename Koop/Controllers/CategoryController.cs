using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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
        public async Task<IActionResult> UpdateInsertCategory([FromBody] CategoryUpdate categoryUpdate)
        {
            try
            {
                if (categoryUpdate.CategoryId == null || categoryUpdate.CategoryId == Guid.Empty)
                {
                    Guid categoryId;

                    if (categoryUpdate.CategoryName.Length > 0)
                    {
                        var categoryNew = new Category();
                        var insertCategory = _mapper.Map(categoryUpdate, categoryNew);
                        await _uow.Repository<Category>().AddAsync(insertCategory);
                        await _uow.SaveChangesAsync();
                        categoryId = insertCategory.CategoryId;
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            error = $"The category field cannot be empty. The category was not added."
                        });
                    }

                    if (categoryId != Guid.Empty)
                    {
                        return Ok(new
                        {
                            info = "The new category has been added."
                        });
                    }

                    return BadRequest(new
                    {
                        error = $"Something went wrong, the category \'{categoryUpdate.CategoryName}\' was not added."
                    });
                }

                var category = await _uow.Repository<Category>()
                    .GetAll().FirstOrDefaultAsync(x => x.CategoryId == categoryUpdate.CategoryId);

                if (category == null)
                {
                    return BadRequest(
                        new {error = $"The category with ID: {categoryUpdate.CategoryId} is unavailable."});
                }

                if (categoryUpdate.CategoryName.Length > 0)
                {
                    categoryUpdate.Picture = category.Picture;
                    _mapper.Map(categoryUpdate, category);

                    await _uow.SaveChangesAsync();
                    return Ok(new {info = "The category has been updated."});
                }

                return BadRequest(new
                {
                    error = $"The category field cannot be empty. The category was not added."
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [HttpPost("Update/Image/Name")]
        public async Task<IActionResult> UpdateImgOfCategory([FromBody] PictureUpdate pictureUpdate)
        {
            try
            {
                if (pictureUpdate.Picture.Length > 0)
                {
                    var category = await _uow.Repository<Category>()
                        .GetAll().FirstOrDefaultAsync(x => x.CategoryId == pictureUpdate.CategoryId);

                    if (category is not null)
                    {
                        _mapper.Map(pictureUpdate, category);
                        await _uow.SaveChangesAsync();
                        return Ok(new {info = "The picture of category has been updated."});
                    }

                    return BadRequest(new
                    {
                        error = "The selected category is not available."
                    });
                }

                return BadRequest(new
                {
                    error = $"The picture name field cannot be empty. The picture was not updated."
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }

        [HttpPost("Upload/Image"), DisableRequestSizeLimit]
        public IActionResult UploadImg()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "CategoryImgs");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var guidFileName = Guid.NewGuid().ToString();
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName ?? guidFileName);
                    var dbPath = Path.Combine(folderName, fileName ?? guidFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return Ok(new {dbPath});
                }

                return BadRequest(new {error = $"The file was not uploaded to the server."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, null, e.Source);
            }
        }
    }
}