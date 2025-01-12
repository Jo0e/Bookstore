using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Models.Model;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public BookController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/<BookController>
        [HttpGet]
        public IActionResult Index()
        {
            var books = unitOfWork.BookRepository.Get(include: [a => a.Author, c => c.Category]);
            return Ok(books);
        }

        // POST api/<BookController>
        [HttpPost("Create")]
        public IActionResult Create(Book book, IFormFile file)
        {
            ModelState.Remove(nameof(file));
            if (ModelState.IsValid)
            {
                unitOfWork.BookRepository.CreateWithImage(book, file, "Book Covers", nameof(Book.BookCoverImg));
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("Edit")]
        public IActionResult Edit(int bookId)
        {
            var book = unitOfWork.BookRepository.GetOne(where: a => a.Id == bookId);
            if (book != null)
            {
                return Ok(book);
            }
            return NotFound();
        }

        // PUT api/<BookController>/5
        [HttpPut("Edit")]
        public IActionResult Edit(Book book, IFormFile file)
        {
            ModelState.Remove(nameof(file));
            if (ModelState.IsValid)
            {
                var oldBook = unitOfWork.BookRepository.GetOne(where: b => b.Id == book.Id, tracked: false);
                unitOfWork.BookRepository.UpdateImage(book, file, oldBook.BookCoverImg, "Book Covers", nameof(Book.BookCoverImg));
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }

        // DELETE api/<BookController>/5
        [HttpDelete("Delete")]
        public IActionResult Delete(int bookId)
        {
            var book = unitOfWork.BookRepository.GetOne(where: b => b.Id == bookId);
            if (book != null)
            {
                unitOfWork.BookRepository.DeleteWithImage(book, "Book Covers", nameof(Book.BookCoverImg));
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }

    }
}
