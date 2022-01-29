using Abp.Domain.Entities.Auditing;
using Facturi.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Subscriptions
{
    public class Subscription: AuditedEntity<long>
    {
        public int NumberOfMonths { get; set; } = 1;
        public bool IsCancelled { get; set; } = false;
        public long UserId { get; set; }
        public User User { get; set; }
        public long SubscriptionPackageId { get; set; }
        public SubscriptionPackage SubscriptionPackage { get; set; }
        public ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
    }

}

