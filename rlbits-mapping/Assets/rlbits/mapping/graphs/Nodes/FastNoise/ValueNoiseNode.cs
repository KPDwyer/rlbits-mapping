using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Noise/Value")]
    public class ValueNoiseNode : FastNoiseNode
    {
        protected override void SetupNoiseType()
        {
            fn.SetNoiseType(FastNoiseLite.NoiseType.Value);
        }
    }
}
