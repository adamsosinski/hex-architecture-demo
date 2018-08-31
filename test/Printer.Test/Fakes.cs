using System;
using System.Threading.Tasks;

namespace Printer.Test.Fakes
{
    using Notifiers;

    internal class FakePrinterNotifier : IPrinterNotifier
    {
        private string _channel;
        private Action<string> _callback;

        public Task Notify(string message, PrinterNotificationTypes notificationType, Guid correlationId = default)
        {
            var channel = GetChannel(notificationType, correlationId);

            if (channel.Equals(_channel))
                _callback(message);

            return Task.CompletedTask;
        }

        public Task Subscribe(Action<string> callback, PrinterNotificationTypes notificationType, Guid correlationId = default)
        {
            _channel = GetChannel(notificationType, correlationId);
            _callback = callback;

            return Task.CompletedTask;
        }

        public void Dispose() { }

        private string GetChannel(PrinterNotificationTypes nt, Guid correlationId) =>
            correlationId == Guid.Empty ? $"FakePrinter.{nt}" : $"FakePrinter.{nt}.{correlationId}";
    }
}