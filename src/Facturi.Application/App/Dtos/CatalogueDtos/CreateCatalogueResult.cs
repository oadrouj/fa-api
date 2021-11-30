using System;

namespace Facturi.Application.App.Dtos.CatalogueDtos
{
    public class CreateCatalogueResult
    {
        public long Id { get; set;}
        public int Reference { get; set;}
        public DateTime AddedDate { get; set;}
    }
}