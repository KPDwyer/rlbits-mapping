using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using RLBits.Mapping.Graphs;

namespace RLBits.Mapping
{
    /// <summary>
    /// Used at runtime to Run and store the output of the PCGGraphs
    /// </summary>
    public class RuntimePCG : MonoBehaviour
    {
        [Header("Project References")]
        public PCGNodeGraph m_Graph;
        [Header("Data")]
        public Vector2Int m_Size = new Vector2Int(128, 128);
        public int m_Seed = 100;
        public ChannelMap[] m_ChannelMaps;
        public bool m_Loop;

        private Dictionary<string, float[]> m_channels;

        void Start()
        {
            GenWorld();
            if (m_Loop)
                StartCoroutine(LoopGenerations());
        }

        private void GenWorld()
        {
            m_channels = m_Graph.GetChannels(m_Seed, m_Size);
            foreach (ChannelMap nc in m_ChannelMaps)
            {
                nc.ProcessChannel(this);
            }
        }

        private IEnumerator LoopGenerations()
        {
            while (m_Loop)
            {
                yield return new WaitForSeconds(0.0f);
                m_Seed++;
                GenWorld();
            }
        }

        /// <summary>
        /// Lookup to retrieve Channels by their string ID
        /// </summary>
        /// <param name="name">name of the channel to retrieve (should match Ouput node)</param>
        /// <returns>a channel as a single-dimension float array</returns>
        public float[] GetChannel(string name)
        {
            //TODO KPD string lookups not great
            if (m_channels.ContainsKey(name))
            {
                return m_channels[name];
            }
            else
            {
                return null;
            }
        }
    }
}
