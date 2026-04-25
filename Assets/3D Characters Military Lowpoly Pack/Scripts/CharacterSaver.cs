using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class CharacterSaver : MonoBehaviour
    {
        [Header("Character Setup")]
        [SerializeField] Transform m_orginalCharacter;
        [SerializeField] Transform m_characterRawPrefab;  // Reference to the character's RAW prefab
        List<int> m_activeTransformIndexes;  // List to store the indexes of active SkinnedMeshRenderer components
        List<int> m_activeTransformIndexesCopy;  // Copy of activeTransformIndexes for testing purposes

        [Header("String Representation")]
        [SerializeField] string m_stringValueOfCharacter;  // String representation of the active SkinnedMeshRenderer indexes
        StringBuilder m_stringBuilder = new StringBuilder();  // StringBuilder for efficient string manipulation
        List<SkinnedMeshRenderer> m_orginalCharacterSkinedMeshRenderers = new List<SkinnedMeshRenderer>();

        [Header("Materials")]
        [SerializeField] Material[] m_materials;
        String[] m_materialsNamesFixedWithInstance;


        private void Start()
        {
            //grabing materials string names enchanced for future comparsions
            m_materialsNamesFixedWithInstance = new string[m_materials.Length];
            for (int i = 0; i < m_materials.Length; i++)
            {
                m_materialsNamesFixedWithInstance[i] = m_materials[i].name + " (Instance) (Instance)";
            }

            // Load character if a string representation is provided
            if (!string.IsNullOrEmpty(m_stringValueOfCharacter))
            {
                LoadCharacter(m_stringValueOfCharacter);
            }


        }

        // Save the active SkinnedMeshRenderer indexes and update the string representation
        public void Save()
        {
            Transform[] _transforms = m_orginalCharacter.GetComponentsInChildren<Transform>(true);
            m_activeTransformIndexes = new List<int>();
            m_orginalCharacterSkinedMeshRenderers.Clear();
            // Identify active SkinnedMeshRenderer components and store their indexes
            for (int i = 0; i < _transforms.Length; i++)
            {
                if (_transforms[i].TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer _component) && _transforms[i].gameObject.activeInHierarchy)
                {
                    m_activeTransformIndexes.Add(i);
                    m_orginalCharacterSkinedMeshRenderers.Add(_component);
                }
            }

            // Update the string representation of active SkinnedMeshRenderer indexes
            m_stringValueOfCharacter = ProvideCharacterStringValue();

            // Provide a copy of activeTransformIndexes for testing purposes
            m_activeTransformIndexesCopy = ProvideCharacterActiveTransformIndex(m_stringValueOfCharacter);

            // Load the character based on the provided string representation
            LoadCharacter(m_activeTransformIndexesCopy);
        }

        // Create a string representation of active SkinnedMeshRenderer indexes
        string ProvideCharacterStringValue()
        {
            m_stringBuilder.Clear();
            for (int i = 0; i < m_activeTransformIndexes.Count; i++)
            {
                m_stringBuilder.Append(m_activeTransformIndexes[i]);
                m_stringBuilder.Append(".");
            }
            return m_stringBuilder.ToString();
        }

        // Extract active SkinnedMeshRenderer indexes from the provided string
        List<int> ProvideCharacterActiveTransformIndex(string _stringValue)
        {
            string[] _stringSplit = _stringValue.Split(".");
            List<int> _activeTransformIndex = new List<int>(_stringSplit.Length);
            for (int i = 0; i < _stringSplit.Length; i++)
            {
                if (int.TryParse(_stringSplit[i], out int _result))
                {
                    _activeTransformIndex.Add(_result);
                }
            }
            return _activeTransformIndex;
        }

        // Load the character based on the provided active SkinnedMeshRenderer indexes
        public void LoadCharacter(List<int> _activeTransformIndexes)
        {
            Transform _newCharacter = createNewCharacter();
            Transform[] _transforms = _newCharacter.GetComponentsInChildren<Transform>(true);

            // Disable all SkinnedMeshRenderer components in the cloned character
            for (int i = 0; i < _transforms.Length; i++)
            {
                if (_transforms[i].TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer _component))
                {
                    _transforms[i].gameObject.SetActive(false);
                }
            }

            fixingMaterialReferencesFromInstanceToMaterialsInFolders(_activeTransformIndexes, _transforms);

            StartCoroutine(ClearAndSaveGameObject(_newCharacter));
        }

        void fixingMaterialReferencesFromInstanceToMaterialsInFolders(List<int> _activeTransformIndexes, Transform[] _transforms)
        {
            // Enable SkinnedMeshRenderer components based on the provided indexes



            for (int i = 0; i < _activeTransformIndexes.Count; i++)
            {
                _transforms[_activeTransformIndexes[i]].gameObject.SetActive(true);
                _transforms[_activeTransformIndexes[i]].GetComponent<SkinnedMeshRenderer>().materials = m_orginalCharacterSkinedMeshRenderers[i].materials;
                Material[] _materials = _transforms[_activeTransformIndexes[i]].GetComponent<SkinnedMeshRenderer>().materials;
                for (int j = 0; j < _materials.Length; j++)
                {
                    for (int k = 0; k < m_materialsNamesFixedWithInstance.Length; k++)
                    {
                        if (_materials[j].name == m_materialsNamesFixedWithInstance[k])
                        {
                            _materials[j] = m_materials[k];
                        }
                    }
                }
                _transforms[_activeTransformIndexes[i]].GetComponent<SkinnedMeshRenderer>().materials = _materials;


            }
        }

        // Load the character based on the provided string representation of active SkinnedMeshRenderer indexes
        public void LoadCharacter(string _stringValue)
        {
            LoadCharacter(ProvideCharacterActiveTransformIndex(_stringValue));
        }

        Transform createNewCharacter()
        {
            Transform _newCharacter = Instantiate(m_characterRawPrefab);
            return _newCharacter;
        }
        void cleanUpInactiveGameObjects(GameObject parent)
        {
            // Get all child transforms of the parent
            Transform[] childTransforms = parent.GetComponentsInChildren<Transform>(true);

            // Iterate through each child transform
            foreach (Transform childTransform in childTransforms)
            {
                // Check if the GameObject is not active in the hierarchy
                if (!childTransform.gameObject.activeInHierarchy)
                {
                    // Destroy the inactive GameObject
                    Destroy(childTransform.gameObject);
                }
            }
        }

        IEnumerator ClearAndSaveGameObject(Transform _newCharacter)
        {
            if (_newCharacter.TryGetComponent<CharacterSaver>(out CharacterSaver _characterSaver)) Destroy(_characterSaver);

            cleanUpInactiveGameObjects(_newCharacter.gameObject);
            yield return new WaitForSecondsRealtime(.05f);
            PrefabSaver.SavePrefab(_newCharacter.gameObject, "Soldier");
            yield return new WaitForSecondsRealtime(.05f);
            Destroy(_newCharacter.gameObject);
        }

    }
}

