using UnityEngine;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    [CreateAssetMenu(fileName = "NewBodyPart", menuName = "ScriptableObjects/BodyParts", order = 1)]
    public class BodyPart : ScriptableObject
    {
        [SerializeField] BodyPartGroup m_bodyGroup;
        public BodyPartGroup m_BodyGroup => m_bodyGroup;


        #region UI Display
        [SerializeField] Sprite m_icon;
        public Sprite m_Icon => m_icon;
        #endregion

        #region Mesh References
        [SerializeField] string m_bodyPartName;
        public string m_BodyPartName => m_bodyPartName;

        GameObject m_bodyPart_Parent;
        public GameObject m_BodyPart_Parent => m_bodyPart_Parent;
        public void AssignBodyPartParent(GameObject _bodyPart_Parent) => m_bodyPart_Parent = _bodyPart_Parent;
        #endregion

        #region Addons
        [SerializeField] Addon[] m_addons;
        public Addon[] m_Addons => m_addons;
        public bool m_HadAddons => m_addons.Length > 0;
        #endregion
    }
}



