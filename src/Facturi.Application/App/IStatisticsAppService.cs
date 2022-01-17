using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Facturi.App.Dtos.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IStatisticsAppService: IApplicationService
    {
        Task<ListResultDto<ActivityLogDto>> GetActivityLog();
        Task<bool> CreateOrUpdateMonthTargetAmount(float amount);
        Task<MonthTargetInfosDto> GetMonthTargetInfos();
        Task<PeriodicTrackingDto> GetPeriodicTrackingInfos(PeriodicTrackingInputDto periodicTrackingInputDto);
        Task<TotalStatisticsDto> GetTotalStatistics();
    }
}
