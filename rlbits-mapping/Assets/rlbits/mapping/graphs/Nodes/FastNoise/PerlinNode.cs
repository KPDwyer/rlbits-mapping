using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Noise/Perlin")]
    public class PerlinNode : FastNoiseNode
    {
        protected override void SetupNoiseType()
        {
            fn.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        }
    }
}