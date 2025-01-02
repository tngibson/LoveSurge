using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(StressManager))]
public class StressManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Add Stress"))
            {
                StressManager.instance.AddToCurrentStress();
            }

            if (GUILayout.Button("Remove Stress"))
            {
                StressManager.instance.RemoveFromCurrentStress(0.1f);
            }
        }
    }
}
