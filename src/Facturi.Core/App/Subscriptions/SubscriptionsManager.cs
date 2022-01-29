using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Subscriptions
{
    public class SubscriptionsManager
    {
        private readonly IRepository<Subscription, long> _subscriptionRepo;
        private readonly IRepository<SubscriptionFeature, long> _subscriptionFeatureRepo;
        private readonly IRepository<SubscriptionPackage, long> _subscriptionPackageRepo;
        public IAbpSession AbpSession { get; set; }
        public SubscriptionsManager(
            IRepository<Subscription, long> subscriptionRepo,
            IRepository<SubscriptionFeature, long> subscriptionFeatureRepo,
            IRepository<SubscriptionPackage, long> subscriptionPackageRepo
        )
        {
            _subscriptionRepo = subscriptionRepo;
            _subscriptionFeatureRepo = subscriptionFeatureRepo;
            _subscriptionPackageRepo = subscriptionPackageRepo;
            AbpSession = NullAbpSession.Instance;
        }

        public async Task<bool> createSubscriptionAsync(Subscription subscription)
        {
           var result = await _subscriptionRepo.InsertAsync(subscription);
            if (result != null)
                return true;
            else
                throw new Exception("could not create subscription");
        }

        public async Task<bool> createSubscriptionFreatureAsync(SubscriptionFeature subscriptionFeature)
        {
            var result = await _subscriptionFeatureRepo.InsertAsync(subscriptionFeature);
            if (result != null)
                return true;
            else
                throw new Exception("could not create subscription feature");
        }

        public async Task<bool> IsEnabled(SubscriptionFeatureTypeEnum subscriptionFeatureType)
        {
            var subscription = await _subscriptionRepo.FirstOrDefaultAsync(e => e.UserId == AbpSession.UserId);
           
            if(subscription.SubscriptionPackage.Name == "Free")
            {
               var subscriptionFeature = await _subscriptionFeatureRepo
                    .FirstOrDefaultAsync(e => e.SubscriptionId == subscription.Id &&
                     e.Type == subscriptionFeatureType);

                if (subscriptionFeature == null)
                {
                    var newsubscriptionFeature = new SubscriptionFeature
                    {
                        Type = subscriptionFeatureType,
                        SubscriptionId = subscription.Id,
                        Value = 1
                    };
                    await _subscriptionFeatureRepo.InsertAsync(newsubscriptionFeature);
                    return true;
                }
                else if (subscriptionFeature.Value > 5)
                {
                        throw new Exception("You cannot use this service anymore");
                }
                else
                {
                        subscriptionFeature.Value = subscriptionFeature.Value++;
                        await _subscriptionFeatureRepo.UpdateAsync(subscriptionFeature);
                        return true;
                }

            }
           
            else if (subscription.SubscriptionPackage.Name == "Paid")
            {
                return true;

            }
            else 
            {
                return false;

            }
        }
        
        
        public async Task<bool> createSubscriptionPackageAsync(SubscriptionPackage subscriptionPackage)
        {
            var result = await _subscriptionPackageRepo.InsertAsync(subscriptionPackage);
            if (result != null)
                return true;
            else
                throw new Exception("could not create subscription package");
        }
    }
}
