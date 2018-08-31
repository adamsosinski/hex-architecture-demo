using System;
using System.Threading.Tasks;

namespace Printer.Commands
{
    using Exceptions;
    using Handlers;
    using Indicators;
    using Notifiers;

    public class PrintColorCommand : IPrintCommand
    {
        private PrintCommandHandler _handler;

        public Guid Id { get; private set; }
        public byte[] Content { get; private set; }

        public PrintColorCommand(byte[] content,
            IPaperLevelIndicator paperLevelIndicator,
            IBlackLevelIndicator blackLevelIndicator,
            IColorLevelIndicator colorLevelIndicator,
            IPrinterNotifier notifier = null)
        {
            if (content is null || content.Length == 0)
                throw new NoContentToPrintException();

            _handler = new PrintCommandHandler(this, notifier, paperLevelIndicator, blackLevelIndicator, colorLevelIndicator);
            Content = content;
            Id = Guid.NewGuid();
        }

        public async Task Execute()
        {
            await _handler.Handle();
        }
    }
}