using System.ComponentModel.DataAnnotations;

namespace Facturi.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}