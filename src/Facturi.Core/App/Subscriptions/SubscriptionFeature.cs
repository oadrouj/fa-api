using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Subscriptions
{
    public class SubscriptionFeature: AuditedEntity<long>
    {
        public SubscriptionFeatureTypeEnum Type { get; set; }
        public int Value { get; set; }
        public long SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
    }
}
