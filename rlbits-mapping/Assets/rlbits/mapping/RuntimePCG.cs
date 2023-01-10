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
        public PCGNodeGraph Graph;
        [Header("Data")]
        public Vector2Int GraphSize = new Vector2Int(128, 128);
        public int Seed = 100;
        public ChannelMap[] ChannelMaps;
        public bool Loop;

        private Dictionary<string, int[]> channels;

        void Start()
        {
            GenWorld();
            if (Loop)
                StartCoroutine(LoopGenerations());
        }

        private void GenWorld()
        {
            channels = Graph.GetChannels(Seed, GraphSize);
            foreach (ChannelMap nc in ChannelMaps)
            {
                nc.ProcessChannel(this);
            }
        }

        private IEnumerator LoopGenerations()
        {
            while (Loop)
            {
                yield return new WaitForSeconds(0.0f);
                Seed++;
                GenWorld();
            }
        }

        /// <summary>
        /// Lookup to retrieve Channels by their string ID
        /// </summary>
        /// <param name="name">name of the channel to retrieve (should match Ouput node)</param>
        /// <returns>a channel as a single-dimension float array</returns>
        public int[] GetChannel(string name)
        {
            //TODO KPD string lookups not great
            if (channels.ContainsKey(name))
            {
                return channels[name];
            }
            else
            {
                return null;
            }
        }
    }
}
