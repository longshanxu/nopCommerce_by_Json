namespace Nop.Plugin.ExternalAuth.QQConnect.Components
{
    using Microsoft.AspNetCore.Mvc;
    using Nop.Web.Framework.Components;

    [ViewComponent(Name="QQConnectAuthentication")]
    public class QQConnectAuthenticationViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke() => 
            base.View("~/Plugins/ExternalAuth.QQConnect/Views/PublicInfo.cshtml");
    }
}

