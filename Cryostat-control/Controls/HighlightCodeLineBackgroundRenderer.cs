using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit;
using System.Windows.Media;
using System.Windows;

namespace Piecyk.Controls
{
    class HighlightCodeLineBackgroundRenderer : IBackgroundRenderer
    {
        public static int LineToHighlight = 1; //< Numer podświetlanej lini. Inicjalizacja 1 w celu uniknięcia błędów. Numeracja lini w AvalonEdit idzie od 1.
        public static bool IsHighlightActive = false; //< Przełącznik podświetlania

        private TextEditor _editor;

        public HighlightCodeLineBackgroundRenderer(TextEditor editor)
        {
            _editor = editor;
        }
        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;
            textView.EnsureVisualLines();
            
            if(IsHighlightActive)
            {
                if (LineToHighlight < _editor.Document.LineCount && LineToHighlight >= 0)
                {
                    var currentLine = _editor.Document.GetLineByNumber(LineToHighlight);
                    foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
                    {
                        drawingContext.DrawRectangle(
                        // Jest to paleta kolorów RGB + A(kanał alpha < == bardziej przeźroczyste)
                        new SolidColorBrush(Color.FromArgb(0x30, 0xE2, 0x14, 0xC0)), null,
                        new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
                    }
                }
            }
        }
    }
}
