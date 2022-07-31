using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Facturi.App.Dtos;
using Facturi.App.Dtos.GenericDtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public class ClientAppService : ApplicationService, IClientAppService
    {
        private readonly IRepository<Client, long> _clientRepository;
        private readonly IRepository<Facture, long> _devisRepository;
        private readonly IRepository<Facture, long> _factureRepository;
        public ClientAppService(
            IRepository<Client, long> clientRepository,
             IRepository<Facture, long> devisRepository,
            IRepository<Facture, long> factureRepository
        )
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _devisRepository = devisRepository ?? throw new ArgumentNullException(nameof(devisRepository));
            _factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
        }

        public async Task<ClientDto> CreateClient(ClientDto input)
        {
            input.Reference = 1;
            ClientDto result = null;
            try
            {
                int nombreClients = _clientRepository.Count();
                var maxRefClient = _clientRepository.GetAll();
          
                if (nombreClients != 0)
                {
                    maxRefClient = maxRefClient.Where(c => c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId).OrderByDescending(c => c.Reference);
                    if (maxRefClient != null && maxRefClient.Any())
                    {
                        input.Reference = maxRefClient.ToList().First().Reference + 1;
                        input.DisplayName = input.CategorieClient == "PRFS" ? input.RaisonSociale : input.Nom;
                    }

                }

                var client = ObjectMapper.Map<Client>(input);
                var Id = await _clientRepository.InsertAndGetIdAsync(client);
                result = ObjectMapper.Map<ClientDto>(client);
            }
            catch (Exception e)
            {

                throw new Exception(e.ToString());
            }
           
            
            return result;
        }

        public async Task<ClientDto> UpdateClient(ClientDto input)
        {
            input.DisplayName = input.CategorieClient == "PRFS" ? input.RaisonSociale : input.Nom;
            var client = ObjectMapper.Map<Client>(input);
            var result = await _clientRepository.UpdateAsync(client);
            return ObjectMapper.Map<ClientDto>(result);
        }

        public async Task<ClientDto> GetByIdClient(long id)
        {
            var clients = await _clientRepository.GetAll().Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId) && c.Id == id).ToListAsync();
            var result = ObjectMapper.Map<ClientDto>(clients.First());
            return result;
        }

        public async Task<ListResultDto<ClientDto>> GetByCategClient(string categ)
        {
            var clients = await _clientRepository.GetAll().Where(c => c.CategorieClient.Equals(categ) && (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId)).ToListAsync();

            var result = new ListResultDto<ClientDto>(ObjectMapper.Map<List<ClientDto>>(clients));
            return result;
        }

        public async Task<ListResultWithTotalEntityItemsDto<ClientDto>> GetAllClients(ListCriteriaDto listCriteria)
        {
           CheckIfIsRefSearch(listCriteria, out bool isRef, out int minRef, out int maxRef);
           CheckIfIsFilterSearch(listCriteria, out string type, out string categorie);
            var clients = new List<Client>();
            var query = _clientRepository.GetAll();
            int nombresClients = _clientRepository.Count();
            if (nombresClients !=0)
            {
               query= query.Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId))
               .WhereIf(listCriteria.GlobalFilter != null && !isRef,
                   c => c.Nom.Trim().StartsWith(listCriteria.GlobalFilter.Trim())
                   || c.RaisonSociale.Trim().StartsWith(listCriteria.GlobalFilter.Trim()))
               .WhereIf(isRef, c => minRef <= c.Reference && c.Reference <= maxRef)
               .WhereIf(categorie != null, c => c.CategorieClient == categorie);

                clients = query.ToList();
            }
              
               

              
            
           

            if(type != null)
                clients =  this.checkClientsByType(query, type);

            if (listCriteria.SortField != null && listCriteria.SortField.Length != 0)
            {
                switch (listCriteria.SortField)
                {
                    case "reference":
                        if (listCriteria.SortOrder == "1") { clients = clients.OrderBy(c => c.Reference).ToList(); }
                        else { clients = clients.OrderByDescending(c => c.Reference).ToList(); }
                        break;
                    case "creationTime":
                        if (listCriteria.SortOrder == "1") { clients = clients.OrderBy(c => c.CreationTime).ToList(); }
                        else { clients = clients.OrderByDescending(c => c.CreationTime).ToList(); }
                        break;
                    case "nom":
                        if (listCriteria.SortOrder == "1") { clients = clients.OrderBy(c => c.DisplayName).ToList(); }
                        else { clients = clients.OrderByDescending(c =>c.DisplayName ).ToList(); }
                        break;
                    case "displayName":
                        if (listCriteria.SortOrder == "1") { clients = clients.OrderBy(c => c.DisplayName).ToList(); }
                        else { clients = clients.OrderByDescending(c =>c.DisplayName).ToList(); }
                        break;
                    default:
                        clients = clients.OrderByDescending(c => c.CreationTime ).ToList();
                        break;
                }

            }
            else
            {
                clients = clients.OrderByDescending(c => c.CreationTime).ToList();
            }

            var list = ObjectMapper.Map<List<ClientDto>>(clients.Skip(listCriteria.First).Take(listCriteria.Rows));
           
          /*    if (listCriteria.SortField != null && listCriteria.SortField.Length != 0)
            {
                switch (listCriteria.SortField)
                {
                    case "pendingInvoicesAmount":
                        if (listCriteria.SortOrder == "1") {
                            list = list.OrderBy(c => c.PendingInvoicesAmount ).ToList(); 
                        }else { 
                            list = list.OrderByDescending(c => c.PendingInvoicesAmount).ToList(); 
                         }
                        break;
                    case "overdueInvoicesAmount":
                        if (listCriteria.SortOrder == "1") {
                            list = list.OrderBy(c => c.OverdueInvoicesAmount ).ToList(); 
                        }else { 
                            list = list.OrderByDescending(c => c.OverdueInvoicesAmount).ToList(); 
                         }
                        break;
                    default:
                        list = list.OrderByDescending(c => c.CreationTime ).ToList();
                        break;
                }

            }
            else
            {
                clients = clients.OrderByDescending(c => c.CreationTime).ToList();
            }

            list = ObjectMapper.Map<List<ClientDto>>(list.Skip(listCriteria.First).Take(listCriteria.Rows)); */

           
            IQueryable<Facture> allValidesFactures;
            float calculationResult = 0;

             list.ForEach( (client) => {

                if (this._factureRepository.FirstOrDefault(f => (f.CreatorUserId == AbpSession.UserId ||
                    f.LastModifierUserId == AbpSession.UserId) && f.ClientId == client.Id) != null)
                        client.ClientType = "Client";

                else client.ClientType = "Prospect";
                
                allValidesFactures = this._factureRepository.GetAllIncluding(f => f.Client, f => f.FactureItems).Where(f => f.ClientId == client.Id
               && f.Statut == FactureStatutEnum.Valide);
                allValidesFactures.ToList()           
               .ForEach((item) => {  
                 calculationResult = (float)(item.FactureItems.Sum(di => (float?)di.TotalTtc) -
                 item.FactureItems.Sum(di => (float?)di.UnitPriceHT * di.Quantity) * item.Remise / 100);

                   if (DateTimeOffset.Compare(DateTimeOffset.Now, item.DateEmission.AddDays(item.EcheancePaiement)) <= 0)
                        client.PendingInvoicesAmount += calculationResult;
                   else
                       client.OverdueInvoicesAmount += calculationResult;
               });

             });

             if (listCriteria.SortField != null && listCriteria.SortField.Length != 0)
            {
                switch (listCriteria.SortField)
                {
                      case "reference":
                        if (listCriteria.SortOrder == "1") { clients = clients.OrderBy(c => c.Reference).ToList(); }
                        else { clients = clients.OrderByDescending(c => c.Reference).ToList(); }
                        break;
                    case "creationTime":
                        if (listCriteria.SortOrder == "1") { clients = clients.OrderBy(c => c.CreationTime).ToList(); }
                        else { clients = clients.OrderByDescending(c => c.CreationTime).ToList(); }
                        break;
                    case "nom":
                        if (listCriteria.SortOrder == "1") { clients = clients.OrderBy(c => c.DisplayName).ToList(); }
                        else { clients = clients.OrderByDescending(c =>c.DisplayName ).ToList(); }
                        break;
                    case "displayName":
                        if (listCriteria.SortOrder == "1") { clients = clients.OrderBy(c => c.DisplayName).ToList(); }
                        else { clients = clients.OrderByDescending(c =>c.DisplayName).ToList(); }
                        break;
                    case "pendingInvoicesAmount":
                        if (listCriteria.SortOrder == "1") {
                            list = list.OrderBy(c => c.PendingInvoicesAmount ).ToList(); 
                        }else { 
                            list = list.OrderByDescending(c => c.PendingInvoicesAmount).ToList(); 
                         }
                        break;
                    case "overdueInvoicesAmount":
                        if (listCriteria.SortOrder == "1") {
                            list = list.OrderBy(c => c.OverdueInvoicesAmount ).ToList(); 
                        }else { 
                            list = list.OrderByDescending(c => c.OverdueInvoicesAmount).ToList(); 
                         }
                        break;
                    default:
                        list = list.OrderByDescending(c => c.CreationTime ).ToList();
                        break;
                }

            }
            else
            {
                list = list.OrderByDescending(c => c.CreationTime).ToList();
            }

            
            list = ObjectMapper.Map<List<ClientDto>>(list.Skip(listCriteria.First).Take(listCriteria.Rows)); 
            var totalRecords = await query.LongCountAsync();
            var result = new ListResultWithTotalEntityItemsDto<ClientDto>(list, totalRecords);
            return result;
        }

        public async Task DeleteClient(long clientId)
        {
            await _clientRepository.DeleteAsync(clientId);
        }

       public async Task<ListResultDto<ClientForAutoCompleteDto>> GetClientForAutoComplete(string motCle)
        {
            var result = await _clientRepository.GetAll()
                .Where(c => c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId)
                .WhereIf(motCle != null, c => (c.CategorieClient.Equals("PRTC") && c.Nom.Contains(motCle)) || (c.CategorieClient.Equals("PRFS") && c.RaisonSociale.Contains(motCle)))
                .Select(c => new ClientForAutoCompleteDto() { Id = c.Id, DisplayName = c.DisplayName })
                .ToListAsync();

            return new ListResultDto<ClientForAutoCompleteDto>(result);
        }

         private static void CheckIfIsRefSearch(ListCriteriaDto clientCriterias, out bool isRef, out int minRef, out int maxRef)
        {
            isRef = false;
            minRef = 0;
            maxRef = 0;
            if (clientCriterias.GlobalFilter != null && clientCriterias.GlobalFilter.Trim().ToLower().StartsWith('c'))
            {
                string strRef = clientCriterias.GlobalFilter.Trim().Remove(0, 1);
                if (Int32.TryParse(strRef, out int n))
                {
                    isRef = true;
                    minRef = Int32.Parse(strRef + new String('0', 5 - strRef.Length));
                    maxRef = Int32.Parse(strRef + new String('9', 5 - strRef.Length));
                }
            }
        }

        public async Task<ClientDefaultsDto> GetClientDefaults(ClientDefaultsInputDto clientDefaultsInputDto)
        { 
           var client = await this.GetByIdClient(clientDefaultsInputDto.ClientId);
           
           ClientDefaultsDto clientDefaultsDto = new()
           { 
               PaymentPeriod = client.DelaiPaiement,
               PermanentDiscount = client.RemisePermanente,
               Currency = client.DeviseFacturation
           };

           return clientDefaultsDto;

        }
        private static void CheckIfIsFilterSearch(ListCriteriaDto clientCriterias, out string type, out string categorie) 
        {
           type = null;
           categorie = null;

            if(clientCriterias.ClientFilter != null)
            {
                type = clientCriterias.ClientFilter.Type;
                categorie = clientCriterias.ClientFilter.Category;
            }
        }
        private List<Client> checkClientsByType(IQueryable<Client> list, string clientType){

            List<Client> result = new();
            switch (clientType)
            {
                case "client":
                    foreach(var val in list)
                    {

                        if (this._factureRepository.FirstOrDefault(f => 
                            (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId) &&
                            f.ClientId == val.Id) != null)
                                result.Add(val);
                    }
                break;
                   
                case "prospect":
                    foreach (var val in list)
                    {
                        if (this._factureRepository.FirstOrDefault(f =>
                            (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId) &&
                            f.ClientId == val.Id) == null)
                                result.Add(val);
                    }
                break;

            }

            return result; 
        }
    // }
}
}
