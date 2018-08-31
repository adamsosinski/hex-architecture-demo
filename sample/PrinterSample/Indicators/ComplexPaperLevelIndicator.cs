using System;
using System.Threading.Tasks;

namespace PrinterSample.Indicators
{
    using Printer.Indicators;

    internal class ComplexPaperLevelIndicator : IPaperLevelIndicator
    {
        private readonly int _p;
        private static readonly int _available;

        static ComplexPaperLevelIndicator()
        {
            var r = new Random();
            _available = r.Next(0, 500);
        }

        public ComplexPaperLevelIndicator(int pages)
        {
            _p = pages;
        }

        public Task<bool> IsEnough() => Task.FromResult(_available > _p);
    }
}
