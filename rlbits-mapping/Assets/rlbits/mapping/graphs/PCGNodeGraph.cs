using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace RLBits.Mapping.Graphs
{
    /// <summary>
    /// Base class for any Procedural Content Generation (PCG) Graphs.
    /// </summary>
    [CreateAssetMenu]
    [RequireNode(typeof(MasterNode))]
    public class PCGNodeGraph : NodeGraph
    {
        /// <summary>
        /// The Master Node controls the global parameters for a PCG Graph
        /// </summary>
        public MasterNode m_MasterNode;

        /// <summary>
        /// Access to the Graph's channel size via MAster Node.
        /// </summary>
        public Vector2Int Size
        {
            get
            {
                CheckMasterNode();
                return m_MasterNode.Size;
            }
        }

        /// <summary>
        /// Access to the Graph's seed value via Master Node.
        /// </summary>
        public int Seed
        {
            get
            {
                CheckMasterNode();
                return m_MasterNode.Seed;
            }
        }

        /// <summary>
        /// total cell count (Channel width * Channel height)
        /// </summary>
        public int TotalCells
        {
            get
            {
                CheckMasterNode();
                return m_MasterNode.Size.x * m_MasterNode.Size.y;
            }
        }

        List<PCGNode> UnProcessed;
        List<PCGNode> ToBeProcessed;
        List<PCGNode> Processed;

        /// <summary>
        /// Update all nodes in the graph (in order)
        /// </summary>
        public void UpdateAll()
        {
            UnProcessed = new List<PCGNode>();
            ToBeProcessed = new List<PCGNode>();
            Processed = new List<PCGNode>();
            BeginProcessingNodes();
            while (UnProcessed.Count > 0 || ToBeProcessed.Count > 0)
            {
                ProcessNodes();
            }
        }

        /// <summary>
        /// used to retrieve the grid position of the top left corener of the viewport.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetViewPortPosition()
        {
            return NodeEditorWindow.current.WindowToGridPosition(Vector2.zero);
        }

        private void BeginProcessingNodes()
        {
            foreach (Node n in nodes)
            {
                PCGNode nn = n as PCGNode;
                if (nn != null)
                {
                    bool hasInput = false;
                    foreach (NodePort np in nn.Inputs)
                    {
                        if (np.ConnectionCount > 0)
                        {
                            hasInput = true;
                            break;
                        }
                    }

                    if (hasInput)
                    {
                        UnProcessed.Add(nn);
                    }
                    else
                    {
                        ToBeProcessed.Add(nn);
                    }
                }
            }
        }

        private void ProcessNodes()
        {
            for (int i = ToBeProcessed.Count - 1; i >= 0; i--)
            {
                PCGNode node = ToBeProcessed[i];
                ToBeProcessed.RemoveAt(i);
                node.UpdateData(false);
                Processed.Add(node);
            }

            for (int i = UnProcessed.Count - 1; i >= 0; i--)
            {
                PCGNode node = UnProcessed[i];
                bool canBeProcessed = true;
                foreach (NodePort np in node.Inputs)
                {
                    foreach (NodePort outer in np.GetConnections())
                    {
                        PCGNode connectedNode = outer.node as PCGNode;
                        if (connectedNode != null)
                        {
                            if (!Processed.Contains(connectedNode))
                            {
                                canBeProcessed = false;
                                break;
                            }
                        }
                    }
                }
                if (canBeProcessed)
                {
                    UnProcessed.RemoveAt(i);
                    ToBeProcessed.Add(node);
                }
            }
        }


        private void CheckMasterNode()
        {
            if (m_MasterNode == null)
            {
                foreach (Node n in nodes)
                {
                    MasterNode mn = n as MasterNode;
                    if (mn != null)
                    {
                        m_MasterNode = mn;
                        return;
                    }
                }
                if (m_MasterNode == null)
                {
                    NodeGraphEditor nge = NodeGraphEditor.GetEditor(this, NodeEditorWindow.current);
                    XNode.Node node = nge.CreateNode(typeof(MasterNode), Vector2.zero);
                    NodeEditorWindow.current.AutoConnect(node);
                    m_MasterNode = (MasterNode)node;
                }
            }
        }

        /// <summary>
        /// Adds a ndoe to the graph.  Overriden to ensure Master Node stays at end of list (render order)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        override public Node AddNode(Type type)
        {
            Node.graphHotfix = this;
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.graph = this;
            nodes.Add(node);
            //order kick
            nodes.Remove(m_MasterNode);
            nodes.Add(m_MasterNode);
            return node;
        }

        #region Runtime
        /// <summary>
        /// Useda t runtime to execute the graph witha  seed + size.
        /// </summary>
        /// <param name="seed">Seed used to run the graph</param>
        /// <param name="size">size of the output channels</param>
        /// <returns>A Dictionary where key is Channel name, value is a flat float channel. </returns>
        public Dictionary<string, int[]> GetChannels(int seed, Vector2Int size)
        {
            CheckMasterNode();
            m_MasterNode.Seed = seed;
            m_MasterNode.Size = size;
            UpdateAll();
            OutputNode outputCache;
            Dictionary<string, int[]> channelMap = new Dictionary<string, int[]>();
            foreach (PCGNode pnode in Processed)
            {
                //TODO KPD have the output nodes set their channel maps when the map is run.
                outputCache = pnode as OutputNode;
                if (outputCache != null)
                {
                    //TODO KPD handle this better
                    if (!channelMap.ContainsKey(outputCache.Name))
                    {
                        channelMap.Add(outputCache.Name, outputCache.GetChannel());
                    }
                }
            }
            return channelMap;
        }
        #endregion

    }
}