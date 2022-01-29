using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Subscriptions
{
    public class SubscriptionPackage: AuditedEntity<long>
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
