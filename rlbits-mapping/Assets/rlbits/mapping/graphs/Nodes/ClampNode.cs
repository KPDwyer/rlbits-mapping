using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Logic/Clamp")]
    [NodeTint(0.1f, 0.5f, 0.3f)]

    public class ClampNode : PCGNode
    {
        public int minValue;
        public int maxValue;

        [Input(backingValue = ShowBackingValue.Never)] public Node m_Input;


        [Output] public int[] m_Result;

        private int[] m_A;


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

            if (m_A.Length != NoiseGraph.TotalCells)
            {
                Debug.Log("MisMatch");
                inputValid = false;
            }

            if (inputValid)
            {
                m_Result = new int[NoiseGraph.TotalCells];
                for (int i = 0; i < m_Result.Length; i++)
                {
                    m_Result[i] = Mathf.Clamp(m_A[i], minValue, maxValue);

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