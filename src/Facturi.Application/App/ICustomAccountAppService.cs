
using Abp.Application.Services;
using Facturi.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facturi.App.Dtos;
using Microsoft.AspNetCore.Identity;
using Facturi.Users.Dto;




namespace Facturi.App
{
    public interface ICustomAccountAppService : IApplicationService
    {
        Task<bool> IsEmailAddresUnique(string emailAddres);
        Task<bool> ChangePassword(long id, string currentPassword, string newPassword, string confirmPassword);
        Task<bool> ChangeEmail(long userId, string emailAddress);
        Task<UserDto> GetUserById(long id);

        long SendConfirmationEmail(string emailAddress, string prenom, long userId);

        void SendResetPasswordMail(string emailAddress);

        Task ResetPassword(long userId, string password);

        UserDto GetUserByEmailAddress(string emailAddress);

        Task<bool> checkOrUpdateConfirmationEmailIsSent(long userId);
    }
}
