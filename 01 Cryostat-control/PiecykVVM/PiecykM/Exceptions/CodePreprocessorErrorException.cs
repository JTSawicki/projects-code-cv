using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiecykM.Exceptions
{
    /// <summary>
    /// Błąd preprocesora kodu programu.
    /// W wiadomości jest zawarta informacja do wyświetlenia użytkownikowi.
    /// </summary>
    public class CodePreprocessorErrorException : Exception
    {
        public CodePreprocessorErrorException() { }
        public CodePreprocessorErrorException(string message) : base(message) { }
        public CodePreprocessorErrorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
