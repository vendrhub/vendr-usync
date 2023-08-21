﻿using System;
using System.Linq;
using System.Xml.Linq;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common;

using Vendr.uSync.Extensions;
using Vendr.uSync.Configuration;

using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;
using Microsoft.Extensions.Logging;
using Umbraco.Extensions;
using Vendr.Extensions;

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("79ED6CC2-B1B6-42DC-9B38-7C6ACCBAF895", "Currency Serializer", VendrConstants.Serialization.Currency)]
    public class CurrencySerializer : VendrSerializerBase<CurrencyReadOnly>,
        ISyncSerializer<CurrencyReadOnly>
    {
        public CurrencySerializer(
            IVendrApi vendrApi, VendrSyncSettingsAccessor settingsAccessor,
            IUnitOfWorkProvider uowProvider,
            ILogger<CurrencySerializer> logger) : base(vendrApi, settingsAccessor, uowProvider, logger)
        {
            _vendrApi = vendrApi;
            _uowProvider = uowProvider;
        }

        protected override SyncAttempt<XElement> SerializeCore(CurrencyReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement("Name", item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.AddStoreId(item.StoreId);

            node.Add(new XElement(nameof(item.Code), item.Code));
            node.Add(new XElement(nameof(item.CultureName), item.CultureName));
            node.Add(new XElement(nameof(item.AllowedCountries), string.Join(",", item.AllowedCountries.Select(x => x.CountryId))));
            node.Add(new XElement(nameof(item.FormatTemplate), item.FormatTemplate));

            return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
        }


        public override bool IsValid(XElement node)
            => base.IsValid(node)
            && node.GetStoreId() != Guid.Empty;

        protected override SyncAttempt<CurrencyReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readOnlyCurrency = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element("Name").ValueOrDefault(alias);
            var storeId = node.GetStoreId();
            var code = node.Element(nameof(readOnlyCurrency.Code)).ValueOrDefault(string.Empty);
            var culture = node.Element(nameof(readOnlyCurrency.CultureName)).ValueOrDefault(string.Empty);

            return _uowProvider.Execute(uow =>
            {
                Currency currency;

                if (readOnlyCurrency == null)
                {
                    currency = Currency.Create(uow, id, storeId, code, name, culture);
                }
                else
                {
                    currency = readOnlyCurrency.AsWritable(uow);

                    currency.SetCode(code)
                        .SetCulture(culture)
                        .SetName(name);
                }

                var sortOrder = node.Element(nameof(currency.SortOrder)).ValueOrDefault(currency.SortOrder);
                currency.SetSortOrder(sortOrder);

                var formatTemplate = node.Element(nameof(currency.FormatTemplate)).ValueOrDefault(currency.FormatTemplate);
                currency.SetCustomFormatTemplate(formatTemplate);

                DeserializeCountries(node, currency);

                _vendrApi.SaveCurrency(currency);

                return uow.Complete(SyncAttemptSucceed(name, currency.AsReadOnly(), ChangeType.Import, true));

            });
        }

        private void DeserializeCountries(XElement node, Currency currency)
        {
            var allowedCountries = node.Element(nameof(currency.AllowedCountries))
                .ValueOrDefault(string.Empty)
                .ToDelimitedList()
                .Select(x => Guid.Parse(x));

            var countriesToRemove = currency.AllowedCountries
                .Where(x => !allowedCountries.Contains(x.CountryId))
                .Select(x => x.CountryId);


            foreach (var countryGuid in allowedCountries)
            {
                if (_vendrApi.GetCountry(countryGuid) != null)
                {
                    currency.AllowInCountry(countryGuid);
                }
            }

            foreach (var countryGuid in countriesToRemove)
            {
                currency.DisallowInCountry(countryGuid);
            }


        }

        // overloads to let base functions do the bulk of the work.

        public override string GetItemAlias(CurrencyReadOnly item)
            => item.Code;

        public override CurrencyReadOnly DoFindItem(Guid key)
            => _vendrApi.GetCurrency(key);

        public override void DoSaveItem(CurrencyReadOnly item)
        {
            _uowProvider.Execute(uow =>
            {
                var entity = item.AsWritable(uow);

                _vendrApi.SaveCurrency(entity);

                uow.Complete();
            });
        }

        public override void DoDeleteItem(CurrencyReadOnly item)
            => _vendrApi.DeleteCurrency(item.Id);

    }
}
