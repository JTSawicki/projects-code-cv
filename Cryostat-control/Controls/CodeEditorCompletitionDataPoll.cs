using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.CodeCompletion;

namespace Piecyk.Controls
{
    public class CodeEditorCompletitionDataPoll
    {
        // Zmienna przechowywująca słowa klucze występujące przed kropką
        public List<string> BeforeDotKeywords { get; private set; }
        // Zmienna przechowywująca wszystkie podpowiedzi dostępne po kropce
        public Dictionary<string, List<MyCompletionData>> AfterDotHintsPoll { get; private set; }

        /// <summary>
        /// Domyślny konstruktor
        /// </summary>
        public CodeEditorCompletitionDataPoll()
        {
            // Tworzenie obiektów kontenerów podpowiedzi
            BeforeDotKeywords = new List<string>();
            AfterDotHintsPoll = new Dictionary<string, List<MyCompletionData>>();
            // Dodawanie podpowiedzi na podstawie szablonu
            foreach (GlobalDataTypes.KeywordCommandTemplate keyword in GlobalDataTypes.CommandPoolTemplate.Keywords)
            {
                BeforeDotKeywords.Add(keyword.Keyword);
                AfterDotHintsPoll.Add(keyword.Keyword, new List<MyCompletionData>());
                foreach (GlobalDataTypes.CommandTemplate command in keyword.SubCommandList)
                {
                    MyCompletionData tmpHint = new MyCompletionData(command.Content, command.CaretBackMovement, command.Description, command.Text);
                    AfterDotHintsPoll[keyword.Keyword].Add(tmpHint);
                }
            }
        }

        public void AddGroupTo(string groupName, IList<ICompletionData> collection)
        {
            foreach(MyCompletionData hint in AfterDotHintsPoll[groupName])
            {
                collection.Add(hint);
            }
        }
    }
}
