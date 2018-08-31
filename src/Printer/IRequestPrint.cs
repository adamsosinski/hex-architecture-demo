using System.Threading.Tasks;

namespace Printer
{
    public interface IRequestPrint
    {
        Task PrintBlack(byte[] content);
        Task PrintColor(byte[] content);
    }
}