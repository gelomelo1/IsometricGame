using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class UI_BodyParts : MonoBehaviour
    {
        [SerializeField] GameObject m_character;                                                                // Reference to the character GameObject
        BodyPart[] m_bodyParts;                                                                                 // Array of BodyPart objects representing different parts of the character

        BodyPart m_previousDisplayedBodyPart;
        Dictionary<string, GameObject> m_bodyParts_GameObjects_ByName = new Dictionary<string, GameObject>();   // Dictionary to store GameObject references for each BodyPart by name
        Dictionary<string, List<BodyPart>> m_bodyParts_ByGroupName = new Dictionary<string, List<BodyPart>>();  // Dictionary to group BodyParts by their associated BodyGroup
        [SerializeField] UI_Button_BodyPart m_buttonPrefab;                                                     // Prefab for the UI_Button_BodyPart
        int m_selectedChildIndex;                                                                               // Index of the currently selected child
        UI_Manager m_uI_Manager;                                                                                // Reference to the UI_Manager
        List<UI_Button_BodyPart> m_buttons = new List<UI_Button_BodyPart>();                                    // List to store UI_Button_BodyPart instances
        string m_currentDisplayStringGroupName;                                                                 // String representing the current display string group name
        RectTransform m_rectTransform;                                                                          // RectTransform component of the UI_BodyParts
        GridLayoutGroup m_gridLayoutGroup;                                                                      // GridLayoutGroup component of the UI_BodyParts

        // Enable the UI_BodyParts with initial setup
        public void Enable(UI_Manager _uI_Manager, BodyPart[] _bodyParts) => enable(_uI_Manager, _bodyParts);

        // Initialization and setup of the UI_BodyParts
        void enable(UI_Manager _uI_Manager, BodyPart[] _bodyParts)
        {
            m_uI_Manager = _uI_Manager;
            m_bodyParts = _bodyParts;
            m_rectTransform = GetComponent<RectTransform>();
            m_gridLayoutGroup = GetComponent<GridLayoutGroup>();
            string _groupName;

            // Iterate through each BodyPart for setup
            for (int i = 0; i < m_bodyParts.Length; i++)
            {
                GameObject _bodyPart = GameObject.Find(m_bodyParts[i].m_BodyPartName);

                // Add GameObject reference to the dictionary
                if (_bodyPart != null)
                {
                    if (!m_bodyParts_GameObjects_ByName.ContainsKey(m_bodyParts[i].m_BodyPartName))
                    {
                        m_bodyParts_GameObjects_ByName.Add(m_bodyParts[i].m_BodyPartName, _bodyPart);
                    }
                }

                _groupName = m_bodyParts[i].m_BodyGroup.ToString();

                // Group BodyParts by their associated BodyGroup
                if (!m_bodyParts_ByGroupName.ContainsKey(_groupName))
                {
                    m_bodyParts_ByGroupName.Add(_groupName, new List<BodyPart>());
                }
                m_bodyParts_ByGroupName[_groupName].Add(m_bodyParts[i]);
            }

            // Display the first body part of each group
            List<string> _all_BodyPartTypeGroups_Keys = new List<string>(m_bodyParts_ByGroupName.Keys);
            for (int i = 0; i < _all_BodyPartTypeGroups_Keys.Count; i++)
            {
                SetupPart(
                    m_bodyParts_ByGroupName[_all_BodyPartTypeGroups_Keys[i]][0],
                    0,
                    true);
            }

        }

        // Set up the display for a specific BodyPart
        public void SetupPart(BodyPart _bodyPart, int _selectedChildIndex, bool _initialLoad = false)
        {
            GameObject _bodyPartGameObject = null;

            // Retrieve GameObject reference from the dictionary
            if (m_bodyParts_GameObjects_ByName.ContainsKey(_bodyPart.m_BodyPartName))
            {
                // Disable all children of the BodyPart
                _bodyPartGameObject = m_bodyParts_GameObjects_ByName[_bodyPart.m_BodyPartName];
                for (int i = 0; i < _bodyPart.m_BodyPart_Parent.transform.childCount; i++)
                {
                    _bodyPart.m_BodyPart_Parent.transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            // Hide addons if the BodyPart has addons

            if (!_initialLoad && m_bodyParts_ByGroupName[m_previousDisplayedBodyPart.m_BodyGroup.ToString()][m_selectedChildIndex].m_HadAddons && m_previousDisplayedBodyPart != null)
            {
                for (int i = 0; i < m_bodyParts_ByGroupName[m_previousDisplayedBodyPart.m_BodyGroup.ToString()][m_selectedChildIndex].m_Addons.Length; i++)
                {
                    m_uI_Manager.HideAll_Addons_GameObjects(m_bodyParts_ByGroupName[m_previousDisplayedBodyPart.m_BodyGroup.ToString()][m_selectedChildIndex].m_Addons);
                }
            }

            m_previousDisplayedBodyPart = _bodyPart;


            // Enable the selected part
            if (_bodyPart.m_BodyPartName != string.Empty)
            {
                enableSelectedPart(_bodyPartGameObject);
            }
            else
            {
                foreach (Transform child in _bodyPart.m_BodyPart_Parent.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            _bodyPart.m_BodyPart_Parent.SetActive(true);

            m_selectedChildIndex = _selectedChildIndex;

            if (!_initialLoad)
            {
                // Update the selected index and display addons panel
                m_uI_Manager.RememberChosenBodyPart_InsideBodyGroup(_selectedChildIndex);
                m_uI_Manager.Display_AddonsPanel(_bodyPart, _bodyPart.m_HadAddons);
            }

            for (int i = 0; i < m_buttons.Count; i++)
            {
                // Set button state based on the selected index
                m_buttons[i].GetComponent<SingleButtonSelected>().SetButtonState(i == _selectedChildIndex ? SingleButtonSelected.buttonState.Selected : SingleButtonSelected.buttonState.Normal);
            }
        }

        // Enable the selected part by setting its GameObject active
        void enableSelectedPart(GameObject _bodyPart)
        {
            _bodyPart.SetActive(true);
        }

        // Display buttons for a specific BodyPart group
        public void DisplayButtons(string _stringGroupName, int _lastselectedChildIndex)
        {
            if (m_currentDisplayStringGroupName == _stringGroupName) return;
            StopAllCoroutines();
            //aasaign previous selected bodyPart...
            BodyPart _bodyPart = m_bodyParts_ByGroupName[_stringGroupName][_lastselectedChildIndex];
            m_previousDisplayedBodyPart = _bodyPart;



            // Calculate the spacing and adjust the size of the RectTransform
            float _spacingSumm = Mathf.CeilToInt(m_bodyParts_ByGroupName[_stringGroupName].Count / 4f) * m_gridLayoutGroup.spacing.y + m_gridLayoutGroup.padding.top;
            m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x, Mathf.CeilToInt(m_bodyParts_ByGroupName[_stringGroupName].Count / 4f) * 150 + _spacingSumm);

            m_currentDisplayStringGroupName = _stringGroupName;
            foreach (Transform child in this.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            m_buttons.Clear();
            m_selectedChildIndex = _lastselectedChildIndex;

            StartCoroutine(DisplayButtonsCoroutine(_stringGroupName, _lastselectedChildIndex));
        }

        // Coroutine to display buttons for a specific BodyPart group
        IEnumerator DisplayButtonsCoroutine(string _stringGroupName, int _lastSelectedChildIndex)
        {
            BodyPart _bodyPart = default;

            if (m_bodyParts_ByGroupName.ContainsKey(_stringGroupName))
            {
                for (int i = 0; i < m_bodyParts_ByGroupName[_stringGroupName].Count; i++)
                {
                    _bodyPart = m_bodyParts_ByGroupName[_stringGroupName][i];
                    UI_Button_BodyPart _uI_Button = Instantiate(m_buttonPrefab, this.transform);
                    _uI_Button.Setup(
                        this,
                        _bodyPart,
                        _bodyPart.m_BodyPart_Parent,
                        i);

                    m_buttons.Add(_uI_Button);

                    if (i == _lastSelectedChildIndex)
                    {
                        _uI_Button.GetComponent<SingleButtonSelected>().SetButtonState(SingleButtonSelected.buttonState.Selected);
                        m_uI_Manager.Display_AddonsPanel(_bodyPart, _bodyPart.m_HadAddons);
                        m_previousDisplayedBodyPart = _bodyPart;

                    }
                    else
                    {
                        _uI_Button.GetComponent<SingleButtonSelected>().SetButtonState(SingleButtonSelected.buttonState.Normal);
                    }

                    yield return new WaitForSecondsRealtime(.01f);
                }
            }
            yield return null;
        }
    }
}

