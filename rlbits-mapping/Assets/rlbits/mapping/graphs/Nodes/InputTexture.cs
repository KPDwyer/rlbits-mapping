using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("InputTexture")]
    public class InputTexture : PCGNode
    {
        [Output] public float[] m_result;
        public Texture2D m_Texture;

        protected Vector2Int m_NoiseParentSize;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "m_result")
            {
                if (m_result.Length != NoiseGraph.TotalCells)
                {
                    UpdateData();
                }
                return m_result;
            }
            return null;
        }

        public override void UpdateData(bool withOutputs = true)
        {
            m_NoiseParentSize = NoiseGraph.Size;


            if (m_result == null || m_result.Length != m_NoiseParentSize.x * m_NoiseParentSize.y)
            {
                m_result = new float[m_NoiseParentSize.x * m_NoiseParentSize.y];
            }
            if (!m_Texture.isReadable)
            {
                base.UpdateData(withOutputs);
            }


            for (int y = 0; y < m_NoiseParentSize.y; y++)
            {
                for (int x = 0; x < m_NoiseParentSize.x; x++)
                {

                    m_result[x + (y * m_NoiseParentSize.x)] =
                        m_Texture.GetPixel(
                            Mathf.RoundToInt(Mathf.Lerp(0, m_Texture.width, Mathf.InverseLerp(0, m_NoiseParentSize.x, x))),
                            Mathf.RoundToInt(Mathf.Lerp(0, m_Texture.height, Mathf.InverseLerp(0, m_NoiseParentSize.y, y)))
                            )[0];
                }
            }

            base.UpdateData(withOutputs);
        }
    }
}
