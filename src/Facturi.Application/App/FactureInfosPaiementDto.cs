using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace Facturi.App
{
    [AutoMap(typeof(FactureInfosPaiement))]
    public class FactureInfosPaiementDto : EntityDto<long>
    {
        public DateTime DatePaiement { get; set; } = new DateTime();

        public float MontantPaye { get; set; }

        public ModePaiementEnum ModePaiement { get; set; }

        public long FactureId { get; set; }
    }
}