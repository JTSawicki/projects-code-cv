using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanychElementow.ViewModels
{
    class AddElementClassWindowViewModel : MyObservableObject
    {
        // Wiązanie danych dla Combobox liczby parametrów
        private List<string> parameterCountList_;
        private string parameterCount_ = "0";

        // Wiązania danych dla pól wprowadzania danych
        private string MainParameterName_ = "";
        private string MainParameterType_ = "";
        private string MainParameterUnit_ = "";

        private string ClassName_ = "";
        private bool HasMasterClass_ = false;

        public AddElementClassWindowViewModel()
        {
            parameterCountList = new List<string>()
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10"
            };
        }

        public List<string> parameterCountList
        {
            get { return parameterCountList_; }
            set
            {
                parameterCountList_ = value;
                OnPropertyChanged("parameterCountList");
            }
        }

        public string parameterCount
        {
            get
            {
                if (string.IsNullOrEmpty(parameterCount_))
                    return "0";
                else
                    return parameterCount_;
            }
            set
            {
                parameterCount_ = value;
                OnPropertyChanged("parameterCount");
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

        public string MainParameterType
        {
            get { return MainParameterType_; }
            set
            {
                MainParameterType_ = value;
                OnPropertyChanged("MainParameterType");
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

        public string ClassName
        {
            get { return ClassName_; }
            set
            {
                ClassName_ = value;
                OnPropertyChanged("ClassName");
            }
        }

        public bool HasMasterClass
        {
            get { return HasMasterClass_; }
            set
            {
                HasMasterClass_ = value;
                OnPropertyChanged("HasMasterClass");
            }
        }
    }
}
