using System;
using System.Threading.Tasks;

namespace Printer.Commands
{
    using Exceptions;
    using Handlers;
    using Indicators;
    using Notifiers;

    public class PrintBlackCommand : IPrintCommand
    {
        private PrintCommandHandler _handler;

        public byte[] Content { get; private set; }

        public Guid Id { get; private set; }

        public PrintBlackCommand(byte[] content,
            IPaperLevelIndicator paperLevelIndicator,
            IBlackLevelIndicator blackLevelIndicator,
            IPrinterNotifier notifier = null)
        {
            if (content is null || content.Length == 0)
                throw new NoContentToPrintException();

            _handler = new PrintCommandHandler(this, notifier, paperLevelIndicator, blackLevelIndicator);
            Content = content;
            Id = Guid.NewGuid();
        }

        public async Task Execute()
        {
            await _handler.Handle();
        }
    }
}