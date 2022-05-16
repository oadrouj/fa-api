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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Facturi.App.Dtos.EstimationDtos;

namespace Facturi.App
{
    public class DevisAppService : ApplicationService
    {
        private readonly IRepository<Devis, long> _devisRepository;
        private readonly IRepository<DevisItem, long> _devisItemRepository;
        private readonly IReportGeneratorAppService _reportGeneratorAppService;
        private readonly IRepository<Client, long> _clientRepository;
        private readonly IInfosEntrepriseAppService _infosEntrepriseAppService;

        public DevisAppService(
            IRepository<Devis, long> DevisRepository,
            IRepository<DevisItem, long> devisItemRepository, 
            IReportGeneratorAppService reportGeneratorAppService,
            IRepository<Client, long> clientRepository,
            IInfosEntrepriseAppService infosEntrepriseAppService
        )
        {
            _devisRepository = DevisRepository ?? throw new ArgumentNullException(nameof(DevisRepository));
            _devisItemRepository = devisItemRepository ?? throw new ArgumentNullException(nameof(devisItemRepository));
            _reportGeneratorAppService = reportGeneratorAppService ?? throw new ArgumentNullException(nameof(reportGeneratorAppService));
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _infosEntrepriseAppService = infosEntrepriseAppService ?? throw new ArgumentNullException(nameof(infosEntrepriseAppService));


        }

