using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Verizon
{
    /// <summary>
    /// Represents the Verizon SMS provider
    /// </summary>
    public class VerizonSmsProvider : BasePlugin, IMiscPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public VerizonSmsProvider(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/SmsVerizon/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new VerizonSettings
            {
                Email = "yournumber@vtext.com",
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.TestFailed", "Test message sending failed");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.TestSuccess", "Test message was sent (queued)");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.Fields.Enabled", "Enabled");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.Fields.Enabled.Hint", "Check to enable SMS provider");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.Fields.Email", "Email");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.Fields.Email.Hint", "Verizon email address(e.g. your_phone_number@vtext.com)");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.Fields.TestMessage", "Message text");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.Fields.TestMessage.Hint", "Text of the test message");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.SendTest", "Send");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Verizon.SendTest.Hint", "Send test message");

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<VerizonSettings>();

            //locales
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.TestFailed");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.TestSuccess");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.Fields.Enabled");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.Fields.Enabled.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.Fields.Email");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.Fields.Email.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.Fields.TestMessage");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.Fields.TestMessage.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.SendTest");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Verizon.SendTest.Hint");

            await base.UninstallAsync();
        }
    }
}
