using System;

namespace LabServices.Exceptions
{
    /// <summary>
    /// Błąd wysłania zbyt dużej ilości danych do funkcji
    /// </summary>
    [Serializable]
    public class ToMuchDataException : Exception
    {
        public ToMuchDataException() { }
        public ToMuchDataException(string message) : base(message) { }
        public ToMuchDataException(string message, Exception innerException) : base(message, innerException) { }
    }
}
