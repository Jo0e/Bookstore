using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Model;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;

        public ContactUsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ContactUs contactUs, IFormFile ImgFile)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            contactUs.UserId = user.Id;
            contactUs.Name = user.Email;
            ModelState.Remove(nameof(ImgFile));
            if (ModelState.IsValid)
            {
                unitOfWork.ContactUsRepository.CreateWithImage(contactUs, ImgFile, "ContactUsImages", "UserImgRequest");
                var message = ConfirmationMessage(user.Id);
                unitOfWork.MessageRepository.Create(message);
                unitOfWork.Complete();

                return Ok();
            }
            return BadRequest(contactUs);
        }

        private static Message ConfirmationMessage(string userId)
        {
            var message = new Message
            {
                UserId = userId,
                Title = "Contact Us",
                MessageString = $"Thank you for Contacting us, \r\nYour Request Has been submitted successfully ",
                MessageDateTime = DateTime.Now,
            };
            return message;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var contactMessage = unitOfWork.ContactUsRepository.Get().OrderByDescending(a => a.Id);
            if (contactMessage.Any())
            {
                return Ok(contactMessage);
            }
            return NoContent();
        }

        [HttpGet("Details")]
        public IActionResult Details(int reqId)
        {
            var contact = unitOfWork.ContactUsRepository.ContactDetail(reqId);

            if (contact != null)
            {
                if (contact.IsReadied == false)
                {
                    contact.IsReadied = true;
                    unitOfWork.Complete();
                }
                return Ok(contact);
            }
            return BadRequest();
        }

        [HttpPut("ReadMessage")]
        public IActionResult ReadMessage(int reqId)
        {
            var contact = unitOfWork.ContactUsRepository.GetOne(where: a => a.Id == reqId);
            if (contact != null)
            {
                if (!contact.IsReadied)
                {
                    contact.IsReadied = true;
                }
                return Ok();
            }
            return NotFound();
        }

        [HttpPost("Respond")]
        public async Task<IActionResult> Respond(Message message)
        {
            message.MessageDateTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                unitOfWork.MessageRepository.Create(message);
                unitOfWork.Complete();
                return Ok();
            }
            return BadRequest(message);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int reqId)
        {
            var toDelete = unitOfWork.ContactUsRepository.GetOne(where: a => a.Id == reqId);
            if (toDelete != null)
            {
                unitOfWork.ContactUsRepository.DeleteWithImage(toDelete, "ContactUsImages", toDelete.UserImgRequest);
                await unitOfWork.CompleteAsync();
            }
            return Ok();
        }


    }
}
