#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.UI;
using System.Collections.Generic;

public class CleanupEmptyUIParams : EditorWindow
{
    [MenuItem("Tools/Cleanup/Remove Empty UI Animator Parameters")]
    static void Open()
    {
        GetWindow<CleanupEmptyUIParams>("Cleanup Empty UI Params");
    }

    void OnGUI()
    {
        GUILayout.Label("Scan and remove empty animator parameter names used in UI Selectables and Animators", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Scan Project")) ScanProject();
        if (GUILayout.Button("Fix Selectables")) FixSelectables();
        if (GUILayout.Button("Fix Animator Controllers")) FixAnimatorControllers();
    }

    void ScanProject()
    {
        var selectables = Resources.FindObjectsOfTypeAll<Selectable>();
        int emptyCount = 0;
        foreach (var s in selectables)
        {
            var triggers = s.GetType().GetField("m_AnimationTriggers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(s);
            if (triggers != null)
            {
                var field = triggers.GetType().GetField("m_NormalTrigger");
                if (field != null)
                {
                    var name = (string)field.GetValue(triggers);
                    if (string.IsNullOrEmpty(name)) emptyCount++;
                }
            }
        }
        Debug.Log("Found " + emptyCount + " Selectables with empty animation trigger names (in loaded assets). Note: some assets may not be loaded; use Fix Selectables to modify serialized files.");

        var controllers = AssetDatabase.FindAssets("t:AnimatorController");
        int paramEmpty = 0;
        foreach (var id in controllers)
        {
            var path = AssetDatabase.GUIDToAssetPath(id);
            var ac = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (ac == null) continue;
            foreach (var p in ac.parameters)
            {
                if (string.IsNullOrEmpty(p.name)) paramEmpty++;
            }
        }
        Debug.Log("Found " + paramEmpty + " empty Animator parameters across AnimatorControllers.");
    }

    void FixSelectables()
    {
        // Only process prefab assets to avoid loading scene objects (which triggers Editor warnings).
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int fixedCount = 0;
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            if (string.IsNullOrEmpty(path)) continue;

            // Load prefab contents for safe editing
            var prefabRoot = PrefabUtility.LoadPrefabContents(path);
            if (prefabRoot == null) continue;

            bool changed = false;
            var sels = prefabRoot.GetComponentsInChildren<Selectable>(true);
            foreach (var sel in sels)
            {
                var so = new SerializedObject(sel);
                var triggersProp = so.FindProperty("m_AnimationTriggers");
                if (triggersProp != null)
                {
                    var normal = triggersProp.FindPropertyRelative("m_NormalTrigger");
                    if (normal != null && normal.stringValue == "")
                    {
                        normal.stringValue = "None";
                        so.ApplyModifiedProperties();
                        changed = true;
                        fixedCount++;
                    }
                }
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<Object>(path));
            }

            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Fixed " + fixedCount + " Selectables in prefabs by replacing empty trigger name with 'None'.");
    }

    void FixAnimatorControllers()
    {
        var controllers = AssetDatabase.FindAssets("t:AnimatorController");
        int fixedParams = 0;
        foreach (var id in controllers)
        {
            var path = AssetDatabase.GUIDToAssetPath(id);
            var ac = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (ac == null) continue;
            var serialized = new SerializedObject(ac);
            var paramsProp = serialized.FindProperty("m_Parameters");
            if (paramsProp == null) continue;
            for (int i = 0; i < paramsProp.arraySize; i++)
            {
                var el = paramsProp.GetArrayElementAtIndex(i);
                var nameProp = el.FindPropertyRelative("m_Name");
                if (nameProp != null && string.IsNullOrEmpty(nameProp.stringValue))
                {
                    nameProp.stringValue = "_unused_param_" + i;
                    fixedParams++;
                }
            }
            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(ac);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Renamed " + fixedParams + " empty Animator parameters.");
    }
}
#endif