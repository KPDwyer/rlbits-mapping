using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Shapes/DistributeShapes")]
    [NodeTint(0.35f, 0.05f, 0.6f)]
    public class DistributeShapes : PCGNode
    {
        [Output] public List<GridShape> m_Shapes;
        [Output] public float[] m_result;

        public int m_MaxShapeCount;
        public GridShape.Shape m_Shape;
        public PlacementMode m_PlacementMode;

        public Vector2 m_ShapeSizeMin;
        public Vector2 m_ShapeSizeMax;
        public int Buffer;
        public bool m_Gradation;

        private int[,] m_Area;

        protected Vector2Int m_NoiseParentSize;
        public enum PlacementMode
        {
            AlwaysPlace = 0,
            DiscardFailedPlace = 1
        }



        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "m_result")
            {
                if (m_result != null)
                {
                    if (m_result.Length != noiseGraph.TotalCells)
                    {
                        UpdateData();
                    }
                    return m_result;
                }
            }
            else if (port.fieldName == "m_Shapes")
            {
                if (m_Shapes != null)
                {
                    return m_Shapes;
                }
            }
            return null;
        }


        public override void UpdateData(bool withOutputs = true)
        {

            m_NoiseParentSize = noiseGraph.Size;

            if (m_result == null || m_result.Length != m_NoiseParentSize.x * m_NoiseParentSize.y)
            {
                m_result = new float[m_NoiseParentSize.x * m_NoiseParentSize.y];
            }

            for (int y = 0; y < m_NoiseParentSize.y; y++)
            {
                for (int x = 0; x < m_NoiseParentSize.x; x++)
                {
                    m_result[x + (y * m_NoiseParentSize.x)] = 0.0f;
                }
            }

            SetShapeData();

            for (int i = 0; i < m_Shapes.Count; i++)
            {
                List<Vector2Int> indices = new List<Vector2Int>();
                m_Shapes[i].GetIndicesInShape(ref indices);
                foreach (Vector2Int index in indices)
                {
                    m_result[GridToArray(index)] =
                        m_Gradation ? (float)(i + 1) / (float)m_Shapes.Count : 1.0f;

                }
            }



            base.UpdateData(withOutputs);
        }

        private void SetShapeData()
        {
            Random.InitState(noiseGraph.m_MasterNode.Seed);
            //may not need this
            m_Area = new int[m_NoiseParentSize.x, m_NoiseParentSize.y];
            m_Shapes = new List<GridShape>();
            for (int i = 0; i < m_MaxShapeCount; i++)
            {
                GridShape rd = new GridShape();
                rd.shape = m_Shape;
                rd.width = (int)Random.Range(m_ShapeSizeMin.x, m_ShapeSizeMax.x);
                rd.height = (int)Random.Range(m_ShapeSizeMin.y, m_ShapeSizeMax.y);
                rd.position.x = Mathf.FloorToInt(Mathf.Lerp((rd.width * 0.5f), m_Area.GetLength(0) - (rd.width * 0.5f) - 1, Random.Range(0.0f, 1.0f)));
                rd.position.y = Mathf.FloorToInt(Mathf.Lerp((rd.height * 0.5f), m_Area.GetLength(1) - (rd.height * 0.5f) - 1, Random.Range(0.0f, 1.0f)));

                bool m_shouldAdd = true;

                switch (m_PlacementMode)
                {
                    default:
                    case PlacementMode.AlwaysPlace:
                        break;
                    case PlacementMode.DiscardFailedPlace:
                        if (DoesShapeIntersectOthers(rd))
                        {
                            m_shouldAdd = false;
                        }
                        break;
                }
                if (m_shouldAdd)
                {
                    m_Shapes.Add(rd);
                }
            }
        }
        public bool DoesShapeIntersectOthers(GridShape rd)
        {
            List<Vector2Int> indices = new List<Vector2Int>();
            rd.GetIndicesInShape(ref indices, Buffer);

            foreach (Vector2Int vi in indices)
            {
                foreach (GridShape gs in m_Shapes)
                {
                    if (gs.isPointInShape(vi))
                    {
                        return true;
                    }
                }
            }
            return false;

        }
    }
}