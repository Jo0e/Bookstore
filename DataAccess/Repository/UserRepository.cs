using DataAccess.DataConnection;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void CreateProfileImage(ApplicationUser entity, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var profileFolderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\Profiles\\{entity.Email}");
                if (!Directory.Exists(profileFolderPath))
                {
                    Directory.CreateDirectory(profileFolderPath);
                }
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                var filePath = Path.Combine(profileFolderPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    imageFile.CopyTo(stream);
                }

                entity.ProfilePhoto = fileName;

            }

        }


        public void UpdateProfileImage(ApplicationUser entity, IFormFile newImageFile)
        {
            if (newImageFile != null && newImageFile.Length > 0)
            {
                var profileFolderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\Profiles\\{entity.Email}");

                // Ensure the directory exists
                if (!Directory.Exists(profileFolderPath))
                {
                    Directory.CreateDirectory(profileFolderPath);
                }

                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(newImageFile.FileName);
                var newFilePath = Path.Combine(profileFolderPath, newFileName);

                // Delete the old profile photo if it exists
                if (!string.IsNullOrEmpty(entity.ProfilePhoto))
                {
                    var oldFilePath = Path.Combine(profileFolderPath, entity.ProfilePhoto);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Save the new profile photo
                using (var stream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                {
                    newImageFile.CopyTo(stream);
                }

                // Update the entity with the new file name
                entity.ProfilePhoto = newFileName;

            }

        }



    }
}
