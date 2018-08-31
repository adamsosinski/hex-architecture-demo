using System;
using System.Threading.Tasks;

namespace Printer.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
        Task Execute();
    }
}