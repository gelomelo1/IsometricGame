using Character_Modular_Soldier_Lowpoly_Pack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class UI_Addons : MonoBehaviour
    {
        [SerializeField] GameObject m_character;
        [SerializeField] UI_Button_Addons m_UI_button_Addons_Prefab;
        List<UI_Button_Addons> m_buttons = new List<UI_Button_Addons>();
        Dictionary<string, GameObject> m_addon_GameObjects = new Dictionary<string, GameObject>();
        Dictionary<string, bool> m_addon_State = new Dictionary<string, bool>();
        BodyPart m_currentDisplayedBodyPart;

        // Create and display buttons for the specified body part's addons
        public void CreateButtons(BodyPart currentBodyPart)
        {
            if (m_currentDisplayedBodyPart == currentBodyPart) return;
            m_currentDisplayedBodyPart = currentBodyPart;

            // Clear existing buttons
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            m_buttons.Clear();

            // Display buttons for addons if the body part has addons
            if (currentBodyPart.m_HadAddons)
            {
                StopAllCoroutines();
                StartCoroutine(DisplayButtonsCoroutine(currentBodyPart));
            }
        }

        // Coroutine to display buttons for each addon
        IEnumerator DisplayButtonsCoroutine(BodyPart currentBodyPart)
        {
            for (int i = 0; i < currentBodyPart.m_Addons.Length; i++)
            {
                if (currentBodyPart.m_Addons[i].m_AddonNames.Length > 1)
                {
                    for (int j = 0; j < currentBodyPart.m_Addons[i].m_AddonNames.Length; j++)
                    {
                        UI_Button_Addons newButton = Instantiate(m_UI_button_Addons_Prefab, transform);
                        newButton.Setup(this, currentBodyPart.m_Addons[i], m_buttons.Count, j);
                        m_buttons.Add(newButton);
                        newButton.PresentAsClicked(m_addon_State[currentBodyPart.m_Addons[i].m_AddonNames[j]]);
                        yield return new WaitForSecondsRealtime(.1f);
                    }
                }
                else
                {
                    UI_Button_Addons newButton = Instantiate(m_UI_button_Addons_Prefab, transform);
                    newButton.Setup(this, currentBodyPart.m_Addons[i], i, 0);
                    m_buttons.Add(newButton);
                    newButton.PresentAsClicked(m_addon_State[currentBodyPart.m_Addons[i].m_AddonNames[0]]);
                    yield return new WaitForSecondsRealtime(.1f);
                }
            }
            yield return null;
        }

        // Handle the selection of an addon by updating its state and button appearance
        public void SetupAddon(Addon addon, int childIndex, int indexInAddonArray)
        {
            EnableAddonGameObjectState(addon, indexInAddonArray);
            m_buttons[childIndex].PresentAsClicked(m_addon_State[addon.m_AddonNames[indexInAddonArray]]);
        }

        // Assign references for addons associated with the specified body part
        public void AssignReferences(BodyPart bodyPart)
        {
            for (int i = 0; i < bodyPart.m_Addons.Length; i++)
            {
                foreach (string addonNameString in bodyPart.m_Addons[i].m_AddonNames)
                {
                    if (!m_addon_GameObjects.ContainsKey(addonNameString))
                    {
                        GameObject addonGameObject = GameObject.Find(addonNameString);
                        if (addonGameObject != null)
                        {
                            m_addon_GameObjects.Add(addonNameString, addonGameObject);
                            m_addon_State.Add(addonNameString, false);
                            addonGameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        // Enable or disable the state of the addon GameObject based on the provided index
        void EnableAddonGameObjectState(Addon addon, int indexOfAddon)
        {
            if (addon.m_AddonNames.Length < 2)
            {
                GameObject addonGameObject = m_addon_GameObjects[addon.m_AddonNames[indexOfAddon]];
                bool state = !m_addon_State[addon.m_AddonNames[indexOfAddon]];
                m_addon_State[addon.m_AddonNames[indexOfAddon]] = state;
                addonGameObject.SetActive(state);
            }
            else
            {
                for (int i = 0; i < addon.m_AddonNames.Length; i++)
                {
                    GameObject addonGameObject = m_addon_GameObjects[addon.m_AddonNames[i]];
                    bool state = i == indexOfAddon ?
                        !m_addon_State[addon.m_AddonNames[indexOfAddon]] :
                        false;
                    m_addon_State[addon.m_AddonNames[i]] = state;
                    addonGameObject.SetActive(state);
                    m_buttons[i].PresentAsClicked(state);
                }
            }
        }

        // Hide all addons associated with the specified array of addons
        public void HideAllAddons(Addon[] addons)
        {
            foreach (Addon addon in addons)
            {
                foreach (string addonNameString in addon.m_AddonNames)
                {
                    GameObject addonGameObject = m_addon_GameObjects[addonNameString];
                    addonGameObject.SetActive(false);
                }
            }
        }

        // Restore the state of all addons associated with the specified array of addons
        public void RestoreAllAddons(Addon[] addons)
        {
            foreach (Addon addon in addons)
            {
                foreach (string addonNameString in addon.m_AddonNames)
                {
                    GameObject addonGameObject = m_addon_GameObjects[addonNameString];
                    addonGameObject.SetActive(m_addon_State[addonNameString]);
                }
            }
        }
    }
}

