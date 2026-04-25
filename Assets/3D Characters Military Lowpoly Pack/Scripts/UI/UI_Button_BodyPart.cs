using UnityEngine;
using UnityEngine.UI;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class UI_Button_BodyPart : MonoBehaviour
    {
        Image m_iconImage;                   // Image component for the body part icon
        BodyPart m_bodyPart;                 // Reference to the associated BodyPart data
        GameObject m_bodyPartsGroupParent;   // Parent GameObject that holds all body parts buttons
        UI_BodyParts m_uiButtonContainer;    // Reference to the UI container managing body parts
        int m_childIndex;                    // Index of the button within its parent group
        bool m_wasEnabled = false;           // Flag to track whether the button has been enabled

        // Enable the button by initializing references to child components
        void Enable()
        {
            m_iconImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
            m_wasEnabled = true;
        }

        // Set up the button with information about the associated BodyPart
        public void Setup(UI_BodyParts uiButtonContainer, BodyPart bodyPart, GameObject bodyPartsGroupParent, int childIndex)
        {
            // Enable the button if it has not been enabled already
            if (!m_wasEnabled)
                Enable();

            // Configure the button's appearance based on the provided BodyPart information
            m_iconImage.enabled = bodyPart.m_Icon != null;
            m_iconImage.sprite = bodyPart.m_Icon;

            // Adjust the icon color based on the existence of the BodyPart's name
            Color iconColor = m_iconImage.color;
            iconColor.a = bodyPart.m_BodyPartName != string.Empty ? 1f : .1f;
            m_iconImage.color = iconColor;

            // Store information about the associated BodyPart, UI container, parent, and child index
            m_bodyPart = bodyPart;
            m_uiButtonContainer = uiButtonContainer;
            m_bodyPartsGroupParent = bodyPartsGroupParent;
            m_childIndex = childIndex;
        }

        // Handle button click event by notifying the UI container about the selected body part
        public void Click()
        {
            m_uiButtonContainer.SetupPart(m_bodyPart, m_childIndex);
        }
    }
}
