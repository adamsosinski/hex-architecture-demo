using System;
using System.Threading.Tasks;

namespace Printer.Handlers
{
    using Commands;
    using Exceptions;
    using Indicators;
    using Indicators.Exceptions;
    using Notifiers;

    internal class PrintCommandHandler : ICommandHandler
    {
        private IPrintCommand _cmd;
        private IPrinterNotifier _n;
        private IPaperLevelIndicator _pli;
        private IBlackLevelIndicator _bli;
        private IColorLevelIndicator _cli;

        private bool _isColor => _cmd is PrintColorCommand;

        internal PrintCommandHandler(IPrintCommand cmd, IPrinterNotifier notifier, params ISufficientLevelIndicator[] indicators)
        {
            _cmd = cmd;
            _n = notifier ?? new EmptyPrinterNotifier();

            InjectSufficientLevelIndicators(indicators);

            ValidateHandler();
        }
        public async Task Handle()
        {
            _ = await IsSufficient();

            //print

            _n.Notify($"Document {_cmd.Id} printed", PrinterNotificationTypes.Printed, _cmd.Id);
        }

        private void InjectSufficientLevelIndicators(ISufficientLevelIndicator[] indicators)
        {
            foreach (var i in indicators)
            {
                switch (i)
                {
                    case IPaperLevelIndicator pli:
                        _pli = pli;
                        break;
                    case IBlackLevelIndicator bli:
                        _bli = bli;
                        break;
                    case IColorLevelIndicator cli:
                        _cli = cli;
                        break;
                }
            }
        }

        private async Task<bool> IsSufficient()
        {
            if (!await _pli.IsEnough())
                throw new NotEnoughPaperException();

            if (!await _bli.IsEnough())
                throw new NotEnoughBlackException();

            if (_isColor ? !await _cli.IsEnough() : false)
                throw new NotEnoughColorException();

            return true;
        }

        private void ValidateHandler()
        {
            Type t = null;
            switch (t)
            {
                case null when _pli is null:
                    t = typeof(IPaperLevelIndicator);
                    break;
                case null when _bli is null:
                    t = typeof(IBlackLevelIndicator);
                    break;
                case null when _isColor && _cli is null:
                    t = typeof(IColorLevelIndicator);
                    break;
                default:
                    return;
            }
            throw new InvalidConfigurationException(caller: t);
        }
    }
}