using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RLBits.Mapping
{
    /// <summary>
    /// Maps a PCG Channel to some system at runtime.
    /// Override for different channel uses.
    /// </summary>
    public abstract class ChannelMap : MonoBehaviour
    {
        public string m_ChannelID;

        protected int[] m_channel;

        public abstract void ProcessChannel(RuntimePCG pcg);
    }
}
