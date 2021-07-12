using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Logging;
using Nop.Services.Messages;
using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Verizon.Services
{
    public class SerderService : ISerderService
    {
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILogger _logger;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IStoreContext _storeContext;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly VerizonSettings _verizonSettings;

        public SerderService(IEmailAccountService emailAccountService,
            ILogger logger,
            IQueuedEmailService queuedEmailService,
            IStoreContext storeContext,
            EmailAccountSettings emailAccountSettings,
            VerizonSettings verizonSettings)
        {
            _emailAccountService = emailAccountService;
            _logger = logger;
            _queuedEmailService = queuedEmailService;
            _storeContext = storeContext;
            _emailAccountSettings = emailAccountSettings;
            _verizonSettings = verizonSettings;
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">SMS text</param>
        /// <returns>Result</returns>
        public async Task<bool> SendSmsAsync(string text)
        {
            try
            {
                var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId) ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

                if (emailAccount == null)
                    throw new Exception("No email account could be loaded");

                var queuedEmail = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = _verizonSettings.Email,
                    ToName = string.Empty,
                    Subject = _storeContext.GetCurrentStore().Name,
                    Body = text,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                };

                await _queuedEmailService.InsertQueuedEmailAsync(queuedEmail);

                return true;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                return false;
            }
        }
    }
}
