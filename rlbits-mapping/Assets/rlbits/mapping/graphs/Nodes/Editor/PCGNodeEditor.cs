using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
namespace RLBits.Mapping.Graphs
{
    [CustomNodeEditor(typeof(PCGNode))]
    public class PCGNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            if (GUILayout.Button("Update"))
            {
                (target as PCGNode).UpdateData();
            }

            if (GUILayout.Button("Update All"))
            {
                (target as PCGNode).noiseGraph.UpdateAll();
            }

            EditorGUI.BeginChangeCheck();
            base.OnBodyGUI();
            if (EditorGUI.EndChangeCheck())
            {
                (target as PCGNode).UpdateData();
            }

        }
    }
}
