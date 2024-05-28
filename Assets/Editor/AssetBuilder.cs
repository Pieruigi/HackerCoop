using HKR.Scriptables;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.TextureAssets;
using UnityEditor;
using UnityEngine;


namespace HKR.Editor
{
    public class AssetBuilder : MonoBehaviour
    {
        public const string ResourceFolder = "Assets/Resources";

        [MenuItem("Assets/Create/HKR/BuildingBlock")]
        public static void CreateBuildingBlockAsset()
        {
            BuildingBlockAsset asset = ScriptableObject.CreateInstance<BuildingBlockAsset>();

            string name = "BuildingBlock.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, BuildingBlockAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/HKR/InfectedNode")]
        public static void CreateInfectedNodeAsset()
        {
            InfectedNodeAsset asset = ScriptableObject.CreateInstance<InfectedNodeAsset>();
            string name = "InfectedNode.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, InfectedNodeAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }


        [MenuItem("Assets/Create/HKR/SecurityCamera")]
        public static void CreateSecurityCameraAsset()
        {
            SecurityCameraAsset asset = ScriptableObject.CreateInstance<SecurityCameraAsset>();
            string name = "SecurityCamera.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, SecurityCameraAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/HKR/SecurityDrone")]
        public static void CreateSecurityDroneAsset()
        {
            SecurityDroneAsset asset = ScriptableObject.CreateInstance<SecurityDroneAsset>();
            string name = "SecurityDrone.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, SecurityDroneAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/HKR/Destroyer")]
        public static void CreateDestroyerAsset()
        {
            DestroyerAsset asset = ScriptableObject.CreateInstance<DestroyerAsset>();
            string name = "Destroyer.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, DestroyerAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }

   

}



