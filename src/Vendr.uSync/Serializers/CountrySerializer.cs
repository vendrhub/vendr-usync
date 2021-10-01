using System;
using System.Xml.Linq;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common;

using Vendr.uSync.Extensions;

#if NETFRAMEWORK
using Umbraco.Core.Logging;
using uSync8.Core;
using uSync8.Core.Extensions;
using uSync8.Core.Models;
using uSync8.Core.Serialization;
#else
using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;
using Microsoft.Extensions.Logging;
using Umbraco.Extensions;
#endif
namespace Vendr.uSync.Serializers
{
    [SyncSerializer("A5C0B948-BA5F-45FF-B6E6-EBA0BB3C6139", "Country Serializer", VendrConstants.Serialization.Country)]
    public class CountrySerializer : VendrSerializerBase<CountryReadOnly>,
        ISyncSerializer<CountryReadOnly>
    {
        public CountrySerializer(
            IVendrApi vendrApi,
            IUnitOfWorkProvider uowProvider,
#if NETFRAMEWORK
            ILogger logger) : base(vendrApi, uowProvider, logger)
#else
            ILogger<CountrySerializer> logger) : base(vendrApi, uowProvider, logger)
#endif        
        { }

        /// <summary>
        ///  Confirm that the xml contains the minimum set of things we need to perform the sync.
        /// </summary>
        public override bool IsValid(XElement node)
            => base.IsValid(node)
            && node.GetStoreId() != Guid.Empty;

        protected override SyncAttempt<CountryReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readOnlyCountry = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element("Name").ValueOrDefault(alias);
            var storeId = node.GetStoreId();
            var code = node.Element(nameof(readOnlyCountry.Code)).ValueOrDefault(string.Empty);

            using (var uow = _uowProvider.Create())
            {
                Country country;
                if (readOnlyCountry == null)
                {
                    country = Country.Create(uow, id, storeId, code, name);
                }
                else
                {
                    country = readOnlyCountry.AsWritable(uow);

                    country.SetName(name)
                        .SetCode(code);
                }

                var sortOrder = node.Element(nameof(country.SortOrder)).ValueOrDefault(country.SortOrder);
                country.SetSortOrder(sortOrder);

                var defaultCurrencyId = node.Element(nameof(country.DefaultCurrencyId)).ValueOrDefault(country.DefaultCurrencyId);
                if (defaultCurrencyId.HasValue && _vendrApi.GetCurrency(defaultCurrencyId.Value) != null) {
                    country.SetDefaultCurrency(defaultCurrencyId);
                }

                var defaultPaymentId = node.Element(nameof(country.DefaultPaymentMethodId)).ValueOrDefault(country.DefaultPaymentMethodId);
                if (defaultPaymentId.HasValue && _vendrApi.GetPaymentMethod(defaultPaymentId.Value) != null) {
                    country.SetDefaultPaymentMethod(defaultPaymentId);
                }

                var defaultShippingId = node.Element(nameof(country.DefaultShippingMethodId)).ValueOrDefault(country.DefaultShippingMethodId);
                if (defaultShippingId.HasValue && _vendrApi.GetShippingMethod(defaultShippingId.Value) != null) {
                    country.SetDefaultShippingMethod(defaultShippingId);
                }

                _vendrApi.SaveCountry(country);

                uow.Complete();

                return SyncAttemptSucceed(name, country.AsReadOnly(), ChangeType.Import, true);
            }
        }


        protected override SyncAttempt<XElement> SerializeCore(CountryReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement("Name", item.Name));
            node.Add(new XElement(nameof(item.Code), item.Code));
            node.Add(new XElement(nameof(item.DefaultCurrencyId), item.DefaultCurrencyId));
            node.Add(new XElement(nameof(item.DefaultPaymentMethodId), item.DefaultPaymentMethodId));
            node.Add(new XElement(nameof(item.DefaultShippingMethodId), item.DefaultShippingMethodId));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.AddStoreId(item.StoreId);

            return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        // overloads to let base functions do the bulk of the work.

        public override string GetItemAlias(CountryReadOnly item)
            => item.Code;

        public override CountryReadOnly DoFindItem(Guid key)
           => _vendrApi.GetCountry(key);

        public override void DoSaveItem(CountryReadOnly item)
        {
            {
                using (var uow = _uowProvider.Create())
                {
                    var entity = item.AsWritable(uow);
                    _vendrApi.SaveCountry(entity);
                    uow.Complete();
                }
            }
        }
        public override void DoDeleteItem(CountryReadOnly item)
            => _vendrApi.DeleteCountry(item.Id);

    }
}
