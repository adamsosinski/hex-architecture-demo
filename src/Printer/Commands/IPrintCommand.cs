namespace Printer.Commands
{
    public interface IPrintCommand : ICommand
    {
        byte[] Content { get; }
    }
}