        public async Task<long> CreateDevis(CreateDevisInput input)
        {
            try
            {
                //Gerer reference
                var devis = ObjectMapper.Map<Devis>(input);
                var newDevisId = await _devisRepository.InsertAndGetIdAsync(devis);

                return newDevisId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> UpdateDevis(UpdateDevisInput input)
        {
            try
            {
                var devisItemsToDelete = _devisItemRepository.GetAll().Where(di => di.DevisId == input.Id).Select(di => di.Id).ToArray();
                if (devisItemsToDelete != null && devisItemsToDelete.Length > 0)
                {
                    foreach (long devisItemId in devisItemsToDelete)
                    {
                        _devisItemRepository.Delete(devisItemId);
                    }
                }

                var devis = ObjectMapper.Map<Devis>(input);
                await _devisRepository.InsertOrUpdateAsync(devis);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<DevisDto> GetByIdDevis(long id)
        {
            var devis = await _devisRepository.GetAllIncluding(d => d.DevisItems, d => d.Client)
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId) && d.Id == id)
                .ToListAsync();
            var result = ObjectMapper.Map<DevisDto>(devis.First());
            return result;
        }

        public async Task<bool> DeleteDevis(long DevisId)
        {
            try
            {
                var devisItemsToDelete = _devisItemRepository.GetAll().Where(di => di.DevisId == DevisId).Select(di => di.Id).ToArray();
                if (devisItemsToDelete != null && devisItemsToDelete.Length > 0)
                {
                    foreach (long devisItemId in devisItemsToDelete)
                    {
                        await _devisItemRepository.DeleteAsync(devisItemId);
                    }
                }
                await _devisRepository.DeleteAsync(DevisId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetLastReference()
        {
             var devis =  (await _devisRepository.GetAllListAsync())
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId)
                    && (Regex.IsMatch(d.Reference, @"^D[0-9]{5}"))).OrderByDescending(d => d.Reference).ToList();

            if (devis != null && devis.Any() && Int32.TryParse(devis.First().Reference.Substring(1), out int reference))
                return reference;
            else
                return 0;
        }
        public async Task<EstimationInitiationDto> GetLastReferenceWithIntroMessageAndFooter()
        {
            var devis = (await _devisRepository.GetAllListAsync())
               .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId)
                   && (Regex.IsMatch(d.Reference, @"^D[0-9]{5}"))).OrderByDescending(d => d.Reference).ToList();
            var defaultAnnotations = await _infosEntrepriseAppService.GetDefaultAnnotations();
            var estimateInitiation = new EstimationInitiationDto();

            if (defaultAnnotations == null)
            {
                estimateInitiation.EstimateIntroMessage = null;
                estimateInitiation.EstimateFooter = null;
            }
            else
            {
                estimateInitiation.EstimateIntroMessage = defaultAnnotations.EstimateIntroMessage;
                estimateInitiation.EstimateFooter = defaultAnnotations.EstimateFooter;
            }

            if (devis != null && devis.Any() && Int32.TryParse(devis.First().Reference.Substring(1), out int reference))
                estimateInitiation.LastReference = reference;
            else
                estimateInitiation.LastReference = 0;

            return estimateInitiation;
        }

        public async Task<bool> ChangeDevisStatut(long DevisId, DevisStatutEnum statut)
        {
            try
            {
                var devis = (await _devisRepository.GetAllIncluding(d => d.DevisItems, d => d.Client)
                                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId) && d.Id == DevisId)
                                .ToListAsync()).First();
                devis.Statut = statut;
                await _devisRepository.UpdateAsync(devis);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ListResultDto<DevisDto>> GetAllDevis(CriteriasDto devisCriterias)
        {
            CheckIfIsRefSearch(devisCriterias, out bool isRef, out int minRef, out int maxRef);
            
            CheckIfIsFilterSearch(devisCriterias, out long client, out DateTime[] dateEmission, out int echeancePaiement,
            out float montantTtc, out DevisStatutEnum statut);

            var DevisList = new List<Devis>();
            var query = _devisRepository.GetAllIncluding(f => f.DevisItems, f => f.Client)
            .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
            //.WhereIf(devisCriterias.GlobalFilter != null & !isRef,
            //    f => f.Client.Nom.Trim().Contains(devisCriterias.GlobalFilter.Trim())
            //    || f.Client.RaisonSociale.Trim().Contains(devisCriterias.GlobalFilter.Trim()))
            .WhereIf(devisCriterias.GlobalFilter != null, f => f.Reference.StartsWith(devisCriterias.GlobalFilter))
            .WhereIf(client != 0, f => f.ClientId == client)
            .WhereIf(dateEmission != null, f => f.DateEmission >= dateEmission[0] && f.DateEmission <= dateEmission[1])
            .WhereIf(echeancePaiement != 0, f => f.EcheancePaiement == echeancePaiement)
            .WhereIf(montantTtc != -1, f => f.DevisItems.Sum(item => item.TotalTtc) == montantTtc)
            .WhereIf(statut != DevisStatutEnum.Undefined && statut != DevisStatutEnum.Expire, f => f.Statut == statut)
            .WhereIf(statut == DevisStatutEnum.Valide, e => e.Statut == DevisStatutEnum.Valide &&
                DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) > 0)
            .WhereIf(statut == DevisStatutEnum.Expire, e => e.Statut == DevisStatutEnum.Valide &&
                DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) < 0);


            if (devisCriterias.SortField != null && devisCriterias.SortField.Length != 0)
            {
                switch (devisCriterias.SortField)
                {
                    case "reference":
                        if (devisCriterias.SortOrder.Equals("1")) { DevisList = await query.OrderBy(d => d.Reference).Skip(devisCriterias.First).Take(devisCriterias.Rows).ToListAsync(); }
                        else if (devisCriterias.SortOrder.Equals("-1")) { DevisList = await query.OrderByDescending(d => d.Reference).Skip(devisCriterias.First).Take(devisCriterias.Rows).ToListAsync(); }
                        break;
                    case "client":
                        if (devisCriterias.SortOrder.Equals("1")) { DevisList = await query.OrderBy(d => d.Client.RaisonSociale + d.Client.Nom).Skip(devisCriterias.First).Take(devisCriterias.Rows).ToListAsync(); }
                        else if (devisCriterias.SortOrder.Equals("-1")) { DevisList = await query.OrderByDescending(d => d.Client.Nom + d.Client.RaisonSociale).Skip(devisCriterias.First).Take(devisCriterias.Rows).ToListAsync(); }
                        break;
                    case "dateEmission":
                        if (devisCriterias.SortOrder.Equals("1")) { DevisList = await query.OrderBy(d => d.DateEmission).Skip(devisCriterias.First).Take(devisCriterias.Rows).ToListAsync(); }
                        else if (devisCriterias.SortOrder.Equals("-1")) { DevisList = await query.OrderByDescending(d => d.DateEmission).Skip(devisCriterias.First).Take(devisCriterias.Rows).ToListAsync(); }
                        break;
                    default:
                        DevisList = await query.OrderBy(d => d.EcheancePaiement).Skip(devisCriterias.First).Take(devisCriterias.Rows).ToListAsync();
                        break;
                }

            }
            else
            {
                DevisList = await query.OrderBy(d => d.EcheancePaiement).Skip(devisCriterias.First).Take(devisCriterias.Rows).ToListAsync();
            }
          
