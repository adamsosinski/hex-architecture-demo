using System;
using System.Threading.Tasks;

namespace Printer.Notifiers
{
    internal class EmptyPrinterNotifier : IPrinterNotifier
    {
        public Task Notify(string message, PrinterNotificationTypes notificationType, Guid correlationId = default) => Task.CompletedTask;
        public Task Subscribe(Action<string> callback, PrinterNotificationTypes notificationType, Guid correlationId = default) => Task.CompletedTask;

        public void Dispose() { }
    }
}