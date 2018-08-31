using System;

namespace Printer.Exceptions
{
    public class NoContentToPrintException : ApplicationException
    {
        public override string Message => "No content to print";
    }
}