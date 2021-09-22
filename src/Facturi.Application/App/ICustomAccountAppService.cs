using Abp.Application.Services;
using Facturi.Users.Dto;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface ICustomAccountAppService : IApplicationService
    {
        Task<bool> IsEmailAddresUnique(string emailAddres);

        void SendConfirmationEmail(string emailAddress, string prenom, long userId);

        void SendResetPasswordMail(string emailAddress);

        Task ResetPassword(long userId, string password);

        UserDto GetUserByEmailAddress(string emailAddress);
    }
}
