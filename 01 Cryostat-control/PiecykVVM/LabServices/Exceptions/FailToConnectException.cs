using System;

namespace LabServices.Exceptions
{
    /// <summary>
    /// Błąd nieudanej próby połączenia z urządzeniem
    /// </summary>
    [Serializable]
    public class FailToConnectException : Exception
    {
        public FailToConnectException() { }
        public FailToConnectException(string message) : base(message) { }
        public FailToConnectException(string message, Exception innerException) : base(message, innerException) { }
    }
}
