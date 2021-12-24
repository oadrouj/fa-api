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
            var query =  _countryRepository.GetAll().OrderBy(x => x.PaysName);
            
            // if(listCriteria.First != 0 && listCriteria.Rows != 0)
            //     query = Queryable.Skip(query, listCriteria.First).Take(listCriteria.Rows);
            
            var list =  await query.Select(x => ObjectMapper.Map<CountryDto>(x))
                .ToListAsync();

            var result = new ListResultDto<CountryDto>(list);
            return result;
        }
        public async Task<List<Country>> GetAllDBCountries()
        {
            var query = await _countryRepository.GetAllListAsync();
            return query;
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