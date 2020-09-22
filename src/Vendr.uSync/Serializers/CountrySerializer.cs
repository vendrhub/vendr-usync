using System;
using System.Xml.Linq;

using Umbraco.Core;
using Umbraco.Core.Logging;

using uSync8.Core;
using uSync8.Core.Extensions;
using uSync8.Core.Models;
using uSync8.Core.Serialization;

using Vendr.Core;
using Vendr.Core.Api;
using Vendr.Core.Models;

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("A5C0B948-BA5F-45FF-B6E6-EBA0BB3C6139", "Country Serializer", VendrConstants.Serialization.Country)]
    public class CountrySerializer : VendrSerializerBase<CountryReadOnly>,
        ISyncSerializer<CountryReadOnly>
    {
        public CountrySerializer(
            IVendrApi vendrApi,
            IUnitOfWorkProvider uowProvider,
            ILogger logger
            ) : base(vendrApi, uowProvider, logger)
        {}

        protected override SyncAttempt<CountryReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readOnlyCountry = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element("Name").ValueOrDefault(alias);
            var storeId = node.Element(nameof(readOnlyCountry.StoreId)).ValueOrDefault(Guid.Empty);
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
                country.SetDefaultCurrency(defaultCurrencyId);

                var defaultPaymentId = node.Element(nameof(country.DefaultPaymentMethodId)).ValueOrDefault(country.DefaultPaymentMethodId);
                country.SetDefaultPaymentMethod(defaultPaymentId);

                var defaultShippingId = node.Element(nameof(country.DefaultShippingMethodId)).ValueOrDefault(country.DefaultShippingMethodId);
                country.SetDefaultShippingMethod(defaultShippingId);

                _vendrApi.SaveCountry(country);

                uow.Complete();

                return SyncAttempt<CountryReadOnly>.Succeed(name, country.AsReadOnly(), ChangeType.Import, true);
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
            node.Add(new XElement(nameof(item.StoreId), item.StoreId));

            return SyncAttempt<XElement>.SucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        // overloads to let base functions do the bulk of the work.

        protected override CountryReadOnly FindItem(Guid key)
           => _vendrApi.GetCountry(key);

        protected override string ItemAlias(CountryReadOnly item)
            => item.Name.ToSafeAlias();

        protected override void SaveItem(CountryReadOnly item)
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
        protected override void DeleteItem(CountryReadOnly item)
            => _vendrApi.DeleteCountry(item.Id);

    }
}
