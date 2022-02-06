using Abp.Domain.Entities;

namespace Facturi.App
{
    public class ClientForAutoCompleteDto : Entity<long>
    {
        public string DisplayName { get; set; }
    }
}