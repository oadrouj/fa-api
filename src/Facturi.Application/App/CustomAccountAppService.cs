using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Net.Mail;
using Facturi.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Facturi.App.Dtos;
using Microsoft.AspNetCore.Identity;
using Facturi.Users.Dto;

namespace Facturi.App
{
    public class CustomAccountAppService : ApplicationService, ICustomAccountAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public CustomAccountAppService(IRepository<User, long> userRepository, UserManager userManager, IPasswordHasher<User> passwordHasher, SignInManager<User> signInManager)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> IsEmailAddresUnique(string emailAddres)
        {
            return !(await _userRepository.GetAll().Where(u => u.EmailAddress.ToLower().Trim().Equals(emailAddres.ToLower().Trim())).ToListAsync()).Any();
        }

        public void SendConfirmationEmail(string emailAddress, string prenom, long userId)
        {
            MimeMessage message = new();

            MailboxAddress from = new("Admin", "admin@factiri.ma");
            message.From.Add(from);

            MailboxAddress to = new("User", emailAddress);
            message.To.Add(to);

            message.Subject = "Facturi - Confirmation d'adresse email";
            BodyBuilder bodyBuilder = new();
            bodyBuilder.TextBody = "http://142.11.215.22:4200/account/validateMail/" + userId;
            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate("facturi277@gmail.com", "FacturiAdmin123");

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }

        public async Task ResetPassword(long userId, string password)
        {

            var user = await _userManager.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, password);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        public void SendResetPasswordMail(string emailAddress)
        {
            var user = GetUserByEmailAddress(emailAddress);
            MimeMessage message = new();

            MailboxAddress from = new("Admin", "admin@factiri.ma");
            message.From.Add(from);

            MailboxAddress to = new("User", emailAddress);
            message.To.Add(to);

            message.Subject = "Facturi - Réinitialisation du mot de passe";
            BodyBuilder bodyBuilder = new();
            bodyBuilder.TextBody = "http://142.11.215.22:4200/account/home/" + user.Id;
            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate("facturi277@gmail.com", "FacturiAdmin123");

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }

        public UserDto GetUserByEmailAddress(string emailAddress)
        {
            var user = _userRepository.GetAll().Where(u => u.EmailAddress.Equals(emailAddress))
                            .FirstOrDefault();
            return ObjectMapper.Map<UserDto>(user);
        }
    }
}
