using Abp.Domain.Entities.Auditing;

namespace Facturi.Core.App
{
    
    public class Country: AuditedEntity<long>
    {
        public string PaysName { get; set; }
        public string PaysCode { get; set; }
    }
}

