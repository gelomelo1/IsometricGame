using UnityEngine;
using UnityEditor;

namespace Character_Modular_Soldier_Lowpoly_Pack
{
    public class PrefabSaver : MonoBehaviour
    {
        // Save the GameObject as a Prefab in the Resources folder with an incremented number
        public static void SavePrefab(GameObject prefabToSave, string basePrefabName)
        {
#if UNITY_EDITOR
            int prefabNumber = 1;
            string prefabName = basePrefabName + " " + prefabNumber;
            string folderPath = "Assets/3D Characters Military Lowpoly Pack/Prefabs/Characters/";
            // Check if a Prefab with the current name already exists
            while (AssetDatabase.LoadAssetAtPath<GameObject>(folderPath + prefabName + ".prefab") != null)
            {
                prefabNumber++;
                prefabName = basePrefabName + " " + prefabNumber;
            }

            // Create or update the Prefab
            PrefabUtility.SaveAsPrefabAsset(prefabToSave, folderPath + prefabName + ".prefab");
            Debug.Log("Prefab saved successfully in the Prefabs folder with name: " + prefabName);
            AssetDatabase.Refresh();
#endif
        }
    }
}


