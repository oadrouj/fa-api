using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Facturi.Application.App.Dtos;
using Facturi.Core.App;
using Microsoft.EntityFrameworkCore;

namespace Facturi.Application.App
{
    public class CountryServiceApp: ApplicationService, ICountryServiceApp
    {
        private readonly IRepository<Country, long> _countryRepository;
        public CountryServiceApp(
            IRepository<Country, long> countryRepository
        )
        {
            _countryRepository = countryRepository;
        }
        public async Task<ListResultDto<CountryDto>> GetAllCountries()
        {
            var list =await  _countryRepository.GetAll().OrderBy(x => x.PaysName)
                .Select(x => ObjectMapper.Map<CountryDto>(x))
                .ToListAsync();

            return new ListResultDto<CountryDto>(list);
        }
        public async Task<List<Country>> GetAllDBCountries()
        {
            return await _countryRepository.GetAllListAsync(); 
        }
        public async Task<bool> InsertManyCountries(List<Country> countries)
        {
            try
            {
                foreach (var item in countries)
                {
                    await _countryRepository.InsertAsync(item);
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        
        }

    }
}