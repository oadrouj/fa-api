using Abp.Domain.Entities.Auditing;
using Facturi.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public class UserEmailConfirmation : AuditedEntity<long>
    {
        public string ConfirmationToken { get; set; }

        [ForeignKey("UserId")]
        public long UserId { get; set; }
        public User User { get; set; }
    }
}
