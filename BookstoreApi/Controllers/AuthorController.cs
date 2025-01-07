using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Models.Model;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public AuthorController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/<AuthorController>
        [HttpGet]
        public IActionResult Index()
        {
            var authors = unitOfWork.AuthorRepository.Get();
            if (authors != null)
            {
                return Ok(authors);
            }
            return NoContent();
        }


        // POST api/<AuthorController>
        [HttpPost("Create")]
        public IActionResult Create(Author author , IFormFile file)
        {
            ModelState.Remove(nameof(file));
            if (ModelState.IsValid)
            {
                unitOfWork.AuthorRepository.CreateWithImage(author,file,"Profiles" ,nameof(Author.ProfilePhoto));
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }

        // PUT api/<AuthorController>/5
        [HttpPut("Edit")]
        public IActionResult Edit(Author author, IFormFile file)
        {
            ModelState.Remove(nameof(file));
            if (ModelState.IsValid)
            {
                var oldAuthor = unitOfWork.AuthorRepository.GetOne(where: a=>a.Id==author.Id);
                unitOfWork.AuthorRepository.UpdateImage(author,file,oldAuthor.ProfilePhoto, "Profiles", nameof(Author.ProfilePhoto));
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }

        // DELETE api/<AuthorController>/5
        [HttpDelete("Delete")]
        public IActionResult Delete(int authorId)
        {
            var author = unitOfWork.AuthorRepository.GetOne(where: b => b.Id == authorId);
            if (author != null)
            {
                unitOfWork.AuthorRepository.DeleteWithImage(author, "Profiles" , nameof(Author.ProfilePhoto));
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }
    }
}
