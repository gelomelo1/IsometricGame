using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class UI_Button_Addons : MonoBehaviour
    {
        Image m_iconImage;                   // Image component for the addon icon
        Image m_parentBackgoundImage;        // Image component for the button's background
        Addon m_addon;                       // Reference to the associated Addon data
        UI_Addons m_uiButtonContainer;      // Reference to the UI container managing addons
        int m_childIndex;                    // Index of the button within its parent group
        int m_arrayIndex;                    // Index of the addon icon within the Addon's arrays
        bool m_wasEnabled = false;           // Flag to track whether the button has been enabled

        // Enable the button by initializing references to child components
        void Enable()
        {
            m_iconImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
            m_parentBackgoundImage = transform.GetChild(0).GetComponent<Image>();
            m_wasEnabled = true;
        }

        // Set up the button with information about the associated Addon
        public void Setup(UI_Addons uiButtonContainer, Addon addon, int childIndex, int arrayIndex)
        {
            // Enable the button if it has not been enabled already
            if (!m_wasEnabled)
                Enable();

            // Configure the button's appearance based on the provided Addon information
            m_iconImage.sprite = addon.m_Icons[arrayIndex];

            // Adjust the icon color based on the existence of the Addon's name
            Color iconColor = m_iconImage.color;
            iconColor.a = addon.m_AddonNames[arrayIndex] != string.Empty ? 1f : .1f;
            m_iconImage.color = iconColor;

            // Store information about the associated Addon, UI container, child index, and array index
            m_addon = addon;
            m_uiButtonContainer = uiButtonContainer;
            m_childIndex = childIndex;
            m_arrayIndex = arrayIndex;
        }

        // Handle button click event by notifying the UI container about the selected addon
        public void Click()
        {
            m_uiButtonContainer.SetupAddon(m_addon, m_childIndex, m_arrayIndex);
        }

        // Update the button's background color based on the provided state
        public void PresentAsClicked(bool state)
        {
            m_parentBackgoundImage.color = state
                ? new Color32(255, 219, 21, 71)  // Highlighted color
                : new Color32(255, 255, 255, 24); // Default color
        }
    }

}
