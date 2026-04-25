using System.Collections.Generic;
using UnityEngine;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class UI_Button_GroupColorChoser : MonoBehaviour
    {
        [SerializeField] Material m_material;
        int m_currentMaterial = 0;

        Transform m_transform;
        Material m_materialToLookFor;
        SkinnedMeshRenderer[] m_selectedSkinedMeshRenderers;

        Material[] m_skinMaterials;
        List<int> m_indexOfSkinedMeshRenderersHavingMaterialInsideArray = new List<int>();

        public void Enable(Transform _transform, Material _material, Material[] _skinMaterials)
        {
            m_transform = _transform;
            m_materialToLookFor = _material;
            Material[] _materials = default;
            List<int> _indexOfSkinedMeshRenderersHavingMaterial = new List<int>();
            m_skinMaterials = _skinMaterials;
            string _fixedName = $"{m_materialToLookFor.name} (Instance)";
            SkinnedMeshRenderer[] _skinedMeshRenderers = m_transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            for (int i=0; i< _skinedMeshRenderers.Length;i++)
            {
                _materials = _skinedMeshRenderers[i].materials;

                for (int j=0; j<_materials.Length;  j++)
                {
                    if (_materials[j].name == _fixedName)
                    {
                        _indexOfSkinedMeshRenderersHavingMaterial.Add(i);
                        m_indexOfSkinedMeshRenderersHavingMaterialInsideArray.Add(j);
                    }
                }

            }
            m_selectedSkinedMeshRenderers = new SkinnedMeshRenderer[_indexOfSkinedMeshRenderersHavingMaterial.Count];
            for (int i=0; i < _indexOfSkinedMeshRenderersHavingMaterial.Count;i++)
            {
                m_selectedSkinedMeshRenderers[i] = _skinedMeshRenderers[_indexOfSkinedMeshRenderersHavingMaterial[i]];
            }

        }


        public void WasClick()
        {
            m_currentMaterial++;
            if (m_currentMaterial > m_skinMaterials.Length - 1) m_currentMaterial = 0;
            Material[] _materials = default;
            for (int i=0; i<m_selectedSkinedMeshRenderers.Length;i++)
            {
                _materials = m_selectedSkinedMeshRenderers[i].materials;
                _materials[m_indexOfSkinedMeshRenderersHavingMaterialInsideArray[i]] = m_skinMaterials[m_currentMaterial];
                m_selectedSkinedMeshRenderers[i].materials = _materials;
            }
        }
    }
}

