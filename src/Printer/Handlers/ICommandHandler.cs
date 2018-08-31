using System.Threading.Tasks;

namespace Printer.Handlers
{
    internal interface ICommandHandler
    {
        Task Handle();
    }
}