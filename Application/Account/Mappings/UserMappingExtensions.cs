using Application.Account.DTOs;
using Application.Account.DTOs.Application.Account;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Account.Mappings
{
    public static class UserMappingExtensions
    {
        public static UserInfoDto? ToDto(this AppUser? user)
        {
            if (user == null) return null;

            return new UserInfoDto
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Address = user.Address.ToDto()
            };
        }
    }
}
