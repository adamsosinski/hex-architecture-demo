using System;

namespace Printer.Indicators.Exceptions
{
    public class NotEnoughPaperException : ApplicationException
    {
        public override string Message => "Not enough paper to print";
    }
}