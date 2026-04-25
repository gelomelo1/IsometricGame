using UnityEngine;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    [CreateAssetMenu(fileName = "NewBodyPart", menuName = "ScriptableObjects/Addon", order = 2)]

    public class Addon : ScriptableObject
    {

        #region UI Display
        [SerializeField] Sprite[] m_icons;
        public Sprite[] m_Icons => m_icons;
        #endregion

        #region Mesh References
        [SerializeField] string[] m_addonNames;
        public string[] m_AddonNames => m_addonNames;
        #endregion

    }
}


