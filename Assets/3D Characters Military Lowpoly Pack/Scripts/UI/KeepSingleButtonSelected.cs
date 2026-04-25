using UnityEngine;


namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class KeepSingleButtonSelected : MonoBehaviour
    {
        SingleButtonSelected[] m_singleButtonSelecteds;
        void enable()
        {
            m_singleButtonSelecteds = GetComponentsInChildren<SingleButtonSelected>();
            for (int i = 0; i < m_singleButtonSelecteds.Length; i++)
            {
                m_singleButtonSelecteds[i].Setup(this, i);
                m_singleButtonSelecteds[i].SetButtonState(SingleButtonSelected.buttonState.Normal);
            }
        }
        public void Enable() => enable();

        public void ReportClick(int _index)
        {
            for (int i = 0; i < m_singleButtonSelecteds.Length; i++)
            {
                if (i != _index) m_singleButtonSelecteds[i].SetButtonState(SingleButtonSelected.buttonState.Normal);
            }
        }
    }

}

