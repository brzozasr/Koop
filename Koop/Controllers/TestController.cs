using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Koop.Extensions;
using Koop.models;
using Koop.Models;
using Koop.Models.Auth;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Koop.Models.Util;
using Koop.Services.MailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using NetTopologySuite.Operation.Union;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private IMyEmailSender _emailSender;

        public TestController(IGenericUnitOfWork genericUnitOfWork, IMyEmailSender emailSender)
        {
            _uow = genericUnitOfWork;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [AllowAnonymous]
        [HttpGet("NoAuth")]
        public IActionResult NoAuth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [Authorize]
        [HttpGet("Auth")]
        public IActionResult Auth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [Authorize(Policy = "Szymek")]
        [HttpGet("AuthUserName")]
        public IActionResult AuthUserName()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [Authorize(Roles = "Koty")]
        [HttpGet("AuthRole")]
        public IActionResult AuthRole()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [Authorize]
        [HttpGet("products")]
        public IActionResult Products(string orderBy = "name", int start = 0, int count = 10, string orderDir = "asc",
            Guid categoryId = default(Guid))
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            orderBy = orderBy.ToLower();
            Expression<Func<ProductsShop, object>> order = orderBy switch
            {
                "productId" => p => p.ProductId,
                "name" => p => p.ProductName,
                "price" => p => p.Price,
                "blocked" => p => p.Blocked,
                "available" => p => p.Available,
                "unit" => p => p.Unit,
                "amountmax" => p => p.AmountMax,
                "supplierabbr" => p => p.SupplierAbbr,
                _ => p => p.ProductName
            };

            OrderDirection direction = orderDir switch
            {
                "asc" => OrderDirection.Asc,
                "desc" => OrderDirection.Desc,
                _ => OrderDirection.Asc
            };

            return Ok(_uow.ShopRepository()
                .GetProductsShop(Guid.Parse(userId), order, start, count, direction, categoryId));
        }

        [HttpGet("product/{productId}/get")]
        public IActionResult Product(Guid productId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetProductById(productId));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("product/add")]
        public IActionResult AddProduct(Product product)
        {
            var response = _uow.ShopRepository().AddProduct(product);

            return ToResult(response);
        }

        [HttpPost("product/update")]
        public IActionResult UpdateProduct()
        {
            IFormFile file;
            if (Request.Form.Files is not null && Request.Form.Files.Count > 0)
            {
                file = Request.Form.Files[0];
            }
            else
            {
                file = null;
            }

            var data = Request.Form["data"].ToString();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            var jdata = (JObject) JsonConvert.DeserializeObject(data);
            var product = JsonSerializer.Deserialize<Product>(data, options);

            var availQuantS = JsonConvert.SerializeObject(jdata["availQuantity"]);
            var availQuantity = JsonSerializer.Deserialize<List<AvailableQuantity>>(availQuantS, options);

            var categoryS = JsonConvert.SerializeObject(jdata["category"]);
            var category = JsonSerializer.Deserialize<List<Category>>(categoryS, options);

            product.Category = category;
            product.AvailQuantity = availQuantity;

            var response = _uow.ShopRepository().UpdateProduct(product, file);

            return Ok(response);
        }

        /*[HttpPost("product/update")]
        public IActionResult UpdateProduct(Product product)
        {
            var response = _uow.ShopRepository().UpdateProduct(product);
            
            return Ok(response);
        }*/

        [HttpDelete("product/remove")]
        public IActionResult RemoveProduct(IEnumerable<Product> products)
        {
            var response = _uow.ShopRepository().RemoveProduct(products);

            return ToResult(response);
        }

        [HttpGet("product/categories")]
        public IActionResult GetProductCatgeories(Guid productId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetProductCategories(productId));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("product/categories/update")]
        public IActionResult UpdateCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            var response = _uow.ShopRepository().UpdateProductCategories(productCategoriesCombos);

            return ToResult(response);
        }

        [HttpDelete("product/categories/remove")]
        public IActionResult RemoveCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            var response = _uow.ShopRepository().RemoveProductCategories(productCategoriesCombos);

            return ToResult(response);
        }

        [HttpGet("product/availQuantities")]
        public IActionResult GetProductAvailQuantities(Guid productId)
        {
            try
            {
                var result = _uow.ShopRepository().GetAvailableQuantities(productId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpGet("product/availAllQuantities")]
        public IActionResult GetProductAllAvailQuantities(Guid productId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetAllAvailableQuantities(productId));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("product/availQuantities/update")]
        public IActionResult UpdateAvailQuantities(IEnumerable<AvailableQuantity> availableQuantities)
        {
            var response = _uow.ShopRepository().UpdateAvailableQuantities(availableQuantities);

            return ToResult(response);
        }

        [HttpDelete("product/availQuantities/remove")]
        public IActionResult RemoveAvailQuantities(IEnumerable<AvailableQuantity> availableQuantities)
        {
            var response = _uow.ShopRepository().RemoveAvailableQuantities(availableQuantities);

            return ToResult(response);
        }

        [Authorize]
        [HttpGet("allUnits")]
        public IActionResult GetAllUnits()
        {
            try
            {
                return Ok(_uow.Repository<Unit>().GetAll());
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("units/update")]
        public IActionResult UnitsUpdate(IEnumerable<Unit> units)
        {
            var response = _uow.ShopRepository().UpdateUnits(units);

            return ToResult(response);
        }

        [HttpDelete("units/remove")]
        public IActionResult RemoveUnits(IEnumerable<Unit> units)
        {
            var response = _uow.ShopRepository().RemoveUnits(units);

            return ToResult(response);
        }

        [HttpGet("product/{productId}/unit")]
        public IActionResult GetProductUnit(Guid productId)
        {
            try
            {
                return Ok(_uow.Repository<Product>().GetAll().Where(p => p.ProductId == productId).Select(p => p.Unit));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            try
            {
                return Ok(_uow.Repository<Category>().GetAll());
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("categories/update")]
        public IActionResult UpdateCategories(IEnumerable<Category> categories)
        {
            var response = _uow.ShopRepository().UpdateCategories(categories);

            return ToResult(response);
        }

        [HttpDelete("categories/remove")]
        public IActionResult RemoveCategories(IEnumerable<Category> categories)
        {
            var response = _uow.ShopRepository().RemoveCategories(categories);

            return ToResult(response);
        }

        [Authorize]
        [HttpGet("order/make/")]
        public IActionResult MakeOrder(Guid productId, int quantity)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is not null)
            {
                var response = _uow.ShopRepository().MakeOrder(productId, Guid.Parse(userId), quantity);

                return Ok(response);
            }

            return Problem("Your identity could not be verified.", null, 500);
        }

        [HttpGet("product/isAvailable")]
        public IActionResult CheckProductAvailability(Guid productId)
        {
            ProblemResponse result;
            try
            {
                result = _uow.ShopRepository().CheckProductAvailability(productId);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("order/orderedItems/count")]
        public IActionResult GetOrderedItemsCount()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is not null)
            {
                var response = _uow.ShopRepository().GetOrderedItemsCount(Guid.Parse(userId));

                return Ok(response);
            }

            return Problem("Your identity could not be verified.", null, 500);
        }

        [HttpGet("user/{coopId}/order/{orderId}")]
        public IActionResult CoopOrder(Guid coopId, Guid orderId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetCooperatorOrders(coopId, orderId));
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        // [Authorize(Roles = "Default")]
        [HttpGet("User/{coopId:guid}/Order/In/Basket")]
        public IActionResult CoopOrderInBasket([FromRoute] Guid coopId)
        {
            try
            {
                var orderStatusIdOpened = _uow.Repository<OrderStatus>().GetAll()
                    .FirstOrDefault(x => x.OrderStatusName == OrderStatuses.Otwarte.ToString())?
                    .OrderStatusId;

                var orderGrande = _uow.Repository<Order>().GetAll()
                    .OrderByDescending(x => x.OrderStartDate)
                    .FirstOrDefault();

                if (orderGrande is not null && orderStatusIdOpened.HasValue &&
                    orderGrande.OrderStatusId == orderStatusIdOpened.Value)
                {
                    var order = _uow.ShopRepository().GetCooperatorOrders(coopId, orderGrande.OrderId)
                        .Where(field => field.OrderStatus == OrderStatuses.Zaplanowane.ToString())
                        .OrderBy(x => x.ProductName);

                    if (order.Any())
                    {
                        return Ok(order);
                    }

                    return Ok(new {info = "The basket is empty."});
                }

                return BadRequest(new {error = "No Grande orders."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("orderedItem/{orderedItemId:guid}/setQuantity/{quantity:int}")]
        public IActionResult UpdateUserOrderQuantity(Guid orderedItemId, int quantity)
        {
            var response = _uow.ShopRepository().UpdateUserOrderQuantity(orderedItemId, quantity);

            return ToResult(response);
        }

        [HttpPost("orderedItem/{orderedItemId:guid}/remove")]
        public IActionResult RemoveUserOrder(Guid orderedItemId)
        {
            ShopRepositoryReturn response = _uow.ShopRepository().RemoveUserOrder(orderedItemId);

            return ToResult(response);
        }

        // [Authorize(Roles = "Default")]
        [HttpPost("User/{coopId:guid}/Order/Submit")]
        public IActionResult SubmitYourOrder([FromRoute] Guid coopId)
        {
            try
            {
                var lastOrderGrandeId = _uow.Repository<Order>().GetAll()
                    .OrderByDescending(x => x.OrderStartDate)
                    .FirstOrDefault()?.OrderId;

                var orderStatusIdPlaned = _uow.Repository<OrderStatus>().GetAll()
                    .FirstOrDefault(x => x.OrderStatusName == OrderStatuses.Zaplanowane.ToString())?
                    .OrderStatusId;

                var orderStatusIdClosed = _uow.Repository<OrderStatus>().GetAll()
                    .FirstOrDefault(x => x.OrderStatusName == OrderStatuses.Zamknięte.ToString())?
                    .OrderStatusId;

                if (lastOrderGrandeId.HasValue)
                {
                    var orders = _uow.Repository<OrderedItem>().GetAll()
                        .Include(p => p.Product)
                        .Include(c => c.Coop)
                        .Where(field => field.OrderId == lastOrderGrandeId &&
                                        field.CoopId == coopId && field.OrderStatusId == orderStatusIdPlaned)
                        .ToList();

                    if (orders.Any())
                    {
                        if (orderStatusIdClosed.HasValue)
                        {
                            var culture = CultureInfo.CreateSpecificCulture("pl-PL");

                            var tBody = @"<tr>
                                <td class=""brd"" style=""padding: 0 10px 0 10px;"">#productName#</td>
                                <td class=""brd"" style=""text-align: center; padding: 0 10px 0 10px;"">#quantity#</td>
                                <td class=""brd"" style=""text-align: right; padding: 0 10px 0 10px;"">#price#</td>
                                <td class=""brd"" style=""text-align: right; padding: 0 10px 0 10px;"">#totalPrice#</td>
                                </tr>";
                            var tFoot = @"<tr class=""brd"" style=""font-weight: bold"">
                                <td style=""padding: 0 10px 0 10px;"">RAZEM</td>
                                <td style=""padding: 0 10px 0 10px; text-align: center;"">&nbsp;</td>
                                <td style=""padding: 0 10px 0 10px; text-align: right;"">&nbsp;</td>
                                <td style=""padding: 0 10px 0 10px; text-align: right;"">#sumPrice#</td>
                                </tr>";

                            StringBuilder sbTBody = new StringBuilder();
                            var sumPrice = 0d;

                            foreach (var item in orders)
                            {
                                item.OrderStatusId = orderStatusIdClosed.Value;

                                var price = Math.Round(item.Product.Price, 2, MidpointRounding.AwayFromZero);
                                var totalPrice = Math.Round(price * item.Quantity, 2, MidpointRounding.AwayFromZero);
                                sumPrice += totalPrice;

                                sbTBody.Append(tBody
                                    .Replace("#productName#", item.Product.ProductName)
                                    .Replace("#quantity#", item.Quantity.ToString())
                                    .Replace("#price#", $"{price.ToString("#0.00", culture)} zł")
                                    .Replace("#totalPrice#", $"{totalPrice.ToString("#0.00", culture)} zł"));
                            }

                            tFoot = tFoot.Replace("#sumPrice#",
                                $"{Math.Round(sumPrice, 2, MidpointRounding.AwayFromZero).ToString("#0.00", culture)} zł");

                            _emailSender.SendOrderConfirmation(orders[0].Coop.Email, sbTBody.ToString(), tFoot);

                            _uow.SaveChanges();
                            return Ok(new {info = "The order has been accepted."});
                        }

                        return BadRequest(new {error = "There is a problem with the order status."});
                    }

                    return BadRequest(new {error = "The basket is empty."});
                }

                return BadRequest(new {error = "No Grande orders"});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        // [HttpGet("supplier/{abbr}")]
        // public IActionResult Supplier(string abbr)
        // {
        //     return Ok(_uow.ShopRepository().GetSupplier(abbr));
        // }


        // [HttpGet("supplier/{abbr}/edit")]
        // public IActionResult EditSupplier(string abbr)
        // {
        //     return Ok(_uow.ShopRepository().GetSupplier(abbr));
        // }

        [HttpPost("user/{userId}/order/{orderId}/setStatus/{statusId}")]
        public IActionResult UpdateUserOrderStatus(Guid orderId, Guid userId, Guid statusId)
        {
            var response = _uow.ShopRepository().UpdateUserOrderStatus(orderId, userId, statusId);

            return ToResult(response);
        }

        /*[HttpPost("user/{userId}/order/{orderId}/setStatus/{statusId}")]
        public IActionResult UpdateUserOrderStatus(Guid orderId, Guid userId, Guid statusId)
        {
            var response = _uow.ShopRepository().UpdateUserOrderStatus(orderId, userId, statusId);
            
            return ToResult(response);
        }*/


        // [HttpGet("allsuppliers")]
        // public IActionResult AllSuppliers()
        // {
        //     return Ok(_uow.Repository<Supplier>().GetAll());
        // }

        [HttpGet("cooperator/{firstname}+{lastname}/history")]
        public IActionResult UserOrdersHistoryView(string firstName, string lastName)
        {
            return Ok(_uow.ShopRepository().GetUserOrders(firstName, lastName));
        }

        // [HttpGet("order/baskets")]
        // public IActionResult BasketName()
        // {
        //     return Ok(_uow.ShopRepository().GetBaskets());
        // }
        //
        // [HttpGet("bigorders")]
        // public IActionResult BigOrders()
        // {
        //     return Ok(_uow.Repository<Order>().GetAll());
        // }

        [NonAction]
        private IActionResult ToResult(ShopRepositoryReturn shopRepositoryReturn)
        {
            try
            {
                _uow.SaveChanges();
                return Ok(shopRepositoryReturn.ToObject());
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [Authorize(Roles = "Admin,Koty")]
        [HttpDelete("supplier/{supplierId}/remove")]
        public async Task<IActionResult> RemoveSupplier(Guid supplierId)
        {
            try
            {
                // IEnumerable<Product> products = _uow.ShopRepository().GetProductsBySupplier(supplierId);
                // _uow.ShopRepository().RemoveProduct(products);

                var supplier = _uow.Repository<Supplier>()
                    .GetDetail(s => s.SupplierId == supplierId);

                _uow.Repository<Supplier>().Delete(supplier);

                await _uow.SaveChangesAsync();
                return Ok(new {info = $"The supplier has been deleted (supplier ABBR: {supplier.SupplierAbbr})."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }

        [HttpGet("funds")]
        public IActionResult GetAllFunds()
        {
            try
            {
                return Ok(_uow.Repository<Fund>().GetAll());
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("allsuppliers")]
        public IActionResult AllSuppliers()
        {
            try
            {
                var suppliers = _uow.Repository<SupplierView>().GetAll().Select(p => new
                {
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierName,
                    SupplierAbbr = p.SupplierAbbr
                });
                return Ok(suppliers);
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }

        [HttpGet("order/isopen")]
        public IActionResult IsOrderOpen()
        {
            var response = _uow.ShopRepository().IsOrderOpen();
            return Ok(response);
        }

        [HttpGet("order/checkStatus")]
        public IActionResult CheckOrderStatus()
        {
            var response = _uow.ShopRepository().CheckOrderStatus();
            return Ok(response);
        }
    }
}