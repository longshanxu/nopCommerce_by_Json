namespace Nop.Plugin.ExternalAuth.QQConnect
{
    using Nop.Core.Configuration;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class QQConnectExternalAuthSettings : ISettings
    {

        public string ClientKeyIdentifier { get; set; }

        public string ClientSecret { get; set; }
    }
}

