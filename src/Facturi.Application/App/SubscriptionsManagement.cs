using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Zero.Configuration;
using Facturi.App.Dtos;
using Facturi.Authorization;
using Facturi.Authorization.Accounts.Dto;
using Facturi.Authorization.Users;
using Facturi.Features;
using Facturi.Identity;

namespace Facturi.App
{
    [AbpAuthorize()]
    public class SubscriptionsManagement: ApplicationService, ISubscriptionsManagement
    {

        private readonly UserManager _userManager;
        private readonly IInfosEntrepriseAppService _infosEntrepriseAppService;

        public SubscriptionsManagement(
            UserManager userManager,
            IInfosEntrepriseAppService infosEntrepriseAppService
        )
        {
            _userManager = userManager;
            _infosEntrepriseAppService = infosEntrepriseAppService;
        }

        public async Task<bool> createFreeSubscription(long userId)
        {
            try
            {
                var createInfosEntrepriseInput = new CreateInfosEntrepriseInput();
                await _infosEntrepriseAppService.CreateInfosEntreprise(createInfosEntrepriseInput);
                return true;

            }

            catch (Exception e)
            {
                return false;
            }


        }
    }
}
