using System.Collections.Generic;
using UnityEngine;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    /*  
        Script will take an array of BodyPart and establish UI for it:
        (+) at start: will scan for all addons and assign GameObject references inside them for further use;
        (+) at start: will find parent GameObject for each BodyPartGroup, references will be stored in dictionary m_bodyPartGroup_GameObjects_ByName
        (+) at start: will scan all BodyParts and assign GameObject references inside them for further use;
        (+) at start: will create buttons for each BodyPartGroup
        (+) at start: will enable KeepSingleButtonSelected
        (+) at start: will enable UI_BodyParts
     */

    public class UI_Manager : MonoBehaviour
    {
        [SerializeField] Transform m_character;

        [Header("BodyGroups")]
        [SerializeField] UI_GroupButton m_uI_GroupButtonPrefab;                                                     // Prefab for UI Group Buttons
        [SerializeField] Transform m_buttonContainer;                                                               // Container for UI Group Buttons

        [Header("BodyParts")]
        [SerializeField] BodyPart[] m_bodyParts;                                                                    // Array of BodyParts
        [SerializeField] UI_BodyParts m_ui_bodyParts;                                                               // UI Body Parts component

        [Header("Addons")]
        [SerializeField] UI_Addons m_ui_Addons;                                                                     // UI Addons component
        [SerializeField] GameObject m_addonsPanel;                                                                  // Panel for displaying addons

        [Header("MaterialChanger")]
        [SerializeField] Material[] m_skinMaterials;
        [SerializeField] Material m_materialToLookFor;
        [SerializeField] UI_Button_GroupColorChoser m_button_MaterialChangerPreafab;

        Dictionary<string, int> m_currentlySelectedBodyPart_ChildIndex = new Dictionary<string, int>();             // Dictionary to store currently selected body part index in each group
        Dictionary<string, GameObject> m_bodyPartGroup_GameObjects_ByName = new Dictionary<string, GameObject>();   // Dictionary to store references to body part group GameObjects
        string m_current_BodyPartGroup;                                                                             // Variable to store the currently selected body part group

        void Start()
        {
            #region Creating SkinMaterial Button
            UI_Button_GroupColorChoser _ui_button_MaterialChanger = Instantiate(m_button_MaterialChangerPreafab, m_buttonContainer);
            _ui_button_MaterialChanger.Enable(m_character, m_materialToLookFor, m_skinMaterials);

            #endregion

            #region Addons: Grabbing GameObject References
            for (int i = 0; i < m_bodyParts.Length; i++)
            {
                if (m_bodyParts[i].m_HadAddons)
                {
                    m_ui_Addons.AssignReferences(m_bodyParts[i]);
                }
            }
            #endregion

            #region BodyParts Assigning GameObject Parent References
            for (int i = 0; i < m_bodyParts.Length; i++)
            {
                GameObject _bodyPartGameObject = GameObject.Find(m_bodyParts[i].m_BodyPartName);
                if (_bodyPartGameObject != null)
                {
                    m_bodyParts[i].AssignBodyPartParent(_bodyPartGameObject.transform.parent.gameObject);
                }
                else
                {
                    for (int j = 0; j < m_bodyParts.Length; j++)
                    {
                        // Check if any of the body parts had the same body group
                        if (m_bodyParts[i].m_BodyGroup == m_bodyParts[j].m_BodyGroup)
                        {
                            if (m_bodyParts[j].m_BodyPart_Parent != null)
                            {
                                m_bodyParts[i].AssignBodyPartParent(m_bodyParts[j].m_BodyPart_Parent);
                            }
                            else
                            {
                                _bodyPartGameObject = GameObject.Find(m_bodyParts[j].m_BodyPartName);
                                if (_bodyPartGameObject != null) m_bodyParts[i].AssignBodyPartParent(_bodyPartGameObject.transform.parent.gameObject);
                            }
                        }
                    }
                }
            }
            #endregion

            #region BodyPartGroup Grabbing GameObject References
            for (int i = 0; i < m_bodyParts.Length; i++)
            {
                if (!m_bodyPartGroup_GameObjects_ByName.ContainsKey(m_bodyParts[i].m_BodyGroup.ToString()))
                {
                    GameObject _bodyPart = GameObject.Find(m_bodyParts[i].m_BodyPartName);
                    if (_bodyPart != null) m_bodyPartGroup_GameObjects_ByName.Add(m_bodyParts[i].m_BodyGroup.ToString(), _bodyPart);
                }
            }
            #endregion



            #region BodyPartGroup Buttons Creation
            List<string> _allKeys = new List<string>(m_bodyPartGroup_GameObjects_ByName.Keys);
            for (int i = 0; i < _allKeys.Count; i++)
            {
                UI_GroupButton _uI_GroupButton = Instantiate(m_uI_GroupButtonPrefab, m_buttonContainer);
                _uI_GroupButton.Setup(this, _allKeys[i]);
                m_currentlySelectedBodyPart_ChildIndex.Add(_allKeys[i], 0);
            }
            #endregion

            m_buttonContainer.GetComponent<KeepSingleButtonSelected>().Enable();
            m_ui_bodyParts.Enable(this, m_bodyParts);
        }

        // Void is Body Part group Button click action
        public void WasClick(string _string)
        {
            m_current_BodyPartGroup = _string;
            m_ui_bodyParts.DisplayButtons(_string, m_currentlySelectedBodyPart_ChildIndex[_string]);
        }

        // Void will store the index of the chosen BodyPart inside the body part group
        public void RememberChosenBodyPart_InsideBodyGroup(int _codyPartChildIndex) => m_currentlySelectedBodyPart_ChildIndex[m_current_BodyPartGroup] = _codyPartChildIndex;

        // Void will set up the addon panel according to the provided BodyPart
        public void Display_AddonsPanel(BodyPart _bodyPart, bool _hadAddons)
        {
            m_addonsPanel.SetActive(_hadAddons);
            m_ui_Addons.RestoreAllAddons(_bodyPart.m_Addons);
            m_ui_Addons.CreateButtons(_bodyPart);
        }

        //Void will hide all addons GameObjects
        public void HideAll_Addons_GameObjects(Addon[] _addons) => m_ui_Addons.HideAllAddons(_addons);
    }
}
