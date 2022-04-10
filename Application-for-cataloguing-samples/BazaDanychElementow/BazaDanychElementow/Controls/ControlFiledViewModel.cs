using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BazaDanychElementow.ViewModels;

namespace BazaDanychElementow.Controls
{
    public class ControlFiledViewModel : ViewModels.MyObservableObject
    {
        private string ParameterInfo_ = "";
        private string Name_ = "";
        private string Type_ = "";
        private string Unit_ = "";

        public DataTypeViewModel dataTypeViewModel { get; private set; }

        public ControlFiledViewModel()
        {
            dataTypeViewModel = new DataTypeViewModel();
        }

        public string Name
        {
            get { return Name_; }
            set
            {
                Name_ = value;
                OnPropertyChanged("Name");
            }
        }

        public string Type
        {
            get { return Type_; }
            set
            {
                Type_ = value;
                OnPropertyChanged("Type");
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

        public string ParameterInfo
        {
            get { return ParameterInfo_; }
            set
            {
                ParameterInfo_ = value;
                OnPropertyChanged("ParameterInfo");
            }
        }
    }
}
