using System;

namespace Printer.Exceptions
{
    public class InvalidConfigurationException : ApplicationException
    {
        private string _message;
        public override string Message => _message;

        public InvalidConfigurationException(Type caller)
        {
            _message = $"There is no implementation for {caller}";
        }
    }
}