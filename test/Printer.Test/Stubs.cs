using System.Threading.Tasks;

namespace Printer.Test.Stubs
{
    using Indicators;

    internal class NotEnoughPaperLevelStub : IPaperLevelIndicator
    {
        public Task<bool> IsEnough() => Task.FromResult(false);
    }

    internal class EnoughPaperLevelStub : IPaperLevelIndicator
    {
        public Task<bool> IsEnough() => Task.FromResult(true);
    }

    internal class NotEnoughBlackLevelStub : IBlackLevelIndicator
    {
        public Task<bool> IsEnough() => Task.FromResult(false);
    }

    internal class EnoughBlackLevelStub : IBlackLevelIndicator
    {
        public Task<bool> IsEnough() => Task.FromResult(true);
    }

    internal class NotEnoughColorLevelStub : IColorLevelIndicator
    {
        public Task<bool> IsEnough() => Task.FromResult(false);
    }

    internal class EnoughColorLevelStub : IColorLevelIndicator
    {
        public Task<bool> IsEnough() => Task.FromResult(true);
    }
}