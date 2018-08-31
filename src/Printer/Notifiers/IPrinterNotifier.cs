using System;
using System.Threading.Tasks;

namespace Printer.Notifiers
{
    public interface IPrinterNotifier : IDisposable
    {
        Task Subscribe(Action<string> callback, PrinterNotificationTypes notificationType, Guid correlationId = default);
        Task Notify(string message, PrinterNotificationTypes notificationType, Guid correlationId = default);
    }
}