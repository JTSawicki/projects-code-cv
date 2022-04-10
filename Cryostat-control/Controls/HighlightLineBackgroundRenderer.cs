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
    public class HighlightLineBackgroundRenderer : IBackgroundRenderer
    {
        public static bool IsHighlightActive = false; //< Przełącznik podświetlania
        private TextEditor _editor;
        
        public HighlightLineBackgroundRenderer(TextEditor editor)
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
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            if(IsHighlightActive)
                foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
                {
                    drawingContext.DrawRectangle(
                    // Jest to paleta kolorów RGB + A(kanał alpha < == bardziej przeźroczyste)
                    new SolidColorBrush(Color.FromArgb(0x20, 0, 0xC0, 0xC0)), null,
                    new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
                }
        }
    }
}
