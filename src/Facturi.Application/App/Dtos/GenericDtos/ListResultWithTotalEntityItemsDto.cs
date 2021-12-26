using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.GenericDtos
{
    public class ListResultWithTotalEntityItemsDto<T> : IListResult<T>
    {
        public ListResultWithTotalEntityItemsDto(IReadOnlyList<T> items, long totalEntityItems)
        {
            this.Items = items;
            this.TotalEntityItems = totalEntityItems;
        }
        public IReadOnlyList<T> Items { get; set; }
        public long TotalEntityItems { get; set; }
    }
}
