using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    /// <summary>
    /// Used by runtime ChannelMaps to retrieve a Channel from the graph.
    /// </summary>
    public class OutputNode : PCGNode
    {
        [Input] public float[] m_Inputs;
        public string Name;

        /// <summary>
        /// This function should be called from a runtime class to retrieve 
        /// a Channel for use.
        /// </summary>
        /// <returns></returns>
        public float[] GetChannel()
        {
            return GetInputValue<float[]>("m_Inputs");
        }
    }
}
