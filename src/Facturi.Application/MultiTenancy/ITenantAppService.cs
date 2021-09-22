using Abp.Application.Services;
using Facturi.MultiTenancy.Dto;

namespace Facturi.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

