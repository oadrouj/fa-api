using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Facturi.App.Dtos;
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

        public ClientAppService(IRepository<Client, long> clientRepository)
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        public async Task<ClientDto> CreateClient(ClientDto input)
        {
            input.Reference = 1;
            var maxRefClient = _clientRepository.GetAll().Where(c => c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId).OrderByDescending(c => c.Reference).ToList();
            if (maxRefClient != null && maxRefClient.Any())
            {
                input.Reference = maxRefClient.First().Reference + 1;
            }
            var client = ObjectMapper.Map<Client>(ObjectMapper.Map<CreateClientInput>(input));
            var result = await _clientRepository.InsertAsync(client);
            return ObjectMapper.Map<ClientDto>(result);
        }

        public async Task<ClientDto> UpdateClient(ClientDto input)
        {
            var client = ObjectMapper.Map<Client>(ObjectMapper.Map<UpdateClientInput>(input));
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

        public async Task<ListResultDto<ClientDto>> GetAllClients(ListCriteriaDto listCriteria)
        {
            bool isRef = false;
            int minRef = 0;
            int maxRef = 0;

            if (listCriteria.ChampsRecherche != null && listCriteria.ChampsRecherche.Trim().ToLower().StartsWith('c'))
            {
                string strRef = listCriteria.ChampsRecherche.Trim().Remove(0, 1);
                if (Int32.TryParse(strRef, out int n))
                {
                    isRef = true;
                    minRef = Int32.Parse(strRef + new String('0', 5 - strRef.Length));
                    maxRef = Int32.Parse(strRef + new String('9', 5 - strRef.Length));
                }
            }
            var clients = new List<Client>();
            var query = _clientRepository.GetAll()
                .Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId))
                .WhereIf(listCriteria.ChampsRecherche != null & !isRef,
                    c => c.Nom.Trim().Contains(listCriteria.ChampsRecherche.Trim())
                    || c.RaisonSociale.Trim().Contains(listCriteria.ChampsRecherche.Trim()))
                .WhereIf(isRef, c => minRef <= c.Reference && c.Reference <= maxRef);
                // .WhereIf(!listCriteria.ClientCategory.Equals("0"), c => c.CategorieClient.Equals(listCriteria.ClientCategory));

            if (listCriteria.SortField != null && listCriteria.SortField.Length != 0)
            {
                switch (listCriteria.SortField)
                {
                    case "reference":
                        if (listCriteria.SortOrder == 1) { clients = await query.OrderBy(c => c.Reference).ToListAsync(); }
                        else { clients = await query.OrderByDescending(c => c.Reference).ToListAsync(); }
                        break;
                    case "creationTime":
                        if (listCriteria.SortOrder == 1) { clients = await query.OrderBy(c => c.CreationTime).ToListAsync(); }
                        else { clients = await query.OrderByDescending(c => c.CreationTime).ToListAsync(); }
                        break;
                    case "nom":
                        if (listCriteria.SortOrder == 1) { clients = await query.OrderBy(c => c.RaisonSociale + c.Nom).ToListAsync(); }
                        else { clients = await query.OrderByDescending(c => c.Nom + c.RaisonSociale).ToListAsync(); }
                        break;
                    default:
                        clients = await query.OrderByDescending(c => c.LastModificationTime != null ? c.LastModificationTime : c.CreationTime).ToListAsync();
                        break;
                }

            }
            else
            {
                clients = await query.OrderByDescending(c => c.LastModificationTime != null ? c.LastModificationTime : c.CreationTime).ToListAsync();
            }
            var result = new ListResultDto<ClientDto>(ObjectMapper.Map<List<ClientDto>>(clients));
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
                .Where(c => (c.CategorieClient.Equals("PRTC") && c.Nom.Contains(motCle)) || (c.CategorieClient.Equals("PRFS") && c.RaisonSociale.Contains(motCle)))
                .Select(c => new ClientForAutoCompleteDto() { Id = c.Id, Nom = c.CategorieClient.Equals("PRTC") ? c.Nom : c.RaisonSociale })
                .ToListAsync();

            return new ListResultDto<ClientForAutoCompleteDto>(result);
        }
    }
}
