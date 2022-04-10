using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Piecyk.Controls
{
    /// <summary>
	/// Implements AvalonEdit ICompletionData interface to provide the entries in the completion drop down.
	/// </summary>
    public class MyCompletionData : ICompletionData
    {
        // Wymagane pola z odziedziczonego interfejsu
        public string Text { get; private set; } //< Tekst wstawiany
        public System.Windows.Media.ImageSource Image { get { return null; } } //< Ikona podpowiedzi
        public object Content { get; private set; } //< Tekst wyświetlany
        public object Description { get; private set; } //< Tekst podpowiedzi
        public double Priority { get { return 0; } }

        // Inne pola
        public int CaretBackMovement { get; private set; } //< O ile cofnąć karetkę po wstawieniu podpowiedzi
        public ushort CharsToDelete { get; private set; } //< Ile znaków usunąć z początku wstawianego tekstu

        /// <summary>
        /// Podstawowy konstruktor obiektu podpowiedzi do kodu
        /// </summary>
        /// <param name="text">Wstawiany tekst</param>
        /// <param name="caretBackMovement">O ile cofnąć karetkę po wstawieniu podpowiedzi</param>
        /// <param name="description">Opis dodawany do podpowiedzi</param>
        /// <param name="content">Wyświetlany tekst(jeżeli inny niż wstawiany)</param>
        /// <param name="charsToDelete">Ile znaków usunąć z początku wstawianego tekstu(znaki już napisane przez użytkownika)</param>
        public MyCompletionData(string hintContent, int caretBackMovement, string hintDescription = "default", string hintText = "default", ushort charsToDelete = 0)
        {
            Content = hintContent;
            CaretBackMovement = caretBackMovement;
            CharsToDelete = charsToDelete;
            if (hintDescription.Equals("default"))
            {
                Description = "Description for " + hintContent;
            } else
            {
                Description = hintDescription;
            }

            if(hintText.Equals("default"))
            {
                Text = hintContent;
            } else
            {
                Text = hintText;
            }
        }

        /// <summary>
        /// Funkcja odpowiadająca za podmianę tekstu
        /// </summary>
        /// <param name="textArea"></param>
        /// <param name="completionSegment"></param>
        /// <param name="insertionRequestEventArgs"></param>
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            // textArea.Document.Replace(completionSegment, this.Text);
            textArea.Document.Replace(completionSegment, Text.Substring(CharsToDelete, Text.Length - CharsToDelete));
            textArea.Caret.Offset = textArea.Caret.Offset - CaretBackMovement;
        }
    }
}
