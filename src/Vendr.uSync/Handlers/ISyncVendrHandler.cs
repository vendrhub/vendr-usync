// slightly tidier than having the #ifdefs on all classes. 
#if NETFRAMEWORK
using uSync8.BackOffice.SyncHandlers;
#else
using uSync.BackOffice.SyncHandlers;
#endif

namespace Vendr.uSync.Handlers
{
    public interface ISyncVendrHandler :
#if NETFRAMEWORK
        ISyncExtendedHandler
#else
        ISyncHandler
#endif
    { }
}
