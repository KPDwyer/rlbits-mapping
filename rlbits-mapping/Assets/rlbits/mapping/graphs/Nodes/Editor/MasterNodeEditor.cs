using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;
using XNodeEditor;
namespace RLBits.Mapping.Graphs
{
    [CustomNodeEditor(typeof(MasterNode))]
    public class MasterNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnBodyGUI();
            if (EditorGUI.EndChangeCheck())
            {
                (target as MasterNode).UpdateGraph();
            }
            if ((target as MasterNode).FreezeNode)
            {
                (target as MasterNode).RepositionNode();
            }
        }
    }
}
