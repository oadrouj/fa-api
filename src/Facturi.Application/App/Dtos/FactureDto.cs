using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Facturi.App.Dtos;
using System;
using System.Collections.Generic;

namespace Facturi.App
{
    [AutoMap(typeof(Facture))]
    public class FactureDto : AuditedEntityDto<long>
    {
        public string Reference { get; set; }
        
        public DateTime DateEmission { get; set; } = new DateTime();
        public int EcheancePaiement { get; set; }
        public string MessageIntroduction { get; set; }
        public string PiedDePage { get; set; }
        public float Remise { get; set; }

        public FactureStatutEnum Statut { get; set; }

        public List<FactureItemDto> FactureItems { get; set; }

        public long ClientId { get; set; }
        public ClientDto Client { get; set; }
        public string Currency { get; set; }

    }
}