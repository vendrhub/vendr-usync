using System;
using System.Xml.Linq;
using Umbraco.Core.Logging;
using uSync8.Core;
using uSync8.Core.Models;
using uSync8.Core.Serialization;
using Vendr.Core;
using Vendr.Core.Models;
using Vendr.Core.Services;

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("d4d2593e-04ad-4a32-9ca7-e2a5b2ff2725", "Store Serializer", VendrConstants.Serialization.Store)]
    public class StoreSerializer : SyncSerializerRoot<StoreReadOnly>, ISyncSerializer<StoreReadOnly>
    {
        private IStoreService _storeService;
        private IUnitOfWorkProvider _uowProvider;

        public StoreSerializer(
            IStoreService storeService,
            IUnitOfWorkProvider uowProvider,
            ILogger logger)
            : base(logger)
        {
            _storeService = storeService;
            _uowProvider = uowProvider;
        }

        protected override Guid ItemKey(StoreReadOnly item)
            => item.Id;

        protected override string ItemAlias(StoreReadOnly item)
            => item.Alias;

        protected override SyncAttempt<StoreReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            return base.DeserializeCore(node, options);
        }

        protected override SyncAttempt<XElement> SerializeCore(StoreReadOnly item, SyncSerializerOptions options)
        {
            var node = new XElement(ItemType,
                new XAttribute("Key", item.Id),
                new XAttribute("Alias", item.Alias));

            return SyncAttempt<XElement>.SucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        protected override StoreReadOnly FindItem(Guid key)
            => _storeService.GetStore(key);

        protected override StoreReadOnly FindItem(string alias)
            => _storeService.GetStore(alias);

        protected override void SaveItem(StoreReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);

                _storeService.SaveStore(entity);

                uow.Complete();
            }
        }

        protected override void DeleteItem(StoreReadOnly item)
            => _storeService.DeleteStore(item.Id);
    }
}
