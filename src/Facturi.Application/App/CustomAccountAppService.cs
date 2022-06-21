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
using MailKit.Security;
using MimeKit;
using Facturi.App.Dtos;
using Microsoft.AspNetCore.Identity;
using Facturi.Users.Dto;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Facturi.App
{
    public class CustomAccountAppService : ApplicationService, ICustomAccountAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IConfiguration _config;




        public CustomAccountAppService(
            IRepository<User, long> userRepository, 
            UserManager userManager, 
            IPasswordHasher<User> passwordHasher, 
            SignInManager<User> signInManager,
            IConfiguration config
        )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        public async Task<bool> IsEmailAddresUnique(string emailAddres)
        {
            return !(await _userRepository.GetAll().Where(u => u.EmailAddress.ToLower().Trim().Equals(emailAddres.ToLower().Trim())).ToListAsync()).Any();
        }

        public long SendConfirmationEmail(string emailAddress, string prenom, long userId)
        {
            try
            {
                string server = _config.GetValue<string>("Smtp:Server");
                int port = Int16.Parse(_config.GetValue<string>("Smtp:Port"));
                string fromAddress = _config.GetValue<string>("Smtp:FromAddress");
                string authEmail = _config.GetValue<string>("Smtp:userEmailAuth");
                string authPass = _config.GetValue<string>("Smtp:userPassAuth");
                MimeMessage message = new();

                MailboxAddress from = new("Admin", fromAddress);
                message.From.Add(from);

                MailboxAddress to = new("User", emailAddress);
                message.To.Add(to);

                message.Subject = "Facturi - Confirmation d'adresse email";
                BodyBuilder bodyBuilder = new();
                var url = _config["App:ClientRootAddress"];

                bodyBuilder.TextBody = $"{url}/account/validateMail/" + userId;
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(server, port, SecureSocketOptions.Auto);
                client.Authenticate(authEmail, authPass);
                //client.Authenticate("rmndkkrs01@gmail.com", "Tdi&&2011");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            
            return userId;
        }

        public async Task<bool> checkOrUpdateConfirmationEmailIsSent(long userId)
        {
            var user = await _userRepository.FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                return false;
            }
            else
            {
                if (user.IsEmailConfirmed == true)
                    return false;

                user.IsEmailConfirmed = true;
                await _userRepository.UpdateAsync(user);
                return true;
            }
           
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
            string server = _config.GetValue<string>("Smtp:Server");
            int port = Int16.Parse(_config.GetValue<string>("Smtp:Port"));
            string fromAddress = _config.GetValue<string>("Smtp:FromAddress");
            string authEmail = _config.GetValue<string>("Smtp:userEmailAuth");
            string authPass = _config.GetValue<string>("Smtp:userPassAuth");
            try
            {
                var user = GetUserByEmailAddress(emailAddress);
                MimeMessage message = new();

                MailboxAddress from = new("Admin", fromAddress);
                message.From.Add(from);

                MailboxAddress to = new("User", emailAddress);
                message.To.Add(to);

                message.Subject = "Facturi - Réinitialisation du mot de passe";
                BodyBuilder bodyBuilder = new();
                var url = _config["App:ClientRootAddress"];
                bodyBuilder.TextBody = $"{url}/account/home/" + user.Id;
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(server, port, SecureSocketOptions.Auto);
                client.Authenticate(authEmail, authPass);

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
           
        }

        public UserDto GetUserByEmailAddress(string emailAddress)
        {
            var user = _userRepository.GetAll().Where(u => u.EmailAddress.Equals(emailAddress))
                            .FirstOrDefault();
            return ObjectMapper.Map<UserDto>(user);
        }
    }
}
