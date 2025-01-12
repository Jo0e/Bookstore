using Microsoft.AspNetCore.Http;
using Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser> 
    {
        void CreateProfileImage(ApplicationUser entity, IFormFile imageFile);
        void UpdateProfileImage(ApplicationUser entity, IFormFile newImageFile);
    }
}
