using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Plugin.SMS.Verizon.Services;

namespace Nop.Plugin.SMS.Verizon
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly VerizonSettings _verizonSettings;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly ISerderService _senderService;

        public OrderPlacedEventConsumer(VerizonSettings verizonSettings,
            IOrderService orderService,
            IStoreContext storeContext,
            ISerderService senderService)
        {
            _verizonSettings = verizonSettings;
            _orderService = orderService;
            _storeContext = storeContext;
            _senderService = senderService;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            //is enabled?
            if (!_verizonSettings.Enabled)
                return;

            var order = eventMessage.Order;

            //send SMS
            if (await _senderService.SendSmsAsync($"New order(#{order.Id}) has been placed."))
            {
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = "\"Order placed\" SMS alert (to store owner) has been sent",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
        }
    }
}