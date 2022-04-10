using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Microsoft.Win32;

namespace Piecyk.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy CodeEditor.xaml
    /// </summary>
    public partial class CodeEditor : UserControl
    {
        CodeEditorViewModel viewModel = new CodeEditorViewModel();
        private string CurrentFileName = ""; //< Zmienna przechowywująca nazwę pliku z którego został wzięty skrypt

        // Control DependencyPropertys
        public string InitialCodePath
        {
            get { return GetValue(InitialCodePathProperity) as string; }
            set { SetValue(InitialCodePathProperity, value); }
        }
        public static readonly DependencyProperty InitialCodePathProperity = DependencyProperty.Register(nameof(InitialCodePath), typeof(string), typeof(CodeEditor));
        public string HighlightingPatternPath
        {
            get { return GetValue(HighlightingPatternPathProperty) as string;  }
            set { SetValue(HighlightingPatternPathProperty, value);  }
        }
        public static readonly DependencyProperty HighlightingPatternPathProperty = DependencyProperty.Register(nameof(HighlightingPatternPath), typeof(string), typeof(CodeEditor));
        public int CurrentlyExecutedLineOfCode
        {
            get { return (int) GetValue(CurrentlyExecutedLineOfCodeProperty); }
            set { SetValue(CurrentlyExecutedLineOfCodeProperty, value); }
        }
        public static readonly DependencyProperty CurrentlyExecutedLineOfCodeProperty = DependencyProperty.Register(nameof(CurrentlyExecutedLineOfCode), typeof(int), typeof(CodeEditor),
            new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnCurrentlyExecutedLineOfCode_Changed)));

        public CodeEditor()
        {
            InitializeComponent();
            // Inicjowanie kontekstu danych dla wiązań danych
            this.DataContext = viewModel;
            // Dodawanie łącza do funkcji kończącej konstrukcję kontrolki po zakończeniu wczytywania parametrów
            this.Loaded += UserControl_Loaded;

            // Inicjalizacja niezmiennych przez urzytkownika ustawień edytora
            textEditor.Options.WordWrapIndentation = 15;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            SetLineHighlightPattern("currentLine");

            // Inicjalizacja zmienialnych przez urzytkownika ustawień
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.FontSize = 14.0;
            textEditor.ShowLineNumbers = true;
            textEditor.Options.ShowEndOfLine = true;
            textEditor.Options.ShowTabs = true;
            textEditor.WordWrap = true;
        }

        /// <summary>
        /// Funkcja uruchamiana po załadowaniu parametrów DependencyProperity do kontrolki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Wczytywanie pliku inicjalizującego pole tekstowe kodu i sprawdzenie czy nie została podana pusta ścieżka do tego pliku(brak podania argumentu)
            string initialCode = "";
            Console.WriteLine("InitialCodePath:  \"" + InitialCodePath + "\"");
            if (InitialCodePath != null) initialCode = File.ReadAllText(InitialCodePath, Encoding.UTF8);
            // Ustawianie początkowego tekstu
            textEditor.Text = initialCode;
            // Zmiana położenia kursora na koniec pliku
            textEditor.CaretOffset = initialCode.Length;

            // Wczytywanie pliku ustawień kolorowania kodu
            using (XmlTextReader reader = new XmlTextReader(HighlightingPatternPath))
            {
                textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        /// <summary>
        /// Funkcja wywoływana przy zmianie właściwości CurrentlyExecutedLineOfCode
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnCurrentlyExecutedLineOfCode_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Brak blokady edytowa. Procesor kodu jet nieaktywny.
            CodeEditor codeEditor = (CodeEditor)d;
            codeEditor.setTextEditorLock((int)e.NewValue != 0); // Wartość 0 oznacza brak działania procesora kodu

            // Zmiana metody podświetlania
            if((int)e.NewValue == 0)
            {
                codeEditor.SetLineHighlightPattern("currentLine");
            }
            else
            {
                codeEditor.SetLineHighlightPattern("setLineNumer");
            }

            // Wywoływania podświetlenlini kodu
            HighlightCodeLineBackgroundRenderer.LineToHighlight = (int)e.NewValue;
            codeEditor.textEditor.TextArea.TextView.InvalidateLayer(ICSharpCode.AvalonEdit.Rendering.KnownLayer.Background);
        }

        // Funkcje obsługujące zdażenia ToggleButton
        private void BlankCharactersTogglebuttonClick(object sender, RoutedEventArgs e)
        {
            if(viewModel.VisibleBlankCharacters)
            {
                textEditor.Options.ShowEndOfLine = true;
                textEditor.Options.ShowTabs = true;
            } else
            {
                textEditor.Options.ShowEndOfLine = false;
                textEditor.Options.ShowTabs = false;
            }
        }
        private void TextWrapTogglebuttonClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(textEditor.CaretOffset);
            if(viewModel.TextWraping)
            {
                textEditor.WordWrap = true;
            } else
            {
                textEditor.WordWrap = false;
            }
        }
        private void ShowLineNumeringTogglebuttonClick(object sender, RoutedEventArgs e)
        {
            if(viewModel.LineNumering)
            {
                textEditor.ShowLineNumbers = true;
            } else
            {
                textEditor.ShowLineNumbers = false;
            }
        }

        // Funkcje obsługujące zdarzenia zmiany wyboru w ComboBox w ustawieniach czcionki
        private void FontSizeSelectionChanged(object sender, EventArgs e)
        {
            double newFontSize = double.Parse(viewModel.choosenFontSize);
            if (newFontSize != textEditor.FontSize) textEditor.FontSize = newFontSize;
        }
        private void FontTypeSelectionChanged(object sender, EventArgs e)
        {
            FontFamily newFontType = new FontFamily(viewModel.choosenFontType);
            Console.WriteLine(newFontType.ToString());
            Console.WriteLine(newFontType.Equals(textEditor.FontFamily));
            if (!newFontType.Equals(textEditor.FontFamily)) textEditor.FontFamily = newFontType;
        }

        // Funkcje przycisków obsługi plików
        private const string DefaultExt = ".furnace";
        private const string DefaultExtFilter = "Furnace Script files (*.furnace)|*.furnace|All files (*.*)|*.*";
        private void SaveToFileButton(object sender, RoutedEventArgs e)
        {
            if (CurrentFileName.Equals(""))
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = DefaultExt;
                dlg.Filter = DefaultExtFilter;
                if (dlg.ShowDialog() ?? false)
                {
                    CurrentFileName = dlg.FileName;
                }
            }
            textEditor.Save(CurrentFileName);
        }
        private void SaveAsToFileButton(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = DefaultExt;
            dlg.Filter = DefaultExtFilter;
            if (dlg.ShowDialog() ?? false)
            {
                CurrentFileName = dlg.FileName;
            }
            textEditor.Save(CurrentFileName);
        }
        private void LoadFromFileButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = DefaultExtFilter;
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                textEditor.Load(dlg.FileName);
                CurrentFileName = dlg.FileName;
            }
        }

        // Funkcja podpowiadania kodu po naciśnięciu kropki
        private CompletionWindow completionWindow;
        private CodeEditorCompletitionDataPoll HintPool = new CodeEditorCompletitionDataPoll();
        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            // Pobieranie obecnej lini tekstu
            int caretOffset = textEditor.CaretOffset;
            ICSharpCode.AvalonEdit.Document.DocumentLine line = textEditor.Document.GetLineByOffset(caretOffset);
            string textLine = textEditor.Document.GetText(line.Offset, line.Length);
            // Tworzenie okna podpowiedzi
            completionWindow = new CompletionWindow(textEditor.TextArea);
            // Tworzenie tablicy podpowiedzi
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            // Usuwanie białych znaków
            textLine = textLine.Trim();

            // Sprawdzanie czy użytkownik nacisną kropkę
            if (e.Text == ".")
            {
                // Znajdywanie odpowiedniej listy podpowiedzi
                foreach(string keyword in HintPool.BeforeDotKeywords)
                {
                    if(textLine.Contains(keyword))
                    {
                        HintPool.AddGroupTo(keyword, data);
                    }
                }
            }
            else
            {
                // Generowanie podpowiedzi
                foreach (string keyword in HintPool.BeforeDotKeywords)
                {
                    /*
                     * Sprawdzenie trzech warunków podania podpowiedzi słowa kluczowego:
                     *      1. Czy wprowadzony znak nie był znakiem pustym np. Enter, Tab, ... . Operacja wykonana przez sprawdzenie długości wczytanej lini.
                     *      2. Czy użytkownik nie napisał już całego słowa kluczowego.
                     *      3. Czy wprowadzany tekst jest początkiem słowa kluczowego.
                     *      
                     */
                    if (textLine.Length != 0 && keyword.Length > textLine.Length && keyword.Substring(0, textLine.Length).Equals(textLine))
                    {
                        data.Add(new MyCompletionData(keyword, 0, charsToDelete: (ushort) textLine.Length));
                    }
                }
            }

            // Wyświetlenie okna podpowiedzi jeżeli lista podpowiedzi nie jest pusta
            if (data.Count > 0)
            {
                completionWindow.Show();
                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            ;
        }

        // Zmienne pomocnicze dla funkcji SetHighlightPattern(umiszczone poza funkcją ponieważ w C# nie ma zmiennych statycznych funkcji)
        private bool FirstFunctionRunSetHighlightPattern = true;
        private HighlightLineBackgroundRenderer CurrentLineHighlightPattern = null;
        /// <summary>
        /// Funkcja ustawiająca sposób podświetlania lini.
        /// 
        /// </summary>
        /// <param name="pattern">Metoda podświetlania: {"none", "currentLine", "setLineNumer"}</param>
        public void SetLineHighlightPattern(string pattern)
        {
            // Inicjowanie funkcjonalności przy pierwszym wywołaniu
            if(FirstFunctionRunSetHighlightPattern)
            {
                // Ustawianie zdarzenia na potrzeby podkreślania lini na kursorze
                textEditor.TextArea.Caret.PositionChanged += (sender, e) => textEditor.TextArea.TextView.InvalidateLayer(ICSharpCode.AvalonEdit.Rendering.KnownLayer.Background);
                FirstFunctionRunSetHighlightPattern = false;

                // Tworzenie modeli podkreślania
                textEditor.TextArea.TextView.BackgroundRenderers.Add(new HighlightLineBackgroundRenderer(textEditor));
                textEditor.TextArea.TextView.BackgroundRenderers.Add(new HighlightCodeLineBackgroundRenderer(textEditor));
            }

            // Ustawianie nowego modelu podkreślania
            if(pattern.Equals("currentLine"))
            {
                HighlightLineBackgroundRenderer.IsHighlightActive = true;
                HighlightCodeLineBackgroundRenderer.IsHighlightActive = false;
            }
            else if(pattern.Equals("setLineNumer"))
            {
                HighlightLineBackgroundRenderer.IsHighlightActive = false;
                HighlightCodeLineBackgroundRenderer.IsHighlightActive = true;
            }
            // Brak ustawienia algorytmu podkreślania w przypadku gdy pattern == "none" albo nieznanemu wzorcowi
        }

        // Funkcje interfejsu dla obiektów uzyskujących dostęp do edytora z zewnątrz
        public void insertTextOnCaret(string text_)
        {
            textEditor.Document.Insert(textEditor.CaretOffset, text_);
        }
        public void insertText(string text_, int offset_)
        {
            textEditor.Document.Insert(offset_, text_);
        }
        public string getText()
        {
            return textEditor.Text;
        }
        public void setText(string text_)
        {
            textEditor.Text = text_;
        }
        public int getCaretOffset()
        {
            return textEditor.CaretOffset;
        }
        public void setCaretOffset(int offest_)
        {
            textEditor.CaretOffset = offest_;
        }
        public int getCurrentLine()
        {
            int caretOffset = textEditor.CaretOffset;
            ICSharpCode.AvalonEdit.Document.DocumentLine line = textEditor.Document.GetLineByOffset(caretOffset);
            return line.Offset;
        }
        public string getCurrentLineText()
        {
            int caretOffset = textEditor.CaretOffset;
            ICSharpCode.AvalonEdit.Document.DocumentLine line = textEditor.Document.GetLineByOffset(caretOffset);
            return textEditor.Document.GetText(line.Offset, line.Length);
        }

        /// <summary>
        /// Funkcja ustawia blokadę edytora i pokazuje indykator działania.
        /// </summary>
        /// <param name="isEditorLocked">Czy zablokować okno.</param>
        public void setTextEditorLock(bool isEditorLocked)
        {
            if (isEditorLocked == true)
            {
                textEditor.IsReadOnly = true;
                codeExecutionProgressBar.Visibility = Visibility.Visible;
            }
            else
            {
                textEditor.IsReadOnly = false;
                codeExecutionProgressBar.Visibility = Visibility.Hidden;
            }
        }
    }
}
