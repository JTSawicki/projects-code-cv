using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanychElementow.ViewModels
{
    class AddElementWindowViewModel : MyObservableObject
    {
        private string ChosenClassName_ = "";
        private string ElementCount_ = "";
        private string MainParameterName_ = "";
        private string MainParameterUnit_ = "";
        private string ElementDescription_ = "";
        private string MainParameterValue_ = "";

        public string ChosenClassName
        {
            get { return ChosenClassName_; }
            set
            {
                ChosenClassName_ = value;
                OnPropertyChanged("ChosenClassName");
            }
        }

        public string ElementCount
        {
            get { return ElementCount_; }
            set
            {
                ElementCount_ = value;
                OnPropertyChanged("ElementCount");
            }
        }

        public string MainParameterName
        {
            get { return MainParameterName_; }
            set
            {
                MainParameterName_ = value;
                OnPropertyChanged("MainParameterName");
            }
        }

        public string MainParameterUnit
        {
            get { return MainParameterUnit_; }
            set
            {
                MainParameterUnit_ = value;
                OnPropertyChanged("MainParameterUnit");
            }
        }

        public string ElementDescription
        {
            get { return ElementDescription_; }
            set
            {
                ElementDescription_ = value;
                OnPropertyChanged("ElementDescription");
            }
        }

        public string MainParameterValue
        {
            get { return MainParameterValue_; }
            set
            {
                MainParameterValue_ = value;
                OnPropertyChanged("MainParameterValue");
            }
        }
    }
}
