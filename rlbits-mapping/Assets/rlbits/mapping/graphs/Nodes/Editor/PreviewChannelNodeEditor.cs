using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;
using XNodeEditor;
namespace RLBits.Mapping.Graphs
{
    [CustomNodeEditor(typeof(PreviewChannelNode))]
    public class PreviewChannelNodeEditor : PCGNodeEditor
    {
        public override void OnBodyGUI()
        {
            // Draw default editor
            base.OnBodyGUI();

            // Get your node
            PreviewChannelNode node = target as PreviewChannelNode;

            // Draw your texture
            Texture2D tex = node.GetTexture();
            if (tex != null)
            {
                var ctr = EditorGUILayout.GetControlRect(false, GetWidth() * 0.9f);
                ctr.x = GetWidth() * 0.05f;
                ctr.width = GetWidth() * 0.9f;
                EditorGUI.DrawPreviewTexture(ctr, tex);
            }

            if (GUILayout.Button("Save PNG"))
            {
                (target as PreviewChannelNode).SaveTexture();
            }
        }
    }
}
