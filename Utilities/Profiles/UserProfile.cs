using AutoMapper;
using Models.DTOs;
using Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUserDTO, ApplicationUser>()
                .ForMember(dest => dest.Id, option => option.Ignore());
        }
    }
}
