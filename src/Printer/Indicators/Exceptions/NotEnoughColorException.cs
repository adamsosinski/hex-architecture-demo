using System;

namespace Printer.Indicators.Exceptions
{
    public class NotEnoughColorException : ApplicationException
    {
        public override string Message => "Not enough color to print";
    }
}