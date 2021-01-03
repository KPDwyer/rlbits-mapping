using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Noise/Simplex")]
    public class SimplexNode : FastNoiseNode
    {
        protected override void SetupNoiseType()
        {
            //TODO KPD figure out difference betwwen Simplex2 and Simplex2s
            fn.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        }
    }
}
