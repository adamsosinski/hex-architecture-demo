using System;
using System.Threading.Tasks;

namespace PrinterSample.Indicators
{
    using Printer.Indicators;

    internal class SimpleColorLevelIndicator : IColorLevelIndicator
    {
        private Random _r = new Random();

        public Task<bool> IsEnough() => Task.FromResult(_r.NextDouble() > 0.2);
    }
}