using AutoMapper;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Models.DTOs;
using Models.Model;
using System.Text.Encodings.Web;
using Utilities;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailSender emailSender;
        private readonly IMapper mapper;

        public AccountController(SignInManager<ApplicationUser> signInManager
            , UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IUnitOfWork unitOfWork
            , IEmailSender emailSender
            , IMapper mapper)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
            this.emailSender = emailSender;
            this.mapper = mapper;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(ApplicationUserDTO userDTO)
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.AdminRole));
                await roleManager.CreateAsync(new(SD.AuthorRole));
                await roleManager.CreateAsync(new(SD.CustomerRole));

                await userManager.CreateAsync(new ApplicationUser
                {
                    Name = "Youssef Khaled",
                    Email = "admin@api.com",
                    UserName = "admin",
                    Address = "EGY",
                    City = "EGY",
                    ProfilePhoto = "default.jpg",
                    PhoneNumber = "1234567890",
                }, "Admin123*");
                var admin = await userManager.FindByEmailAsync("admin@api.com");
                unitOfWork.CartRepository.Create(new Cart { UserId = admin.Id });
                unitOfWork.Complete();
            }

            if (ModelState.IsValid)
            {
                var user = mapper.Map<ApplicationUser>(userDTO);
                user.UserName = userDTO.Email;
                var result = await userManager.CreateAsync(user, userDTO.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, SD.CustomerRole);
                    await signInManager.SignInAsync(user, false);
                    unitOfWork.CartRepository.Create(new Cart { UserId = user.Id });
                    unitOfWork.Complete();

                    return Ok();
                }

                return BadRequest(result.Errors);
            }

            return BadRequest(userDTO);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user != null)
            {
                var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

                if (result)
                {
                    await signInManager.SignInAsync(user, loginDTO.RememberMe);
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login Failed");
                }
            }
            return NotFound();
        }

        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }


        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);

            // Send email with this link
            await emailSender.SendEmailAsync(forgotPasswordDTO.Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>");
            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            var result = await userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.Password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }



    }
}
