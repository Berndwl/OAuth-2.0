using System.ComponentModel;

namespace OAuth_Workspace
{
    public enum StoreType
    {
        [Description("Autorisatie Code")] AutorisatieCode = 0,
        [Description("RefreshToken")] RefreshToken = 1,
        [Description("Access Token")] AccessToken = 2,
    }
}
