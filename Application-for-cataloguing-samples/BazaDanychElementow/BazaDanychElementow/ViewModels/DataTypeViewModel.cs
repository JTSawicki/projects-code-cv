using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanychElementow.ViewModels
{
    public class DataTypeViewModel : MyObservableObject
    {
        // Wiązanie danych dla Combobox
        private List<string> dataTypes_;

        public DataTypeViewModel()
        {
            dataTypes_ = new List<string>()
            {
                "int",
                "uint",
                "string",
                "float"
            };
        }

        public List<string> dataTypes
        {
            get { return dataTypes_; }
            set
            {
                dataTypes_ = value;
                OnPropertyChanged("dataTypes");
            }
        }
    }
}
