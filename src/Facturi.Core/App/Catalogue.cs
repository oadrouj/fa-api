using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturi.Core.App
{
    public class Catalogue: AuditedEntity<long>
    {
        
        public int Reference { get; set; }
        
        public string CatalogueType { get; set; }
        public DateTime AddedDate { get; set; }
        public string Designation { get; set; }
        public string Description { get; set; }
        public float HtPrice { get; set; }
        public string Unity { get; set; }
        public int Tva { get; set; }
        public float MinimalQuantity { get; set; }

    }
}