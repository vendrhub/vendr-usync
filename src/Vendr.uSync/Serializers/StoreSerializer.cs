﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common;
using Vendr.uSync.Configuration;

using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Services;
using Microsoft.Extensions.Logging;
using Vendr.Extensions;

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("d4d2593e-04ad-4a32-9ca7-e2a5b2ff2725", "Store Serializer", VendrConstants.Serialization.Store,IsTwoPass = true)]
    public class StoreSerializer : VendrSerializerBase<StoreReadOnly>, ISyncSerializer<StoreReadOnly>
    {
        private IUserService _userService;

        public StoreSerializer(
            IUserService userService,
            IVendrApi vendrApi, 
            VendrSyncSettingsAccessor settingsAccessor,
            IUnitOfWorkProvider uowProvider,
            ILogger<StoreSerializer> logger) : base(vendrApi, settingsAccessor, uowProvider, logger)
        {
            _userService = userService;
        }

        protected override SyncAttempt<XElement> SerializeCore(StoreReadOnly item, SyncSerializerOptions options)
        {
            // makes the basic xml,
            var node = InitializeBaseNode(item, item.Alias);

            node.Add(new XElement("Name", item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));

            node.Add(new XElement(nameof(item.PricesIncludeTax), item.PricesIncludeTax));
            node.Add(new XElement(nameof(item.CookieTimeout), item.CookieTimeout));
            node.Add(new XElement(nameof(item.CartNumberTemplate), item.CartNumberTemplate));

            // product
            node.Add(new XElement(nameof(item.ProductPropertyAliases), string.Join(",", item.ProductPropertyAliases)));
            node.Add(new XElement(nameof(item.ProductUniquenessPropertyAliases), item.ProductUniquenessPropertyAliases));

            // gift card
            node.Add(new XElement(nameof(item.GiftCardCodeLength), item.GiftCardCodeLength));
            node.Add(new XElement(nameof(item.GiftCardDaysValid), item.GiftCardDaysValid));
            node.Add(new XElement(nameof(item.GiftCardCodeTemplate), item.GiftCardCodeTemplate));
            node.Add(new XElement(nameof(item.GiftCardPropertyAliases), string.Join(",", item.GiftCardPropertyAliases)));
            node.Add(new XElement(nameof(item.GiftCardActivationMethod), (int)item.GiftCardActivationMethod));

            // order
#pragma warning disable CS0618 // Type or member is obsolete
            node.Add(new XElement(nameof(item.OrderEditorConfig), item.OrderEditorConfig));
#pragma warning restore CS0618 // Type or member is obsolete
            node.Add(new XElement(nameof(item.OrderNumberTemplate), item.OrderNumberTemplate));

            node.Add(AddNullableGuid(nameof(item.BaseCurrencyId), item.BaseCurrencyId));
            node.Add(AddNullableGuid(nameof(item.DefaultCountryId), item.DefaultCountryId));
            node.Add(AddNullableGuid(nameof(item.DefaultTaxClassId), item.DefaultTaxClassId));
            node.Add(AddNullableGuid(nameof(item.DefaultOrderStatusId), item.DefaultOrderStatusId));
            node.Add(AddNullableGuid(nameof(item.ErrorOrderStatusId), item.ErrorOrderStatusId));
            node.Add(AddNullableGuid(nameof(item.GiftCardActivationOrderStatusId), item.GiftCardActivationOrderStatusId));
            node.Add(AddNullableGuid(nameof(item.DefaultGiftCardEmailTemplateId), item.DefaultGiftCardEmailTemplateId));
            
            node.Add(AddNullableGuid(nameof(item.ConfirmationEmailTemplateId), item.ConfirmationEmailTemplateId));
            node.Add(AddNullableGuid(nameof(item.ErrorEmailTemplateId), item.ErrorEmailTemplateId));

            node.Add(AddNullableGuid(nameof(item.ErrorOrderStatusId), item.ErrorOrderStatusId));
            node.Add(AddNullableGuid(nameof(item.ShareStockFromStoreId), item.ShareStockFromStoreId));

            SerializeAllowedUsers(node, item);

            SerializeUserRoles(node, item);

            return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        private void SerializeAllowedUsers(XElement node, StoreReadOnly item)
        {
            var allowedUsers = new XElement(nameof(item.AllowedUsers));

            if (item.AllowedUsers.Count > 0)
            {
                foreach (var id in item.AllowedUsers)
                {
                    var user = _userService.GetByProviderKey(id.UserId);
                    if (user != null)
                    {
                        allowedUsers.Add(new XElement("User", user.Username));
                    }
                }
            }

            node.Add(allowedUsers);
        }

        private void SerializeUserRoles(XElement node, StoreReadOnly item)
        {
            var allowedRoles = new XElement(nameof(item.AllowedUserRoles));
            if (item.AllowedUserRoles.Count > 0)
            {
                foreach (var role in item.AllowedUserRoles)
                {
                    var group = _userService.GetUserGroupByAlias(role.Role);
                    if (group != null)
                    {
                        allowedRoles.Add(new XElement("Role", group.Alias));
                    }
                }
            }

            node.Add(allowedRoles);
        }

        public override bool IsValid(XElement node)
            => base.IsValid(node)
            && !string.IsNullOrWhiteSpace(node.Element("Name").ValueOrDefault(string.Empty));

        protected override SyncAttempt<StoreReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readOnlyStore = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element("Name").ValueOrDefault(alias);

            return _uowProvider.Execute(uow =>
            {
                Store store;

                if (readOnlyStore == null)
                {
                    store = Store.Create(uow, id, alias, name, false);
                }
                else
                {
                    store = readOnlyStore.AsWritable(uow);
                }

                // here we have found or created the store item.

                store.SetName(name);

                store.SetSortOrder(node.Element(nameof(store.SortOrder)).ValueOrDefault(store.SortOrder));

                store.SetPriceTaxInclusivity(node.Element(nameof(store.PricesIncludeTax)).ValueOrDefault(false));

                store.SetCartNumberTemplate(node.Element(nameof(store.CartNumberTemplate)).ValueOrDefault(string.Empty));

                store.SetProductPropertyAliases(node.Element(nameof(store.ProductPropertyAliases)).ValueOrDefault(string.Empty)
                    .ToDelimitedList());

                store.SetProductUniquenessPropertyAliases(node.Element(nameof(store.ProductUniquenessPropertyAliases)).ValueOrDefault(string.Empty).ToDelimitedList());

                store.SetGiftCardCodeLength(node.Element(nameof(store.GiftCardCodeLength)).ValueOrDefault(store.GiftCardCodeLength));
                store.SetGiftCardValidityTimeframe(node.Element(nameof(store.GiftCardDaysValid)).ValueOrDefault(store.GiftCardDaysValid));
                store.SetGiftCardCodeTemplate(node.Element(nameof(store.GiftCardCodeTemplate)).ValueOrDefault(store.GiftCardCodeTemplate));

                store.SetGiftCardActivationMethod(node.Element(nameof(store.GiftCardActivationMethod)).ValueOrDefault(store.GiftCardActivationMethod));

                var giftCardPropertyAliasList = node.Element(nameof(store.GiftCardPropertyAliases))
                    .ValueOrDefault(string.Empty).ToDelimitedList();
                if (giftCardPropertyAliasList != null && giftCardPropertyAliasList.Count > 0)
                {
                    store.SetGiftCardPropertyAliases(giftCardPropertyAliasList);
                }
                else
                {
                    store.ClearGiftCardPropertyAliases();
                }

#pragma warning disable CS0618 // Type or member is obsolete
                store.SetOrderEditorConfig(node.Element(nameof(store.OrderEditorConfig)).ValueOrDefault(store.OrderEditorConfig));
#pragma warning restore CS0618 // Type or member is obsolete

                // base currency
                Guid? currencyId = GetCurrencyId(node, nameof(store.BaseCurrencyId));
                store.SetBaseCurrency(currencyId);

                // country 
                Guid? countryId = GetCountryId(node, nameof(store.DefaultCountryId));
                store.SetDefaultCountry(countryId);

                // tax class 
                Guid? taxClassId = GetTaxClassId(node, nameof(store.DefaultTaxClassId));
                store.SetDefaultTaxClass(taxClassId);

                // DefaultOrderStatus 
                Guid? defaultOrderStatusId = GetOrderStatusId(node, nameof(store.DefaultOrderStatusId));
                store.SetDefaultOrderStatus(defaultOrderStatusId);

                // error order status 
                Guid? errorOrderStatusId = GetOrderStatusId(node, nameof(store.ErrorOrderStatusId));
                store.SetErrorOrderStatus(errorOrderStatusId);

                // gift card template
                var defaultGiftCardEmailTemplateId = GetEmailTemplateId(node, nameof(store.DefaultGiftCardEmailTemplateId));
                store.SetDefaultGiftCardEmailTemplate(defaultGiftCardEmailTemplateId);

                // confimation email template
                var confirmationEmailTemplateId = GetEmailTemplateId(node, nameof(store.ConfirmationEmailTemplateId));
                store.SetConfirmationEmailTemplate(confirmationEmailTemplateId);

                // error email template
                var errorEmailTemplateId = GetEmailTemplateId(node, nameof(store.ErrorEmailTemplateId));
                store.SetErrorEmailTemplate(errorEmailTemplateId);

                DeserializeAllowedUsers(node, store);

                DeserializeAllowedRoles(node, store);

                _vendrApi.SaveStore(store);

                return uow.Complete(SyncAttemptSucceed(name, store.AsReadOnly(), ChangeType.Import, true));
            });
        }

        private void DeserializeAllowedRoles(XElement node, Store store)
        {
            var roleIds = new List<string>();

            var collection = node.Element(nameof(store.AllowedUserRoles));
            if (collection != null && collection.HasElements)
            {
                foreach (var item in collection.Elements("Role"))
                {
                    var alias = item.Value;

                    if (!string.IsNullOrEmpty(alias))
                    {
                        var role = _userService.GetUserGroupByAlias(alias);
                        if (role != null)
                        {
                            roleIds.Add(role.Alias);
                        }
                    }
                }
            }

            store.SetAllowedUserRoles(roleIds, SetBehavior.Replace);
        }

        private void DeserializeAllowedUsers(XElement node, Store store)
        {
            var userIds = new List<string>();

            var collection = node.Element(nameof(store.AllowedUsers));
            if (collection != null && collection.HasElements)
            {
                foreach (var item in collection.Elements("User"))
                {
                    var username = item.Value;

                    if (!string.IsNullOrEmpty(username))
                    {
                        var user = _userService.GetByUsername(username);
                        if (user != null)
                        {
                            userIds.Add(user.Id.ToString());
                        }
                    }
                }
            }

            store.SetAllowedUsers(userIds, SetBehavior.Replace);
        }

        /// <summary>
        ///  called as part of the serialization, after all stores are serialized.
        /// </summary>
        /// <remarks>
        ///  The second pass happens once all store items have gone through their first pass - as such you only need to put things
        ///  here that rely on other stores being setup. 
        ///  </remarks>
        public override SyncAttempt<StoreReadOnly> DeserializeSecondPass(StoreReadOnly item, XElement node, SyncSerializerOptions options)
        {
            if (item == null) return SyncAttempt<StoreReadOnly>.Fail(node.GetAlias(), ChangeType.ImportFail, "Store Item not set for second pass");

            // currency 
            return _uowProvider.Execute(uow =>
            {
                var store = item.AsWritable(uow);

                // StockSharingStore
                var stockSharingStore = GetStoreId(node, nameof(store.ShareStockFromStoreId));
                if (stockSharingStore.HasValue)
                {
                    store.ShareStockFrom(stockSharingStore.Value);
                }
                else
                {
                    store.StopSharingStock();
                }

                _vendrApi.SaveStore(store);

                return uow.Complete(SyncAttemptSucceed(store.Name, store.AsReadOnly(), ChangeType.Import, true));
            });

        }


        /// <summary>
        ///  gets a guid value from the xml, and if set checks with the passed vendr method that it exsits
        /// </summary>
        /// <param name="node">XElement containing values</param>
        /// <param name="name">name of the node in the xml containing the guid</param>
        /// <param name="action">method (that returns a EntityBase) to check value</param>
        private Guid? GetVendrIdFromXml(XElement node, string name, Func<Guid, EntityBase> action)
        {
            var value = node.Element(name).ValueOrDefault(Guid.Empty);
            if (value != Guid.Empty)
            {
                return action(value)?.Id;
            }

            return null;
        }

        /// <summary>
        ///  Get the CurrencyId from the xml and confirm it exist in vendr.
        /// </summary>
        private Guid? GetCurrencyId(XElement node, string name)
            => GetVendrIdFromXml(node, name, _vendrApi.GetCurrency);

        /// <summary>
        ///  Get the CountryId from the xml and confirm it exist in vendr.
        /// </summary>
        private Guid? GetCountryId(XElement node, string name)
            => GetVendrIdFromXml(node, name, _vendrApi.GetCountry);

        /// <summary>
        ///  Get the TaxClassId from the xml and confirm it exist in vendr.
        /// </summary>
        private Guid? GetTaxClassId(XElement node, string name)
            => GetVendrIdFromXml(node, name, _vendrApi.GetTaxClass);

        /// <summary>
        ///  Get the OrderId from the xml and confirm it exist in vendr.
        /// </summary>
        private Guid? GetOrderStatusId(XElement node, string name)
            => GetVendrIdFromXml(node, name, _vendrApi.GetOrderStatus);

        /// <summary>
        ///  Get the EmailTemplateId from the xml and confirm it exist in vendr.
        /// </summary>
        private Guid? GetEmailTemplateId(XElement node, string name)
            => GetVendrIdFromXml(node, name, _vendrApi.GetEmailTemplate);

        /// <summary>
        ///  Get the StoreId from the xml and confirm it exist in vendr.
        /// </summary>
        private Guid? GetStoreId(XElement node, string name)
            => GetVendrIdFromXml(node, name, _vendrApi.GetStore);



        private XElement AddNullableGuid(string alias, Guid? value)
            => new XElement(alias, value.HasValue ? value : Guid.Empty);


        // overloads to let base functions do the bulk of the work.

        public override string GetItemAlias(StoreReadOnly item)
            => item.Alias;

        public override StoreReadOnly DoFindItem(Guid key)
            => _vendrApi.GetStore(key);

        public override StoreReadOnly DoFindItem(string alias)
            => _vendrApi.GetStore(alias);

        public override void DoSaveItem(StoreReadOnly item)
        {
            _uowProvider.Execute(uow =>
            {
                var entity = item.AsWritable(uow);
                _vendrApi.SaveStore(entity);
                uow.Complete();
            });
        }

        public override void DoDeleteItem(StoreReadOnly item)
            => _vendrApi.DeleteStore(item.Id);
    }
}
