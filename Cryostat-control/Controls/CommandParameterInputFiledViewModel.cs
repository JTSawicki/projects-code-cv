using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piecyk.Controls
{
    public class CommandParameterInputFiledViewModel : ViewModels.MyObservableObject
    {
        private string InfoLabelText_ = "";
        private string TextBoxHint_ = "";
        private string TextBoxText_ = "";

        public string InfoLabelText
        {
            get { return InfoLabelText_; }
            set
            {
                InfoLabelText_ = value;
                OnPropertyChanged("InfoLabelText");
            }
        }

        public string TextBoxHint
        {
            get { return TextBoxHint_; }
            set
            {
                TextBoxHint_ = value;
                OnPropertyChanged("TextBoxHint");
            }
        }
        public string TextBoxText
        {
            get { return TextBoxText_; }
            set
            {
                TextBoxText_ = value;
                OnPropertyChanged("TextBoxText");
            }
        }
    }
}
