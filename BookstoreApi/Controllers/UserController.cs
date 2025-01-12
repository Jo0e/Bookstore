using AutoMapper;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.Model;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UserController(UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork
            , IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = userManager.Users.ToList();
            var usersDTO = new List<UsersRolesDTO>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                usersDTO.Add(new UsersRolesDTO
                {
                    User = user,
                    Roles = roles
                });
            }
            return Ok(usersDTO);
        }
        [HttpPut("LockOut")]
        public async Task<ActionResult> Lockout(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (await userManager.GetLockoutEndDateAsync(user) > DateTime.Now)
                {
                    // Unlock user
                    var unlockResult = await userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(-1));
                    if (!unlockResult.Succeeded)
                    {
                        foreach (var error in unlockResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    // Lock user
                    var lockResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                    if (!lockResult.Succeeded)
                    {
                        foreach (var error in lockResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                return Ok();
            }
            return BadRequest();
        }



        // GET: UserController/Edit/5
        [HttpGet("Edit")]
        public async Task<ActionResult> Edit(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            var userDTO = mapper.Map<ApplicationUserDTO>(user);
            return Ok(userDTO);
        }

        // POST: UserController/Edit/5
        [HttpPut("Edit")]
        public async Task<ActionResult> Edit(ApplicationUser user, string role, IFormFile ProfileImage)
        {
            try
            {
                var existingUser = await userManager.FindByIdAsync(user.Id);
                if (existingUser == null)
                {
                    return NotFound("User not found.");
                }

                unitOfWork.UserRepository.UpdateProfileImage(user, ProfileImage);

                var appUser = mapper.Map<ApplicationUser>(user);

                var result = await userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    var oldRole = await userManager.GetRolesAsync(appUser);
                    if (oldRole.Any())
                    {
                        var removeResult = await userManager.RemoveFromRolesAsync(appUser, oldRole);
                    }

                    var addRoleResult = await userManager.AddToRoleAsync(appUser, role);
                    if (addRoleResult.Succeeded)
                    {
                        return Ok();
                    }

                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Ok(user);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok();
        }



    }
}
