using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanychElementow.Controls
{
    public class ParameterInputControlViewModel : ViewModels.MyObservableObject
    {
        private string Name_ = "";
        private string Unit_ = "";
        private string Value_ = "";

        public string Name
        {
            get { return Name_; }
            set
            {
                Name_ = value;
                OnPropertyChanged("Name");
            }
        }

        public string Unit
        {
            get { return Unit_; }
            set
            {
                Unit_ = value;
                OnPropertyChanged("Unit");
            }
        }

        public string Value
        {
            get { return Value_; }
            set
            {
                Value_ = value;
                OnPropertyChanged("Value");
            }
        }
    }
}