            var result = new ListResultDto<DevisDto>(ObjectMapper.Map<List<DevisDto>>(DevisList));
            return result;
        }

        public async Task<int> GetAllDevisTotalRecords(CriteriasDto devisCriterias)
        {
            CheckIfIsRefSearch(devisCriterias, out bool isRef, out int minRef, out int maxRef);
            CheckIfIsFilterSearch(devisCriterias, out long client, out DateTime[] dateEmission, out int echeancePaiement,
            out float montantTtc, out DevisStatutEnum statut);

            var DevisList = new List<Devis>();
            var query = _devisRepository.GetAllIncluding(f => f.DevisItems, f => f.Client)
            .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
            //.WhereIf(devisCriterias.GlobalFilter != null & !isRef,
            //    f => f.Client.Nom.Trim().Contains(devisCriterias.GlobalFilter.Trim())
            //    || f.Client.RaisonSociale.Trim().Contains(devisCriterias.GlobalFilter.Trim()))
            .WhereIf(devisCriterias.GlobalFilter != null, f => f.Reference.StartsWith(devisCriterias.GlobalFilter))
            .WhereIf(client != 0, f => f.ClientId == client)
            .WhereIf(dateEmission != null, f => f.DateEmission >= dateEmission[0] && f.DateEmission <= dateEmission[1])
            .WhereIf(echeancePaiement != 0, f => f.EcheancePaiement == echeancePaiement)
            .WhereIf(montantTtc != -1, f => f.DevisItems.Sum(item => item.TotalTtc) == montantTtc)
            .WhereIf(statut != DevisStatutEnum.Undefined && statut != DevisStatutEnum.Expire, f => f.Statut == statut)
            .WhereIf(statut == DevisStatutEnum.Valide, e => e.Statut == DevisStatutEnum.Valide &&
                DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) > 0)
            .WhereIf(statut == DevisStatutEnum.Expire, e => e.Statut == DevisStatutEnum.Valide &&
                DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) < 0);

            return await query.CountAsync();
        }

        public async Task<float> GetAllDevisMontantTotal(CriteriasDto devisCriterias)
        {
          CheckIfIsRefSearch(devisCriterias, out bool isRef, out int minRef, out int maxRef);
            CheckIfIsFilterSearch(devisCriterias, out long client, out DateTime[] dateEmission, out int echeancePaiement,
            out float montantTtc, out DevisStatutEnum statut);

            var DevisList = new List<Devis>();
            var query = _devisRepository.GetAllIncluding(f => f.DevisItems, f => f.Client)
            .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
            //.WhereIf(devisCriterias.GlobalFilter != null & !isRef,
            //    f => f.Client.Nom.Trim().Contains(devisCriterias.GlobalFilter.Trim())
            //    || f.Client.RaisonSociale.Trim().Contains(devisCriterias.GlobalFilter.Trim()))
            .WhereIf(devisCriterias.GlobalFilter != null, f => f.Reference.StartsWith(devisCriterias.GlobalFilter))
            .WhereIf(client != 0, f => f.ClientId == client)
            .WhereIf(dateEmission != null, f => f.DateEmission >= dateEmission[0] && f.DateEmission <= dateEmission[1])
            .WhereIf(echeancePaiement != 0, f => f.EcheancePaiement == echeancePaiement)
            .WhereIf(montantTtc != -1, f => f.DevisItems.Sum(item => item.TotalTtc) == montantTtc)
            .WhereIf(statut != DevisStatutEnum.Undefined && statut != DevisStatutEnum.Expire, f => f.Statut == statut)
            .WhereIf(statut == DevisStatutEnum.Valide, e => e.Statut == DevisStatutEnum.Valide &&
                DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) > 0)
            .WhereIf(statut == DevisStatutEnum.Expire, e => e.Statut == DevisStatutEnum.Valide &&
                DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) < 0);

            var result = 0.0f;
            foreach (var item in query)
            {
                result += (float)(item.DevisItems.Sum(di => (float?)di.TotalTtc) -
                 item.DevisItems.Sum(di => (float?)di.UnitPriceHT * di.Quantity) * item.Remise /100);

            }
            return result;
        }

        private static void CheckIfIsRefSearch(CriteriasDto devisCriterias, out bool isRef, out int minRef, out int maxRef)
        {
            isRef = false;
            minRef = 0;
            maxRef = 0;
            if (devisCriterias.GlobalFilter != null && devisCriterias.GlobalFilter.Trim().ToLower().StartsWith('d'))
            {
                string strRef = devisCriterias.GlobalFilter.Trim().Remove(0, 1);
                if (Int32.TryParse(strRef, out int n))
                {
                    isRef = true;
                    minRef = Int32.Parse(strRef + new String('0', 5 - strRef.Length));
                    maxRef = Int32.Parse(strRef + new String('9', 5 - strRef.Length));
                }
            }
        }

        public async Task<byte[]> GetByIdDevisReport(long id)
        {
            var facture = await _devisRepository.GetAllIncluding(f => f.Client, f => f.DevisItems)
                .Where(f => f.Id == id)
                .ToListAsync();
            return _reportGeneratorAppService.GetByteDataDevis(ObjectMapper.Map<DevisDto>(facture.First()));
        }

        public async Task<byte[]> GetByteDataDevisReport(CreateDevisInput input)
        {
            var devis = ObjectMapper.Map<Devis>(input);
            devis.Client = (await _clientRepository.GetAll()
                .Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId) && c.Id == input.ClientId).ToListAsync()).First();
            return _reportGeneratorAppService.GetByteDataDevis(ObjectMapper.Map<DevisDto>(devis));
    }
        private static void CheckIfIsFilterSearch(CriteriasDto devisCriterias, out long client, out DateTime[] dateEmission, out int echeancePaiement,
            out float montantTtc, out DevisStatutEnum statut) 
        {
            client = 0;
            dateEmission = null;
            echeancePaiement = 0;
            montantTtc = -1;
            statut = DevisStatutEnum.Undefined;

            if(devisCriterias.Filtres != null)
            {
                client = devisCriterias.Filtres.Client;
                dateEmission = devisCriterias.Filtres.DateEmission;
                echeancePaiement = devisCriterias.Filtres.EcheancePaiement;
                montantTtc = devisCriterias.Filtres.MontantTtc;
                statut = (DevisStatutEnum)devisCriterias.Filtres.Statut;
            }
        }
         public async Task<bool> CheckIfReferenceIsExist(string reference) {
            var query = await this._devisRepository.GetAll()
                .FirstOrDefaultAsync(item => 
                    (item.CreatorUserId == AbpSession.UserId || item.LastModifierUserId == AbpSession.UserId) &&
                    item.Reference == reference);
            if(query != null)
                return true;
            else 
                return false;
        }

    }
   

}
