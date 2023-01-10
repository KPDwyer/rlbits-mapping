using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RLBits.Mapping
{
    /// <summary>
    /// Reads a Channel with normalized values (0.0f-1.0f) to set Tiles in a tilemap.
    /// </summary>
    public class NormalizedChannelMap : ChannelMap
    {
        [Header("Data")]
        public ChannelRange[] m_ranges;
        [Header("References")]
        public Tilemap m_TargetMap;
        public override void ProcessChannel(RuntimePCG pcg)
        {
            m_channel = pcg.GetChannel(m_ChannelID);
            BoundsInt bounds = new BoundsInt(0, 0, 0, pcg.GraphSize.x, pcg.GraphSize.y, 1);

            TileBase[] tileArray = m_TargetMap.GetTilesBlock(bounds);
            for (int y = 0; y < pcg.GraphSize.y; y++)
            {
                for (int x = 0; x < pcg.GraphSize.x; x++)
                {
                    tileArray[x + (y * pcg.GraphSize.x)] = GetTileForValue(m_channel[x + (y * pcg.GraphSize.x)]);
                }
            }
            m_TargetMap.SetTilesBlock(bounds, tileArray);
        }

        private TileBase GetTileForValue(float value)
        {
            foreach (ChannelRange c in m_ranges)
            {
                if (value >= c.m_LowerBound && value <= c.m_UpperBound)
                {
                    return c.m_Tile;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Simple Class to store a range + tile for populating a Tilemap based on value.
    /// </summary>
    [System.Serializable]
    public class ChannelRange
    {
        public float m_LowerBound;
        public float m_UpperBound;
        public TileBase m_Tile;
    }
}