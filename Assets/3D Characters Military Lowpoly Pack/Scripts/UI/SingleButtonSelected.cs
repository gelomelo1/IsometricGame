using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class SingleButtonSelected : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Selected")]
        [SerializeField] Color m_imageColorSelected;
        [SerializeField] Color m_textColorSelected;

        [Header("Highlited")]
        [SerializeField] Color m_imageColorHighlited;
        [SerializeField] Color m_textColorHighlited;

        [Header("Normal")]
        [SerializeField] Color m_imageColorNormal;
        [SerializeField] Color m_textColorNormal;

        public enum buttonState
        {
            Normal,
            Highlited,
            Selected
        }
        buttonState m_buttonState;
        [SerializeField] Image m_image;
        [SerializeField] Image m_frameImage;
        [SerializeField] TextMeshProUGUI m_label;
        bool m_willSetupTMP;
        KeepSingleButtonSelected m_keepSingleButtonSelected;
        int m_index;

        [SerializeField] bool m_setupAsSingleButton = true;

        void Start()
        {
            if (m_setupAsSingleButton)
            {
                m_willSetupTMP = m_label != null;

            }
        }

        public void Setup(KeepSingleButtonSelected _keepSingleButtonSelected, int _index)
        {
            m_keepSingleButtonSelected = _keepSingleButtonSelected;
            m_index = _index;
            GetComponentInChildren<Button>().onClick.AddListener(reportClick);
            m_willSetupTMP = m_label != null;
        }
        void reportClick()
        {
            m_keepSingleButtonSelected.ReportClick(m_index);
            m_buttonState = buttonState.Selected;
            setColorsAccordingToState(m_buttonState);
        }
        public void SetButtonState(buttonState _buttonState)
        {
            m_buttonState = _buttonState;
            setColorsAccordingToState(m_buttonState);
        }
        void setColorsAccordingToState(buttonState _buttonState)
        {
            switch (_buttonState)
            {
                case buttonState.Normal:
                    {
                        m_image.color = m_imageColorNormal;
                        if (m_willSetupTMP) m_label.color = m_textColorNormal;
                        m_frameImage.color = m_imageColorNormal;
                        return;
                    }
                case buttonState.Highlited:
                    {
                        m_image.color = m_imageColorHighlited;
                        if (m_willSetupTMP) m_label.color = m_textColorHighlited;
                        m_frameImage.color = m_imageColorSelected;
                        return;
                    }
                case buttonState.Selected:
                    {
                        m_image.color = m_imageColorSelected;
                        if (m_willSetupTMP) m_label.color = m_textColorSelected;
                        m_frameImage.color = m_imageColorSelected;
                        return;
                    }
            }

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //frame enable
            if (m_buttonState != buttonState.Selected)
            {
                setColorsAccordingToState(buttonState.Highlited);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //frame disable
            if (m_buttonState != buttonState.Selected)
            {
                setColorsAccordingToState(m_buttonState);
            }
        }
    }
}


