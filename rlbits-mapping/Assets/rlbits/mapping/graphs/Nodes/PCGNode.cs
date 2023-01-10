using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("")]
    public class PCGNode : Node
    {
        [HideInInspector] public PCGNodeGraph NoiseGraph;

        protected override void Init()
        {
            NoiseGraph = graph as PCGNodeGraph;
            base.Init();
        }

        protected int GridToArray(int x, int y)
        {
            return x + (y * NoiseGraph.m_MasterNode.Size.x);
        }

        protected int GridToArray(Vector2Int gridIndex)
        {
            return gridIndex.x + (gridIndex.y * NoiseGraph.m_MasterNode.Size.x);
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            UpdateData();
            base.OnCreateConnection(from, to);
        }

        public virtual void UpdateData(bool withOutputs = true)
        {
            if (withOutputs)
            {
                //update all outputs
                foreach (NodePort np in Outputs)
                {
                    for (int i = 0; i < np.ConnectionCount; i++)
                    {
                        PCGNode nn = (PCGNode)np.GetConnection(i).node;
                        if (nn != null)
                        {
                            nn.UpdateData();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// tries to convert converts any NodePort's input to a float[]
        /// </summary>
        /// <param name="np">NodePort to fetch data from</param>
        /// <param name="targetArray">result of casting input to float[]</param>
        /// <returns></returns>
        protected bool ProcessNodeInput(NodePort np, out int[] targetArray)
        {
            if (np.TryGetInputValue<int[]>(out targetArray))
            {
                return true;
            }


            int inInt;
            if (np.TryGetInputValue<int>(out inInt))
            {
                if (targetArray == null || targetArray.Length != NoiseGraph.TotalCells)
                {
                    targetArray = new int[NoiseGraph.TotalCells];
                }

                for (int i = 0; i < targetArray.Length; i++)
                {
                    targetArray[i] = inInt;
                }
                return true;
            }
            else
            {
                Debug.Log("Invalid Input: " + np.fieldName);
            }

            targetArray = null;
            return false;
        }
    }
}