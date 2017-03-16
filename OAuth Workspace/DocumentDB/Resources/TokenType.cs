using System.ComponentModel;

namespace DocumentDB.Resources
{
    public enum TokenType
    {
        [Description("Bearer token")]
        Bearer = 1,
        [Description("Access token")]
        Refresh = 1,
    }
}