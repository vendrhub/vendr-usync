
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using uSync8.BackOffice.Models;

namespace Vendr.uSync
{
    /// <summary>
    ///  Info class, so the version etc, appear on the dashboard
    /// </summary>
    /// <remarks>
    ///  Not strictly required, just lets people see its installed.
    /// </remarks>
    [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy))]
    public class VendrSync : ISyncAddOn
    {
        public string Name => "Vendr.uSync";

        public string Version => "1.0.0";

        public string Icon => "icon-store";

        public string View => string.Empty;

        public string Alias => "vendrSync";

        public string DisplayName => "uSync for Vendr";

        public int SortOrder => 11;
    }
}
