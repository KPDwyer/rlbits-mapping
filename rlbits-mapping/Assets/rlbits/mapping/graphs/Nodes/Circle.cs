using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace RLBits.Mapping.Graphs
{

    [CreateNodeMenu("Shapes/Circle")]
    public class Circle : PCGNode
    {
        [Output] public int[] m_result;

        protected Vector2Int m_NoiseParentSize;


        // Return the correct value of an output port when requested
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

            Vector2 centre = new Vector2(
                m_NoiseParentSize.x * 0.5f,
                m_NoiseParentSize.y * 0.5f);

            if (m_result == null || m_result.Length != m_NoiseParentSize.x * m_NoiseParentSize.y)
            {
                m_result = new int[m_NoiseParentSize.x * m_NoiseParentSize.y];
            }

            for (int y = 0; y < m_NoiseParentSize.y; y++)
            {
                for (int x = 0; x < m_NoiseParentSize.x; x++)
                {
                    m_result[x + (y * m_NoiseParentSize.x)] =
                        255 -
                        Mathf.RoundToInt(Vector2.Distance(
                            new Vector2(x, y),
                            centre) / (centre.magnitude * 0.7f)*255);
                }
            }

            base.UpdateData(withOutputs);
        }

    }
}