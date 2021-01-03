using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Logic/Number")]
    [NodeTint(0.5f, 0.5f, 0.5f)]
    public class NumberNode : PCGNode
    {
        [Output(ShowBackingValue.Always)] public float m_Number;

        protected override void Init()
        {
            base.Init();
        }

        public override object GetValue(NodePort port)
        {
            return m_Number;
        }
    }
}
