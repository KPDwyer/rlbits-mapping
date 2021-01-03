using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Noise/Cubic")]
    public class CubicNode : FastNoiseNode
    {
        protected override void SetupNoiseType()
        {
            //TODO KPD we were doing something here that fastnoiseLite doesn't support
            fn.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
        }
    }
}
