using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Text;

namespace Piecyk.Controls
{
    /// <summary>
    /// Klasa obiektu obserwowalnego dla kontrolki CodeEditor
    /// </summary>
    public class CodeEditorViewModel : ViewModels.MyObservableObject
    {
        // Zmienne dla obiektów ComboBox wyboru parametrów czcionki
        private List<string> fontSizes_;
        private List<string> fontTypes_;
        // Zmienne dla wybranych parametrów
        private string choosenFontSize_ = "";
        private string choosenFontType_ = "";
        // Zmienne dla ToggleButton
        private bool VisibleBlankCharacters_ = true;
        private bool TextWraping_ = true;
        private bool LineNumering_ = true;

        public CodeEditorViewModel()
        {
            // Generowanie listy rozmiarów czcionek
            fontSizes_ = new List<string>(new string[] { "10", "12", "14", "16", "20", "24", "28", "30" });
            // Generowanie listy rodzajów czcionek
            fontTypes_ = new List<string>(200);
            using (InstalledFontCollection col = new InstalledFontCollection())
            {
                foreach (FontFamily fa in col.Families)
                {
                    fontTypes_.Add(fa.Name);
                }
            }
            // Ustawianie parametrów wybranych na wartości domyślne
            choosenFontSize_ = "14";
            choosenFontType_ = "Consolas";
        }

        public List<string> fontSizes
        {
            get { return fontSizes_;  }
            set
            {
                fontSizes_ = value;
                OnPropertyChanged("fontSizes");
            }
        }
        public List<string> fontTypes
        {
            get { return fontTypes_; }
            set
            {
                fontTypes_ = value;
                OnPropertyChanged("fontTypes");
            }
        }
        public string choosenFontSize
        {
            get { return choosenFontSize_; }
            set
            {
                choosenFontSize_ = value;
                OnPropertyChanged("choosenFontSize");
            }
        }
        public string choosenFontType
        {
            get { return choosenFontType_; }
            set
            {
                choosenFontType_ = value;
                OnPropertyChanged("choosenFontType");
            }
        }

        public bool VisibleBlankCharacters
        {
            get { return VisibleBlankCharacters_; }
            set
            {
                VisibleBlankCharacters_ = value;
                OnPropertyChanged("VisibleBlankCharacters");
            }
        }
        public bool TextWraping
        {
            get { return TextWraping_; }
            set
            {
                TextWraping_ = value;
                OnPropertyChanged("TextWraping");
            }
        }
        public bool LineNumering
        {
            get { return LineNumering_; }
            set
            {
                LineNumering_ = value;
                OnPropertyChanged("LineNumering");
            }
        }
    }
}
