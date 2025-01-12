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
using System.Net;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/Categories
        [HttpGet]
        public IActionResult Index()
        {
            var categories = unitOfWork.CategoryRepository.Get();
            if (categories != null)
            {
                return Ok(categories);
            }
            return NoContent();
        }

        // GET: api/Categories/5
        [HttpPost("Create")]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.CategoryRepository.Create(category);
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }


        [HttpGet("Edit")]
        public IActionResult Edit(int categoryId)
        {
            var category = unitOfWork.CategoryRepository.GetOne(where: a => a.Id == categoryId);
            if (category != null)
            {
                return Ok(category);
            }
            return NotFound();
        }

        // PUT: api/Categories/5
        [HttpPut("Edit")]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.CategoryRepository.Update(category);
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }

       
        // DELETE: api/Categories/5
        [HttpDelete("Delete")]
        public IActionResult Delete(int categoryId)
        {
            var category = unitOfWork.CategoryRepository.GetOne(where: b => b.Id == categoryId);
            if (category != null)
            {
                unitOfWork.CategoryRepository.Delete(category);
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }

    }
}
