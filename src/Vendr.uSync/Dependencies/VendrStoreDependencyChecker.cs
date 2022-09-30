using System;
using System.Collections.Generic;
using System.Linq;
using Vendr.Core.Api;
using Vendr.Core.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using uSync.Core.Dependency;

namespace Vendr.uSync.Dependencies
{
    public class VendrStoreDependencyChecker : ISyncDependencyChecker<StoreReadOnly>
    {
        private readonly IVendrApi _vendrApi;

        public VendrStoreDependencyChecker(IVendrApi vendrApi)
        {
            _vendrApi = vendrApi;
        }

        public UmbracoObjectTypes ObjectType => UmbracoObjectTypes.Unknown;

        public IEnumerable<uSyncDependency> GetDependencies(StoreReadOnly item, DependencyFlags flags)
        {
            if (item == null) return Enumerable.Empty<uSyncDependency>();

            var items = new List<uSyncDependency>();


            items.Add(new uSyncDependency
            {
                Name = item.Name,
                Order = VendrConstants.Priorites.Stores,
                Udi = Udi.Create(VendrConstants.UdiEntityType.Store, item.Id),
                Flags = DependencyFlags.None
            });

            // a store has a lot of dependencies,
            items.AddRange(GetOrderStatuses(item.Id));
            items.AddRange(GetCurrencies(item.Id));
            items.AddRange(GetShippingMethods(item.Id));
            items.AddRange(GetCountries(item.Id));
            items.AddRange(GetRegions(item.Id));
            items.AddRange(GetPaymentMethods(item.Id));
            items.AddRange(GetTaxClasses(item.Id));
            items.AddRange(GetEmailTemplates(item.Id));
            items.AddRange(GetExportTemplates(item.Id));
            items.AddRange(GetPrintTemplates(item.Id));

            return items;
        }

        public IEnumerable<uSyncDependency> GetOrderStatuses(Guid storeId)
            => _vendrApi.GetOrderStatuses(storeId)
                    .Select(x => new uSyncDependency
                    {
                        Name = x.Name,
                        Order = VendrConstants.Priorites.OrderStatus,
                        Udi = Udi.Create(VendrConstants.UdiEntityType.OrderStatus, x.Id)
                    });

        private IEnumerable<uSyncDependency> GetCurrencies(Guid storeId)
            => _vendrApi.GetCurrencies(storeId)
                    .Select(x => new uSyncDependency
                    {
                        Name = x.Name,
                        Order = VendrConstants.Priorites.Currency,
                        Udi = Udi.Create(VendrConstants.UdiEntityType.Currency, x.Id)
                    });

        private IEnumerable<uSyncDependency> GetShippingMethods(Guid storeId)
            => _vendrApi.GetShippingMethods(storeId)
                .Select(x => new uSyncDependency
                {
                    Name = x.Name,
                    Order = VendrConstants.Priorites.ShippingMethod,
                    Udi = Udi.Create(VendrConstants.UdiEntityType.ShippingMethod)
                });

        private IEnumerable<uSyncDependency> GetCountries(Guid storeId)
            => _vendrApi.GetCountries(storeId)
                .Select(x => new uSyncDependency
                {
                    Name = x.Name,
                    Order = VendrConstants.Priorites.Country,
                    Udi = Udi.Create(VendrConstants.UdiEntityType.Country, x.Id)
                });

        private IEnumerable<uSyncDependency> GetRegions(Guid storeId)
            => _vendrApi.GetRegions(storeId)
                .Select(x => new uSyncDependency
                {
                    Name = x.Name,
                    Order = VendrConstants.Priorites.Region,
                    Udi = Udi.Create(VendrConstants.UdiEntityType.Region, x.Id)
                });


        private IEnumerable<uSyncDependency> GetPaymentMethods(Guid storeId)
            => _vendrApi.GetPaymentMethods(storeId)
                .Select(x => new uSyncDependency
                {
                    Name = x.Name,
                    Order = VendrConstants.Priorites.PaymentMethod,
                    Udi = Udi.Create(VendrConstants.UdiEntityType.PaymentMethod, x.Id)
                });

        private IEnumerable<uSyncDependency> GetTaxClasses(Guid storeId)
            => _vendrApi.GetTaxClasses(storeId)
                .Select(x => new uSyncDependency
                {
                    Name = x.Name,
                    Order = VendrConstants.Priorites.TaxClass,
                    Udi = Udi.Create(VendrConstants.UdiEntityType.TaxClass, x.Id)
                });

        private IEnumerable<uSyncDependency> GetEmailTemplates(Guid storeId)
            => _vendrApi.GetEmailTemplates(storeId)
                .Select(x => new uSyncDependency
                {
                    Name = x.Name,
                    Order = VendrConstants.Priorites.EmailTemplate,
                    Udi = Udi.Create(VendrConstants.UdiEntityType.EmailTemplate, x.Id)
                });

        private IEnumerable<uSyncDependency> GetExportTemplates(Guid storeId)
            => _vendrApi.GetExportTemplates(storeId)
                .Select(x => new uSyncDependency
                {
                    Name = x.Name,
                    Order = VendrConstants.Priorites.EmailTemplate,
                    Udi = Udi.Create(VendrConstants.UdiEntityType.EmailTemplate, x.Id)
                });

        private IEnumerable<uSyncDependency> GetPrintTemplates(Guid storeId)
            => _vendrApi.GetPrintTemplates(storeId)
                .Select(x => new uSyncDependency
                {
                    Name = x.Name,
                    Order = VendrConstants.Priorites.PrintTemplate,
                    Udi = Udi.Create(VendrConstants.UdiEntityType.PrintTemplate, x.Id)
                });
                


    }
}
