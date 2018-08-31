using System;

namespace Printer.Indicators.Exceptions
{
    public class NotEnoughBlackException : ApplicationException
    {
        public override string Message => "Not enough black to print";
    }
}