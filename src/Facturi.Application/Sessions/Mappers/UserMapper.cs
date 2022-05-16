using Facturi.Sessions.Dto;
using Facturi.Users.Dto;

using Facturi.Authorization.Users;

namespace Facturi.Sessions.Mappers
{
    public class UserMapper
    {
        public static UserLoginInfoDto MapToEntityDto(User user)
        {
            return new UserLoginInfoDto { UserName=user.UserName,Id=user.Id,EmailAddress=user.EmailAddress,
            Name=user.Name,Surname=user.Surname};
        }
    }
}
