using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.DataConnection;
using Models.Model;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using DataAccess.Repository;
using Stripe.Checkout;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);
            if (userId != null)
            {
                var items = unitOfWork.CartRepository.GetCartDetails(userId);
                if (items != null)
                {
                    return Ok(items);
                }
                return NoContent();
            }
            return BadRequest();
        }

        // GET: api/Cart/5
        [HttpPost("AddToCart")]
        public IActionResult AddToCart(int bookId, int quantity)
        {
            var userId = userManager.GetUserId(User);
            if (userId != null)
            {
                var book = unitOfWork.BookRepository.GetOne(where: o => o.Id == bookId);
                if (book != null)
                {
                    var cart = unitOfWork.CartRepository.GetCartDetails
                        (userId).FirstOrDefault();

                    var ifItemExist = cart.Items.Where(i => i.Book == book).FirstOrDefault();
                    if (ifItemExist != null)
                    {
                        ifItemExist.Quantity += quantity;
                        unitOfWork.CartItemRepository.Update(ifItemExist);
                    }
                    else
                    {
                        var newItems = new CartItem
                        { Book = book, Quantity = quantity, UserId = userId, CartId = cart.Id };
                        unitOfWork.CartItemRepository.Create(newItems);
                    }
                    unitOfWork.Complete();
                    return Ok();
                }
                return BadRequest();
            }
            return RedirectToAction("Register", "Account");
        }


        // DELETE: api/Cart/5
        [HttpDelete("Delete")]
        public IActionResult Delete(int cartItemId)
        {
            var item = unitOfWork.CartItemRepository.GetOne(where: a => a.Id == cartItemId);
            if (item != null)
            {
                unitOfWork.CartItemRepository.Delete(item);
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }


        [HttpPut("Increment")]
        public IActionResult Increment(int cartItemId)
        {
            var item = unitOfWork.CartItemRepository.GetOne(where: e => e.Id == cartItemId);
            if (item != null)
            {
                item.Quantity++;
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("Decrement")]
        public IActionResult Decrement(int cartItemId)
        {
            var item = unitOfWork.CartItemRepository.GetOne(where: e => e.Id == cartItemId);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity == 0)
                {
                    unitOfWork.CartItemRepository.Delete(item);
                }
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost("Pay")]
        public IActionResult Pay()
        {
            var userId = userManager.GetUserId(User);
            if (userId == null)
            {
                return BadRequest();
            }
            var userCart = unitOfWork.CartRepository.GetCartDetails(userId).FirstOrDefault();
            if (!userCart.Items.Any())
            {
                return BadRequest();
            }
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Cart/Success?userId={userId}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Cart/Cancel",
            };

            foreach (var item in userCart.Items)
            {
                var result = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Book.Title,
                        },
                        UnitAmount = (long)item.Book.Price * 100,
                    },
                    Quantity = item.Quantity,
                };
                options.LineItems.Add(result);
            }


            var service = new SessionService();
            var session = service.Create(options);
            return Ok(new { redirectUrl = session.Url });
        }

        [HttpGet("Success")]
        public IActionResult Success(string userId)
        {
            var userCart = unitOfWork.CartRepository.GetCartDetails(userId).FirstOrDefault();

            PaymentRecord paymentRecord = new()
            {
                UserId = userId,
                TotalAmount = userCart.Items.Sum(a => a.Book.Price * a.Quantity),
                PaymentDate = DateTime.Now,
            };

            foreach (var item in userCart.Items)
            {
                paymentRecord.PurchasedItems.Add(new PurchasedItem
                {
                    BookTitle = item.Book.Title,
                    BookPrice = item.Book.Price,
                    Quantity = item.Quantity,
                });
            }

            unitOfWork.PaymentRecordRepository.Create(paymentRecord);
            unitOfWork.CartItemRepository.DeleteRange(userCart.Items);

            unitOfWork.Complete();

            return Ok();
        }
        [HttpGet("Cancel")]
        public IActionResult Cancel()
        {
            return BadRequest();
        }


    }
}
