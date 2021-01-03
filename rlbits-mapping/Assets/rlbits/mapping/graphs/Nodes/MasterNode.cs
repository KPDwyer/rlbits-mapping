using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [NodeTint(0.5f, 0.5f, 0.1f)]
    [DisallowMultipleNodes(1)]
    public class MasterNode : Node
    {
        /// <summary>
        /// enable to freeze this node in the top left corner
        /// </summary>
        public bool FreezeNode;

        /// <summary>
        /// The seed used for all the Random instances in this graph.
        /// </summary>
        public int Seed;

        /// <summary>
        /// The Size of the channels this graph will generate. 
        /// </summary>
        public Vector2Int Size = new Vector2Int(100, 100);

        /// <summary>
        /// updates every node in the graph
        /// </summary>
        public void UpdateGraph()
        {
            (graph as PCGNodeGraph).UpdateAll();
        }

        /// <summary>
        /// used when <c>FreezeNode</c> is true to place the node in the top left corner.
        /// </summary>
        public void RepositionNode()
        {
            position = (graph as PCGNodeGraph).GetViewPortPosition();
        }
    }
}
