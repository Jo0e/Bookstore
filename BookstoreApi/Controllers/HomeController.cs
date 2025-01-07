using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Model;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var books = unitOfWork.BookRepository.Get(include: [a => a.Author, c => c.Category]);
            return Ok(books);
        }

        [HttpGet("Details")]
        public IActionResult Details(int bookId)
        {
            var book = unitOfWork.BookRepository.GetOne(where: b => b.Id == bookId, include: [a => a.Author, c => c.Category, cm => cm.Comments]);
            if (book != null)
                return Ok(book);
            return NotFound();
        }

        [HttpGet("Categories")]
        public IActionResult Categories()
        {
            var categories = unitOfWork.CategoryRepository.Get();
            if (categories != null)
                return Ok(categories);
            return NoContent();
        }

        [HttpGet("CategoryMovie")]
        public IActionResult Category(int categoryId)
        {
            var books = unitOfWork.BookRepository.Get(where: e => e.CategoryId == categoryId
            , include: [a => a.Category, au => au.Author]);
            if (books != null)
                return Ok(books);
            return NotFound();
        }

        [HttpGet("Wishlist")]
        public IActionResult Wishlist()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = userManager.GetUserId(User);
                var wishlist = unitOfWork.WishlistRepository.Get(where: a => a.UserId == userId);
                if (!wishlist.Any())
                {
                    return Ok(wishlist);
                }
                else
                {
                    return NoContent();
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost("AddToWishlist")]
        public IActionResult AddToWishlist(int bookId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = userManager.GetUserId(User);
                if (userId != null)
                {
                    var cartCheck = unitOfWork.WishlistRepository.Get(where: e => e.BookId == bookId && e.UserId == userId, tracked: false);
                    if (!cartCheck.Any())
                    {
                        var wishlist = new Wishlist
                        {
                            BookId = bookId,
                            UserId = userId,
                        };
                        unitOfWork.WishlistRepository.Create(wishlist);
                    }
                    else
                    {
                        var removeFromWishlist = unitOfWork.WishlistRepository.GetOne(
                            where: e => e.BookId == bookId && e.UserId == userId, tracked: false);
                        if (removeFromWishlist != null)
                            unitOfWork.WishlistRepository.Delete(removeFromWishlist);
                    }
                    unitOfWork.Complete();
                    return Ok();
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpDelete("RemoveFromWishlist")]
        public IActionResult RemoveFromWishlist(int wishlistId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var toDelete = unitOfWork.WishlistRepository.GetOne(where: a => a.Id == wishlistId);
                if (toDelete != null)
                {
                    unitOfWork.WishlistRepository.Delete(toDelete);
                    unitOfWork.Complete();
                }
                return NotFound();
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
