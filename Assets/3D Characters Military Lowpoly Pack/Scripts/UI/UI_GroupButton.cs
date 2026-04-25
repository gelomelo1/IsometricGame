using UnityEngine;
using TMPro;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    /*  
    Short script handling buttons actions of buttons created for BodyPartGroups
    */


    public class UI_GroupButton : MonoBehaviour
    {
        UI_Manager m_uI_Manager;
        string m_string;
        [SerializeField] TextMeshProUGUI m_label;

        public void Setup(UI_Manager _uI_Manager, string _string)
        {
            m_uI_Manager = _uI_Manager;
            m_string = _string;
            string processedString = ProcessString(m_string);
            m_label.SetText(processedString);
        }

        private string ProcessString(string inputString)
        {
            // Check for uppercase letters and split the internal string with spaces
            char[] charArray = inputString.ToCharArray();
            for (int i = 1; i < charArray.Length; i++)
            {
                if (char.IsUpper(charArray[i]))
                {
                    // Insert space before the uppercase letter
                    inputString = inputString.Insert(i, " ");
                    i++; // Increment i to skip the added space
                }
            }

            return inputString;
        }

        // Void is Body Part group Button click action
        public void WasClick()
        {
            m_uI_Manager.WasClick(m_string);
        }
    }

}

