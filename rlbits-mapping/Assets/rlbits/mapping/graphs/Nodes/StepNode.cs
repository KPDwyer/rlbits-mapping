using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Logic/Step")]
    [NodeTint(0.1f, 0.5f, 0.3f)]
    public class StepNode : PCGNode
    {
        [Input(backingValue = ShowBackingValue.Never)] public Node m_Input;
        [Input] public float Value;

        [Output] public float[] m_Result;

        private float[] m_A;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        public override void UpdateData(bool withOutputs = true)
        {
            bool inputValid = true;

            NodePort np = GetInputPort("m_Input");


            if (!ProcessNodeInput(np, out m_A))
            {
                inputValid = false;
            }

            if (m_A == null)
            {
                inputValid = false;
            }

            if (inputValid && m_A.Length != noiseGraph.TotalCells)
            {
                Debug.Log("MisMatch");
                inputValid = false;
            }

            if (inputValid)
            {
                m_Result = new float[noiseGraph.TotalCells];
                for (int i = 0; i < m_Result.Length; i++)
                {
                    m_Result[i] = m_A[i] >= Value ? 1.0f : 0.0f;
                }
            }
            base.UpdateData(withOutputs);
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "m_Result")
            {
                return m_Result;
            }
            return null;
        }
    }
}