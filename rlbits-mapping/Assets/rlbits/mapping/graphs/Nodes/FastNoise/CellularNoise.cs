using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    /// Cellular noise deviates quite a bit from the other FastNoiseNodes, so for the sake of a 
    /// Clean inspector, i made it its own thing rather than clutter the inspector.
    /// TODO: Revisit 
    [CreateNodeMenu("Noise/Cellular")]
    [NodeTint(0.6f, 0.1f, 0.3f)]
    public class CellularNoise : PCGNode
    {
        [Input] public float m_frequencyMin = 0.01f;
        [Input] public float m_frequencyMax = 0.01f;
        [Space]
        [Input] public Vector2 m_OffsetMin = Vector2.zero;
        [Input] public Vector2 m_OffsetMax = Vector2.zero;
        [Space]
        public FastNoiseLite.CellularDistanceFunction m_DistanceFunction;
        public FastNoiseLite.CellularReturnType m_ReturnType;
        [Space]
        public bool m_NormalizedOutput = true;
        [Space]
        [Output] public int[] m_result;

        protected Vector2 m_offset;
        protected FastNoiseLite fn;
        protected Vector2Int m_NoiseParentSize;

        protected override void Init()
        {
            base.Init();
            //TODO: can we elevate this FN object to the graph level and reuse it?
            fn = new FastNoiseLite(NoiseGraph.Seed);
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "m_result")
            {
                return m_result;
            }
            return null;
        }

        public override void UpdateData(bool withOutputs = true)
        {
            if (fn == null)
            {
                Init();
            }
            fn.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            fn.SetSeed(NoiseGraph.Seed);
            Random.InitState(NoiseGraph.Seed);
            fn.SetFrequency(Random.Range(m_frequencyMin, m_frequencyMax));

            m_offset = new Vector2(
                Random.Range(m_OffsetMin.x, m_OffsetMax.x),
                Random.Range(m_OffsetMin.y, m_OffsetMax.y));

            fn.SetCellularDistanceFunction(m_DistanceFunction);
            fn.SetCellularReturnType(m_ReturnType);

            m_NoiseParentSize = NoiseGraph.Size;
            if (m_result == null || m_result.Length != m_NoiseParentSize.x * m_NoiseParentSize.y)
            {
                m_result = new int[m_NoiseParentSize.x * m_NoiseParentSize.y];
            }

            float amtx = (1.0f / (float)m_NoiseParentSize.x) * 100;
            float amty = (1.0f / (float)m_NoiseParentSize.y) * 100;

            for (int y = 0; y < m_NoiseParentSize.y; y++)
            {
                for (int x = 0; x < m_NoiseParentSize.x; x++)
                {
                    float val = fn.GetNoise(
                            ((x * amtx) + m_offset.x),
                            ((y * amty) + m_offset.y));
                    if (m_NormalizedOutput)
                    {
                        val = (val * 0.5f) + 0.5f;
                    }
                    m_result[x + (y * m_NoiseParentSize.x)] = Mathf.RoundToInt(val*255);
                }
            }

            base.UpdateData(withOutputs);
        }
    }
}