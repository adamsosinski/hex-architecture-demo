using System.Threading.Tasks;

namespace Printer.Indicators
{
    public interface ISufficientLevelIndicator
    {
        Task<bool> IsEnough();
    }
}