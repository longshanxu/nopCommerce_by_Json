namespace Nop.Plugin.ExternalAuth.QQConnect.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.QQ;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Nop.Core;
    using Nop.Core.Domain.Customers;
    using Nop.Plugin.ExternalAuth.QQConnect;
    using Nop.Plugin.ExternalAuth.QQConnect.Models;
    using Nop.Services.Authentication.External;
    using Nop.Services.Configuration;
    using Nop.Services.Localization;
    using Nop.Services.Security;
    using Nop.Services.Stores;
    using Nop.Web.Framework.Controllers;
    using Nop.Web.Framework.Mvc.Filters;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class QQConnectAuthenticationController : BasePluginController
    {
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly QQConnectExternalAuthSettings _facebookExternalAuthSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IOptionsMonitorCache<QQOptions> _optionsCache;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        public QQConnectAuthenticationController(ISettingService settingService, ExternalAuthenticationSettings externalAuthenticationSettings, IPermissionService permissionService, IStoreContext storeContext, IStoreService storeService, ILocalizationService localizationService, IExternalAuthenticationService externalAuthenticationService, QQConnectExternalAuthSettings facebookExternalAuthSettings, IOptionsMonitorCache<QQOptions> optionsCache)
        {
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._externalAuthenticationService = externalAuthenticationService;
            this._facebookExternalAuthSettings = facebookExternalAuthSettings;
            this._optionsCache = optionsCache;
        }

        [AuthorizeAdmin(false), Area("Admin")]
        public IActionResult Configure()
        {
            if (!this._permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
            {
                return this.AccessDeniedView();
            }
            ConfigurationModel model = new ConfigurationModel {
                CallbackUrl = (string) new QQOptions().CallbackPath,
                ClientKeyIdentifier = this._facebookExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = this._facebookExternalAuthSettings.ClientSecret
            };
            return this.View("~/Plugins/ExternalAuth.QQConnect/Views/Configure.cshtml", model);
        }

        [HttpPost, AdminAntiForgery(false), AuthorizeAdmin(false), Area("Admin")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!this._permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
            {
                return this.AccessDeniedView();
            }
            if (base.ModelState.IsValid)
            {
                this._facebookExternalAuthSettings.ClientKeyIdentifier = model.ClientKeyIdentifier;
                this._facebookExternalAuthSettings.ClientSecret = model.ClientSecret;
                this._settingService.SaveSetting<QQConnectExternalAuthSettings>(this._facebookExternalAuthSettings, 0);
                this._optionsCache.TryRemove("QQConnect");
                this.SuccessNotification(this._localizationService.GetResource("Admin.Plugins.Saved"), true);
            }
            return this.Configure();
        }

        public IActionResult Login(string returnUrl)
        {
            if (!this._externalAuthenticationService.ExternalAuthenticationMethodIsAvailable("ExternalAuth.QQConnect"))
            {
                throw new NopException("QQConnect authentication module cannot be loaded");
            }
            if (string.IsNullOrEmpty(this._facebookExternalAuthSettings.ClientKeyIdentifier) || string.IsNullOrEmpty(this._facebookExternalAuthSettings.ClientSecret))
            {
                throw new NopException("QQConnect authentication module not configured");
            }
            AuthenticationProperties properties = new AuthenticationProperties {
                RedirectUri = base.Url.Action("LoginCallback", "QQConnectAuthentication", new { returnUrl = returnUrl })
            };
            string[] authenticationSchemes = new string[] { "QQConnect" };
            return this.Challenge(properties, authenticationSchemes);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            IActionResult result;
            AuthenticateResult asyncVariable0 = await AuthenticationHttpContextExtensions.AuthenticateAsync(this.HttpContext, "QQConnect");
            AuthenticateResult authenticateResult = asyncVariable0;
            asyncVariable0 = null;
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any<Claim>())
            {
                result = this.RedirectToRoute("Login");
            }
            else
            {
                ExternalAuthenticationParameters asyncVariable3 = new ExternalAuthenticationParameters {
                    ProviderSystemName = "ExternalAuth.QQConnect"
                };
                ExternalAuthenticationParameters asyncVariable1 = asyncVariable3;
                string asyncVariable2 = await AuthenticationHttpContextExtensions.GetTokenAsync(this.HttpContext, "QQConnect", "access_token");
                asyncVariable1.AccessToken = asyncVariable2;
                asyncVariable3.Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value;
                asyncVariable3.ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                asyncVariable3.ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value;
                asyncVariable3.Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList();
                ExternalAuthenticationParameters parameters = asyncVariable3;
                asyncVariable1 = null;
                asyncVariable2 = null;
                asyncVariable3 = null;
                result = this._externalAuthenticationService.Authenticate(parameters, returnUrl);
            }
            return result;
        }

    }
}

