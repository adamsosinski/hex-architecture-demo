using System.Threading.Tasks;

namespace Printer
{
    using Commands;
    using Indicators;
    using Notifiers;

    public class Printer : IRequestPrint
    {
        private readonly IPaperLevelIndicator _pli;
        private readonly IBlackLevelIndicator _bli;
        private readonly IColorLevelIndicator _cli;
        private readonly IPrinterNotifier _n;

        public Printer(IPaperLevelIndicator paperLevelIndicator,
            IBlackLevelIndicator blackLevelIndicator,
            IColorLevelIndicator colorLevelIndicator,
            IPrinterNotifier notifier)
        {
            _pli = paperLevelIndicator;
            _bli = blackLevelIndicator;
            _cli = colorLevelIndicator;
            _n = notifier;
        }

        public async Task PrintBlack(byte[] content)
        {
            var command = new PrintBlackCommand(content, _pli, _bli, _n);
            await command.Execute();
        }

        public async Task PrintColor(byte[] content)
        {
            var command = new PrintColorCommand(content, _pli, _bli, _cli, _n);
            await command.Execute();
        }
    }
}