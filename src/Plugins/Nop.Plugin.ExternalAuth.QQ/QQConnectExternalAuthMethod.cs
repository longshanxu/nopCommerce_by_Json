namespace Nop.Plugin.ExternalAuth.QQConnect
{
    using Nop.Core;
    using Nop.Core.Plugins;
    using Nop.Services.Authentication.External;
    using Nop.Services.Configuration;
    using Nop.Services.Localization;
    using System;
    using System.Runtime.InteropServices;

    public class QQConnectExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod, IPlugin
    {
        private readonly ISettingService _settingService;
        private IWebHelper _webHelper;

        public QQConnectExternalAuthMethod(ISettingService settingService, IWebHelper webHelper)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        public override string GetConfigurationPageUrl() => 
            $"{this._webHelper.GetStoreLocation(null)}Admin/QQConnectAuthentication/Configure";

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "QQConnectAuthentication";
        }

        public string GetPublicViewComponentName()
        {
            return QQConnectAuthenticationDefaults.ViewComponentName;
        }

        public override void Install()
        {
            QQConnectExternalAuthSettings settings = new QQConnectExternalAuthSettings {
                ClientKeyIdentifier = "",
                ClientSecret = ""
            };
            this._settingService.SaveSetting<QQConnectExternalAuthSettings>(settings, 0);
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.Login", "Login using QQConnect account", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.CallbackUrl", "Callback Url", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.ClientKeyIdentifier", "App ID/API Key", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your QQConnect application page.", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.ClientSecret", "App Secret", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.ClientSecret.Hint", "Enter your app secret here. You can find it on your QQConnect application page.", null);
            base.Install();
        }

        public override void Uninstall()
        {
            this._settingService.DeleteSetting<QQConnectExternalAuthSettings>();
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.Login");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.CallbackUrl");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.ClientKeyIdentifier");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.ClientKeyIdentifier.Hint");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.ClientSecret.Hint");
            base.Uninstall();
        }
    }
}

