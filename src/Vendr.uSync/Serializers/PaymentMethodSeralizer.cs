using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;

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
    [SyncSerializer("707A16D7-AAA8-4399-8CF4-BEC82B8F6C8E", "PaymentMethod Serializer", VendrConstants.Serialization.PaymentMethod)]
    public class PaymentMethodSeralizer : MethodSerializerBase<PaymentMethodReadOnly>, ISyncSerializer<PaymentMethodReadOnly>
    {
        public PaymentMethodSeralizer(IVendrApi vendrApi, IUnitOfWorkProvider uowProvider, ILogger logger)
            : base(vendrApi, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(PaymentMethodReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.Add(new XElement(nameof(item.StoreId)), item.StoreId);

            node.Add(SerializeCountryRegions(item.AllowedCountryRegions));

            node.Add(new XElement(nameof(item.CanCancelPayments)), item.CanCancelPayments);
            node.Add(new XElement(nameof(item.CanCapturePayments)), item.CanCapturePayments);
            node.Add(new XElement(nameof(item.CanFetchPaymentStatuses)), item.CanFetchPaymentStatuses);
            node.Add(new XElement(nameof(item.CanRefundPayments)), item.CanRefundPayments);

            node.Add(SerializeProviderSettings(item.PaymentProviderSettings));
            node.Add(SerializePrices(item.Prices));

            node.Add(new XElement(nameof(item.ImageId)), item.ImageId);
            node.Add(new XElement(nameof(item.PaymentProviderAlias)), item.PaymentProviderAlias);

            node.Add(new XElement(nameof(item.Sku)), item.Sku);
            node.Add(new XElement(nameof(item.TaxClassId), item.TaxClassId));

            return SyncAttempt<XElement>.SucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        private XElement SerializeProviderSettings(IReadOnlyDictionary<string, string> values)
        {
            var root = new XElement("ProviderSettings");

            if (values != null && values.Any())
            {
                foreach (var setting in values)
                {
                    root.Add(new XElement("Setting",
                        new XElement("Key", setting.Key),
                        new XElement("Value", setting.Value)));
                }
            }

            return root;
        }


        protected override SyncAttempt<PaymentMethodReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readonlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readonlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.Element(nameof(readonlyItem.StoreId)).ValueOrDefault(Guid.Empty);
            var providerAlias = node.Element(nameof(readonlyItem.PaymentProviderAlias)).ValueOrDefault(string.Empty);

            using (var uow = _uowProvider.Create())
            {
                PaymentMethod item;
                if (readonlyItem == null)
                {
                    item = PaymentMethod.Create(uow, id, storeId, alias, name, providerAlias);
                }
                else
                {
                    item = readonlyItem.AsWritable(uow);
                    item.SetAlias(alias)
                        .SetName(name);
                }

                item.SetSortOrder(node.Element(nameof(item.SortOrder)).ValueOrDefault(item.SortOrder));
                item.SetImage(node.Element(nameof(item.ImageId)).ValueOrDefault(item.ImageId));
                item.SetSku(node.Element(nameof(item.Sku)).ValueOrDefault(item.Sku));
                item.SetTaxClass(node.Element(nameof(item.TaxClassId)).ValueOrDefault(item.TaxClassId));

                item.ToggleFeatures(
                    node.Element(nameof(item.CanFetchPaymentStatuses)).ValueOrDefault(item.CanFetchPaymentStatuses),
                    node.Element(nameof(item.CanCapturePayments)).ValueOrDefault(item.CanCapturePayments),
                    node.Element(nameof(item.CanCancelPayments)).ValueOrDefault(item.CanCancelPayments),
                    node.Element(nameof(item.CanRefundPayments)).ValueOrDefault(item.CanRefundPayments));

                // do the payment method stuff
                DeserializeProviderSettings(node, item);

                // Country regions
                DeserializeCountryRegions(node, item);

                // currency 
                DeserializePrices(node, item);

                _vendrApi.SavePaymentMethod(item);

                uow.Complete();

                return SyncAttempt<PaymentMethodReadOnly>.Succeed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        private void DeserializeProviderSettings(XElement node, PaymentMethod item)
        {
            var settings = new Dictionary<string, string>();

            var root = node.Element("ProviderSettings");
            if (root != null && root.HasElements)
            {
                foreach (var setting in root.Elements("Setting"))
                {
                    var key = setting.Element("Key").ValueOrDefault(string.Empty);
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        var value = setting.Element("Value").ValueOrDefault(string.Empty);
                        settings.Add(key, value);
                    }
                }
            }

            item.SetSettings(settings, SetBehavior.Replace);
        }

        private void DeserializeCountryRegions(XElement node, PaymentMethod item)
        {
            var countryRegions = GetCountryRegionsList(node);

            var valuesToRemove = item.AllowedCountryRegions
                .Where(x => countryRegions == null || !item.AllowedCountryRegions.Any(y => y.CountryId == x.CountryId
                     && y.RegionId == y.RegionId))
                .ToList();

            if (countryRegions.Count > 0)
            {
                foreach (var acr in countryRegions)
                {
                    if (acr.RegionId != null)
                    {
                        item.AllowInRegion(acr.CountryId, acr.RegionId.Value);
                    }
                    else
                    {
                        item.AllowInCountry(acr.CountryId);
                    }
                }
            }

            foreach (var acr in valuesToRemove)
            {
                if (acr.RegionId != null)
                {
                    item.DisallowInRegion(acr.CountryId, acr.RegionId.Value);
                }
                else
                {
                    item.DisallowInCountry(acr.CountryId);
                }
            }
        }

        private void DeserializePrices(XElement node, PaymentMethod item)
        {
            var prices = GetServicePrices(node);

            var pricesToRemove = item.Prices
                .Where(x => item.Prices == null
                || !prices.Any(y => y.CountryId == x.CountryId
                                && y.RegionId == x.RegionId
                                && y.CurrencyId == y.CurrencyId))
                .ToList();

            foreach (var price in prices)
            {
                if (price.CountryId == null && price.RegionId == null)
                {
                    item.SetDefaultPriceForCurrency(price.CurrencyId.Value, price.Value);
                }
                else
                {
                    if (price.RegionId != null)
                    {
                        item.SetRegionPriceForCurrency(price.CountryId.Value, price.RegionId.Value, price.CurrencyId.Value, price.Value);
                    }
                    else
                    {
                        item.SetCountryPriceForCurrency(price.CountryId.Value, price.CurrencyId.Value, price.Value);
                    }
                }
            }

            foreach (var price in pricesToRemove)
            {
                if (price.CountryId == null && price.RegionId == null)
                {
                    item.ClearDefaultPriceForCurrency(price.CurrencyId);
                }
                else if (price.CountryId != null && price.RegionId == null)
                {
                    item.ClearCountryPriceForCurrency(price.CountryId.Value, price.CurrencyId);
                }
                else
                {
                    item.ClearRegionPriceForCurrency(price.CountryId.Value, price.RegionId.Value, price.CurrencyId);
                }
            }
        }

        protected override void DeleteItem(PaymentMethodReadOnly item)
            => _vendrApi.DeletePaymentMethod(item.Id);

        protected override PaymentMethodReadOnly FindItem(Guid key)
            => _vendrApi.GetPaymentMethod(key);

        protected override string ItemAlias(PaymentMethodReadOnly item)
            => item.Alias;

        protected override void SaveItem(PaymentMethodReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _vendrApi.SavePaymentMethod(entity);
                uow.Complete();
            }
        }
    }
}
