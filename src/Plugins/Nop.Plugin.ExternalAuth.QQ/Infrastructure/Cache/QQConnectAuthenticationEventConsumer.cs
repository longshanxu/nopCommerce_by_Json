namespace Nop.Plugin.ExternalAuth.QQConnect.Infrastructure.Cache
{
    using Nop.Core.Domain.Customers;
    using Nop.Services.Authentication.External;
    using Nop.Services.Common;
    using Nop.Services.Events;
    using System;
    using System.IdentityModel.Claims;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class QQConnectAuthenticationEventConsumer : IConsumer<CustomerAutoRegisteredByExternalMethodEvent>
    {
        private readonly IGenericAttributeService _genericAttributeService;

        public QQConnectAuthenticationEventConsumer(IGenericAttributeService genericAttributeService)
        {
            this._genericAttributeService = genericAttributeService;
        }

        public void HandleEvent(CustomerAutoRegisteredByExternalMethodEvent eventMessage)
        {
            if (((eventMessage?.Customer != null) && (eventMessage.AuthenticationParameters != null)) && eventMessage.AuthenticationParameters.ProviderSystemName.Equals("ExternalAuth.QQConnect"))
            {
                string str = eventMessage.AuthenticationParameters.Claims.FirstOrDefault<ExternalAuthenticationClaim>(claim => claim.Type == ClaimTypes.GivenName)?.Value;
                if (!string.IsNullOrEmpty(str))
                {
                    this._genericAttributeService.SaveAttribute<string>(eventMessage.Customer, SystemCustomerAttributeNames.FirstName, str, 0);
                }
                string str2 = eventMessage.AuthenticationParameters.Claims.FirstOrDefault<ExternalAuthenticationClaim>(claim => claim.Type == ClaimTypes.Surname)?.Value;
                if (!string.IsNullOrEmpty(str2))
                {
                    this._genericAttributeService.SaveAttribute<string>(eventMessage.Customer, SystemCustomerAttributeNames.LastName, str2, 0);
                }
            }
        }
    }
}

