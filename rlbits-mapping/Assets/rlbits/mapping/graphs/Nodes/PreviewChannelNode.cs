using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace RLBits.Mapping.Graphs
{
    /// <summary>
    /// Simple Node Used to view the state of a channel at a specific point in the graph.
    /// You can also export PNGs from this node.
    /// </summary>
    [NodeTint(0.1f, 0.1f, 0.1f)]
    public class PreviewChannelNode : PCGNode
    {
        [Input] public float[] m_input;
        public bool m_useGradient = false;
        public Gradient m_Gradient;
        public bool m_UpscaleExport;
        private Texture2D m_texture;
        private PCGNodeGraph m_noiseParent;
        private Vector2Int m_noiseParentSize;

        protected override void Init()
        {
            base.Init();
            m_noiseParent = graph as PCGNodeGraph;
        }

        public override void UpdateData(bool withOutputs = true)
        {
            GenerateTexture();
            base.UpdateData();
        }

        public Texture2D GetTexture()
        {
            return m_texture;
        }

        private void GenerateTexture()
        {
            m_noiseParentSize = m_noiseParent.Size;
            m_input = GetPort("m_input").GetInputValue<float[]>();

            if (m_input == null)
            {
                return;
            }
            if (m_input.Length != m_noiseParentSize.x * m_noiseParentSize.y)
            {
                return;
            }

            m_texture = new Texture2D(m_noiseParentSize.x, m_noiseParentSize.y);
            for (int y = 0; y < m_noiseParentSize.y; y++)
            {
                for (int x = 0; x < m_noiseParentSize.x; x++)
                {
                    Color c;
                    if (m_useGradient)
                    {
                        c = m_Gradient.Evaluate(m_input[x + (y * m_noiseParentSize.x)]);
                    }
                    else
                    {
                        c = Color.Lerp(
                             Color.black,
                             Color.white,
                             m_input[x + (y * m_noiseParentSize.x)]);
                    }

                    m_texture.SetPixel(
                        x,
                        y,
                        c
                    );
                }
            }
            m_texture.filterMode = FilterMode.Point;
            m_texture.Apply();
        }

        public void SaveTexture()
        {
            Vector2Int size = new Vector2Int();
            if (m_UpscaleExport)
            {
                size = noiseGraph.m_MasterNode.Size;
                noiseGraph.m_MasterNode.Size = new Vector2Int(1024, 1024);
            }
            noiseGraph.UpdateAll();

            GenerateTexture();

            // Encode texture into PNG
            byte[] bytes = m_texture.EncodeToPNG();

            //For testing purposes, also write to a file in the project folder
            File.WriteAllBytes(Application.dataPath + "/" + m_noiseParent.Seed + ".png", bytes);

            AssetDatabase.Refresh();
            if (m_UpscaleExport)
            {
                noiseGraph.m_MasterNode.Size = size;
                noiseGraph.UpdateAll();
            }
        }
    }
}