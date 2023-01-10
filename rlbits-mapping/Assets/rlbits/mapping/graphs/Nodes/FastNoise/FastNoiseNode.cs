using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    /// <summary>
    /// FastNoiseNode is the base class for most of the Noise-based Nodes.
    /// </summary>
    [CreateNodeMenu("")]
    [NodeTint(0.6f, 0.1f, 0.3f)]
    public class FastNoiseNode : PCGNode
    {
        [Input] public float m_frequencyMin = 0.01f;
        [Input] public float m_frequencyMax = 0.01f;
        [Space]
        [Input] public Vector2 m_OffsetMin = Vector2.zero;
        [Input] public Vector2 m_OffsetMax = Vector2.zero;
        [Space]
        public bool m_NormalizedOutput = true;
        public bool m_FractalNoise = false;
        [Space]
        public FastNoiseLite.FractalType m_FractalType = FastNoiseLite.FractalType.FBm;
        public int m_Octaves = 5;
        public float m_Lacunarity = 2.0f;
        public float m_Gain = 0.5f;
        [Space]
        [Output] public int[] m_result;
        protected Vector2Int m_NoiseParentSize;
        protected Vector2 m_offset;
        protected FastNoiseLite fn;
        protected override void Init()
        {
            base.Init();
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

        protected virtual void SetupNoiseType()
        {
        }

        public override void UpdateData(bool withOutputs = true)
        {
            if (fn == null)
            {
                Init();
            }
            SetupNoiseType();
            fn.SetSeed(NoiseGraph.Seed);
            Random.InitState(NoiseGraph.Seed);
            fn.SetFrequency(Random.Range(m_frequencyMin, m_frequencyMax));
            if (m_FractalNoise)
            {
                fn.SetFractalType(m_FractalType);
                fn.SetFractalOctaves(m_Octaves);
                fn.SetFractalLacunarity(m_Lacunarity);
                fn.SetFractalGain(m_Gain);
            }
            m_offset = new Vector2(
                Random.Range(m_OffsetMin.x, m_OffsetMax.x),
                Random.Range(m_OffsetMin.y, m_OffsetMax.y));

            m_NoiseParentSize = NoiseGraph.Size;

            m_result = new int[m_NoiseParentSize.x * m_NoiseParentSize.y];

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