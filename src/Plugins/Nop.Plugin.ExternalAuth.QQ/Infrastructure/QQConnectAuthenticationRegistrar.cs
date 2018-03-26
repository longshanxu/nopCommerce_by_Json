namespace Nop.Plugin.ExternalAuth.QQConnect.Infrastructure
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.QQ;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.ExternalAuth.QQConnect;
    using Nop.Services.Authentication.External;

    public class QQConnectAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddQQ("QQConnect", delegate (QQOptions options) {
                QQConnectExternalAuthSettings settings = EngineContext.Current.Resolve<QQConnectExternalAuthSettings>();
                options.AppId = settings.ClientKeyIdentifier;
                options.AppKey = settings.ClientSecret;
                options.SaveTokens = true;
            });
        }
    }
}

