using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Logic/Math")]
    [NodeTint(0.1f, 0.5f, 0.3f)]
    public class MathNode : PCGNode
    {

        [Input(backingValue = ShowBackingValue.Never)] public Node m_AInput;
        [Input(backingValue = ShowBackingValue.Never)] public Node m_BInput;

        [Output] public int[] m_Result;

        public Operation m_operation = Operation.Add;

        private int[] m_A;
        private int[] m_B;

        public enum Operation
        {
            Add = 0,
            Subtract = 1,
            Multiply = 2,
            Average = 3
        }
        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        public override void UpdateData(bool withOutputs = true)
        {
            bool inputValid = true;

            NodePort npA = GetInputPort("m_AInput");
            NodePort npB = GetInputPort("m_BInput");
            if (!npA.IsConnected || !npB.IsConnected)
            {
                inputValid = false;
            }
            else
            {
                if (!ProcessNodeInput(npA, out m_A))
                {
                    inputValid = false;
                }

                if (!ProcessNodeInput(npB, out m_B))
                {
                    inputValid = false;
                }

                if (m_B == null)
                {
                    inputValid = false;
                }

                if (m_A == null)
                {
                    inputValid = false;
                }
            }

            if (m_A.Length != m_B.Length)
            {
                Debug.Log(m_A.Length + " " + m_B.Length);
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
                    switch (m_operation)
                    {
                        case Operation.Add:
                            m_Result[i] = m_A[i] + m_B[i];
                            break;
                        case Operation.Subtract:
                            m_Result[i] = m_A[i] - m_B[i];
                            break;
                        case Operation.Multiply:
                            m_Result[i] = m_A[i] * m_B[i];
                            break;
                        case Operation.Average:
                            m_Result[i] = (m_A[i] + m_B[i]) / 2;
                            break;
                    }
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