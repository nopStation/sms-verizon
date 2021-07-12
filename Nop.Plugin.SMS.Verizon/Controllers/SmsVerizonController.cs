using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.SMS.Verizon;
using Nop.Plugin.Sms.Verizon.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Plugins;
using System.Threading.Tasks;
using Nop.Services.Messages;
using Nop.Plugin.SMS.Verizon.Services;

namespace Nop.Plugin.Sms.Verizon.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class SmsVerizonController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly VerizonSettings _verizonSettings;
        private readonly INotificationService _notificationService;
        private readonly ISerderService _senderService;

        public SmsVerizonController(ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            VerizonSettings verizonSettings,
            INotificationService notificationService,
            ISerderService senderService)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _verizonSettings = verizonSettings;
            _notificationService = notificationService;
            _senderService = senderService;
        }

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = new SmsVerizonModel
            {
                Enabled = _verizonSettings.Enabled,
                Email = _verizonSettings.Email
            };

            return View("~/Plugins/SMS.Verizon/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(SmsVerizonModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //save settings
            _verizonSettings.Enabled = model.Enabled;
            _verizonSettings.Email = model.Email;
            await _settingService.SaveSettingAsync(_verizonSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return RedirectToAction("Configure");
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("test-sms")]
        public async Task<IActionResult> TestSms(SmsVerizonModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                if (string.IsNullOrEmpty(model.TestMessage))
                {
                    _notificationService.ErrorNotification("Enter test message");
                }
                else
                {
                    if (!await _senderService.SendSmsAsync(model.TestMessage))
                    {
                        _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Sms.Verizon.TestFailed"));
                    }
                    else
                    {
                        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Sms.Verizon.TestSuccess"));
                    }
                }
            }
            catch(Exception exc)
            {
                _notificationService.ErrorNotification(exc.ToString());
            }

            return View("~/Plugins/SMS.Verizon/Views/Configure.cshtml", model);
        }
    }
}