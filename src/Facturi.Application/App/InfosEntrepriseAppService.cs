using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Facturi.App.Dtos;
using Facturi.App.Dtos.ProfileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public class InfosEntrepriseAppService : ApplicationService, IInfosEntrepriseAppService
    {
        private readonly IRepository<InfosEntreprise, long> _infosEntrepriseRepository;
        public InfosEntrepriseAppService(
            IRepository<InfosEntreprise, long>  infosEntrepriseRepository
        )
        {
            _infosEntrepriseRepository = infosEntrepriseRepository ?? throw new ArgumentNullException(nameof(infosEntrepriseRepository));
        }

        public async Task<bool> CreateInfosEntreprise(CreateInfosEntrepriseInput input)
        {
            try
            {
                var infosEntreprise = ObjectMapper.Map<InfosEntreprise>(input);
                infosEntreprise.UserId = AbpSession.UserId.GetValueOrDefault();
                await _infosEntrepriseRepository.InsertAsync(infosEntreprise);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
           
        }

        public async Task<InfosEntrepriseDto> GetByIdInfosEntreprise(long id)
        {
            var infosEntreprise = await _infosEntrepriseRepository.GetAsync(id);

            return ObjectMapper.Map<InfosEntrepriseDto>(infosEntreprise);
        }

        public async Task<GeneralInfosDto> GetGeneralInfos()
        {
            try
            {
                var infosEnteprise = await _infosEntrepriseRepository.FirstOrDefaultAsync(x => x.UserId == AbpSession.UserId);
                return ObjectMapper.Map<GeneralInfosDto>(infosEnteprise);
            }
            catch (Exception e)
            {
                return null;
            }
          
        }
        public async Task<ListResultDto<InfosEntrepriseDto>> GetAllInfosEntreprise()
        {
            var list = await _infosEntrepriseRepository.GetAllListAsync();
            return new ListResultDto<InfosEntrepriseDto>(ObjectMapper.Map<List<InfosEntrepriseDto>>(list));
        }

        public async Task<bool> UpdateGeneralInfos(GeneralInfosDto generalInfosDto)
        {
            try
            {
                var entity = await _infosEntrepriseRepository.FirstOrDefaultAsync(x => x.Id == generalInfosDto.Id);
                entity.RaisonSociale = generalInfosDto.RaisonSociale;
                entity.SecteurActivite = generalInfosDto.SecteurActivite;
                entity.HasLogo = generalInfosDto.HasLogo;
                await _infosEntrepriseRepository.UpdateAsync(entity);
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        public async Task<ContactInfosDto> GetContactInfos()
        {
            try
            {
                var infosEnteprise = await _infosEntrepriseRepository.FirstOrDefaultAsync(x => x.UserId == AbpSession.UserId);
                return ObjectMapper.Map<ContactInfosDto>(infosEnteprise);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> UpdateContactInfos(ContactInfosDto contactInfosDto)
        {
            try
            {
                var entity = await _infosEntrepriseRepository.FirstOrDefaultAsync(x => x.Id == contactInfosDto.Id);
                entity.Adresse = contactInfosDto.Adresse;
                entity.Pays = contactInfosDto.Pays;
                entity.Ville = contactInfosDto.Ville;
                entity.CodePostal = contactInfosDto.CodePostal;
                entity.Telephone = contactInfosDto.Telephone;
                entity.AdresseMail = contactInfosDto.AdresseMail;

                await _infosEntrepriseRepository.UpdateAsync(entity);
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        public async Task<DefaultAnnotationsDto> GetDefaultAnnotations()
        {
            try
            {
                var infosEnteprise = await _infosEntrepriseRepository.FirstOrDefaultAsync(x => x.UserId == AbpSession.UserId);
                return ObjectMapper.Map<DefaultAnnotationsDto>(infosEnteprise);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> UpdateDefaultAnnotations(DefaultAnnotationsDto defaultAnnotationsDto)
        {
            try
            {
                var entity = await _infosEntrepriseRepository.FirstOrDefaultAsync(x => x.Id == defaultAnnotationsDto.Id);
                entity.EstimateIntroMessage = defaultAnnotationsDto.EstimateIntroMessage;
                entity.EstimateFooter = defaultAnnotationsDto.EstimateFooter;
                entity.InvoiceIntroMessage = defaultAnnotationsDto.InvoiceIntroMessage;
                entity.InvoiceFooter = defaultAnnotationsDto.InvoiceFooter;
                await _infosEntrepriseRepository.UpdateAsync(entity);
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }
    }
}
