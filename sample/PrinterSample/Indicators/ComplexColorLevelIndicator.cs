using System;
using System.Threading.Tasks;

namespace PrinterSample.Indicators
{
    using Printer.Indicators;
    internal class ComplexColorLevelIndicator : IColorLevelIndicator
    {
        private static readonly bool _isCyan;
        private static readonly bool _isMagenta;
        private static readonly bool _isYellow;

        private const double _min = 0.25;

        static ComplexColorLevelIndicator()
        {
            var r = new Random();
            _isCyan = r.NextDouble() > _min;
            _isMagenta = r.NextDouble() > _min;
            _isYellow = r.NextDouble() > _min;
        }

        public Task<bool> IsEnough() => Task.FromResult(_isCyan && _isMagenta && _isYellow);
    }
}