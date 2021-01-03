using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Shapes/ConnectShapes")]
    [NodeTint(0.35f, 0.05f, 0.6f)]

    public class ConnectShapes : PCGNode
    {
        [Input] public List<GridShape> m_Input;

        [Output] public List<GridShape> m_Shapes;
        [Output] public float[] m_result;

        public int AdditionalConnectionsToMake;
        public float MapValue;
        public HallwayAttach RoomPositionSample;
        public GridShape.Shape HallwayShape;
        public int HallwayMinSize;
        public int HallwayMaxSize;
        public bool HallWayUniformSize;

        protected Vector2Int m_NoiseParentSize;


        public enum HallwayAttach
        {
            ShapePosition = 0,
            RandomInside = 1,
            AxisAligned = 2
        }

        public class EdgeData
        {
            public int a;
            public int b;
            public float weight;
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
            m_Input = GetPort("m_Input").GetInputValue<List<GridShape>>();

            m_result = new float[m_NoiseParentSize.x * m_NoiseParentSize.y];

            Random.InitState(noiseGraph.Seed);

            if (m_Input == null)
            {
                return;
            }

            //construct mst
            List<EdgeData> allEdges = new List<EdgeData>();
            for (int starts = 0; starts < m_Input.Count; starts++)
            {
                for (int ends = 0; ends < starts; ends++)
                {
                    if (starts == ends)
                        continue;


                    EdgeData ed = new EdgeData();
                    ed.a = starts;
                    ed.b = ends;
                    ed.weight = Vector2Int.Distance(m_Input[starts].position, m_Input[ends].position);
                    allEdges.Add(ed);
                }
            }

            List<int> processedIndices = new List<int>();
            List<EdgeData> minimumEdges = new List<EdgeData>();

            processedIndices.Add(Random.Range(0, m_Input.Count));

            for (int i = 0; i < m_Input.Count - 1; i++)
            {
                float lowDistance = Mathf.Infinity;
                EdgeData target = null;
                foreach (EdgeData ed in allEdges)
                {
                    if (processedIndices.Contains(ed.a) ^ processedIndices.Contains(ed.b))
                    {
                        if (ed.weight <= lowDistance)
                        {
                            lowDistance = ed.weight;
                            target = ed;
                        }
                    }
                }

                minimumEdges.Add(target);
                allEdges.Remove(target);
                if (!processedIndices.Contains(target.a))
                {
                    processedIndices.Add(target.a);
                }
                if (!processedIndices.Contains(target.b))
                {
                    processedIndices.Add(target.b);
                }
            }


            for (int i = 0; i < AdditionalConnectionsToMake; i++)
            {
                float lowDistance = Mathf.Infinity;
                EdgeData target = null;
                foreach (EdgeData ed in allEdges)
                {
                    if (ed.weight <= lowDistance)
                    {
                        lowDistance = ed.weight;
                        target = ed;
                    }
                }
                if (target == null)
                {
                    break;
                }
                minimumEdges.Add(target);
                allEdges.Remove(target);

            }
            foreach (EdgeData ed in minimumEdges)
            {

                GridShape a = m_Input[ed.a];
                GridShape b = m_Input[ed.b];


                Vector2Int startPos = a.position;
                Vector2Int endPos = b.position;
                bool axisAlignedPlaced = false;

                switch (RoomPositionSample)
                {
                    case HallwayAttach.RandomInside:
                        startPos = a.GetRandomIndexInShape();
                        endPos = b.GetRandomIndexInShape();
                        break;
                    case HallwayAttach.AxisAligned:
                        axisAlignedPlaced = ConnectGridsAxisAligned(a, b, ref startPos, ref endPos);
                        break;
                }
                //will need to support multiple lines here for proper axis aligned.
                if (axisAlignedPlaced)
                    continue;
                m_Shapes = GetLineBetweenPoints(startPos, endPos);
                List<Vector2Int> m_targetPositions = new List<Vector2Int>();

                foreach (GridShape gs in m_Shapes)
                {
                    gs.GetIndicesInShape(ref m_targetPositions);
                }
                foreach (Vector2Int pos in m_targetPositions)
                {
                    if (GridToArray(pos) < m_result.Length && GridToArray(pos) >= 0)
                    {
                        m_result[GridToArray(pos)] = MapValue;
                    }
                }

            }
            base.UpdateData(withOutputs);
        }

        private bool ConnectGridsAxisAligned(GridShape startShape, GridShape endShape, ref Vector2Int startPos, ref Vector2Int endPos)
        {
            //we pad with min hallway size to cover our bases - not always going to work out
            //KPD TODO revisit padding here.
            Vector2Int minA = new Vector2Int(startShape.MinX() + HallwayMinSize, startShape.MinY() + HallwayMinSize);
            Vector2Int maxA = new Vector2Int(startShape.MaxX() - HallwayMinSize, startShape.MaxY() - HallwayMinSize);

            Vector2Int minB = new Vector2Int(endShape.MinX() + HallwayMinSize, endShape.MinY() + HallwayMinSize);
            Vector2Int maxB = new Vector2Int(endShape.MaxX() - HallwayMinSize, endShape.MaxY() - HallwayMinSize);

            startPos = new Vector2Int(Random.Range(minA.x, maxA.x), Random.Range(minA.y, maxA.y));
            endPos = new Vector2Int(Random.Range(minB.x, maxB.x), Random.Range(minB.y, maxB.y));


            bool overlapOnX = (minA.x <= maxB.x && maxA.x >= minB.x);
            bool overlapOnY = (minA.y <= maxB.y && maxA.y >= minB.y);

            if (overlapOnX && overlapOnY)
                return false; //already overlapping on both axes.


            if (overlapOnX)
            {
                startPos.x = Random.Range(Mathf.Max(minA.x, minB.x), Mathf.Min(maxA.x, maxB.x));
                startPos.y = Random.Range(minA.y, maxA.y);
                endPos.x = startPos.x;
                endPos.y = Random.Range(minB.y, maxB.y);
                return false;

            }
            else if (overlapOnY)
            {
                startPos.y = Random.Range(Mathf.Max(minA.y, minB.y), Mathf.Min(maxA.y, maxB.y));
                startPos.x = Random.Range(minA.x, maxA.x);
                endPos.y = startPos.y;
                endPos.x = Random.Range(minB.x, maxB.x);
                return false;
            }


            //at this point we need a bend, so we make the joint
            int jointX = startPos.x;
            int jointY = endPos.y;

            //and then do all the work to get and set these 2 paths.
            m_Shapes = GetLineBetweenPoints(startPos, new Vector2Int(jointX, jointY));
            m_Shapes.AddRange(GetLineBetweenPoints(new Vector2Int(jointX, jointY), endPos));
            List<Vector2Int> m_targetPositions = new List<Vector2Int>();

            foreach (GridShape gs in m_Shapes)
            {
                gs.GetIndicesInShape(ref m_targetPositions);
            }
            foreach (Vector2Int pos in m_targetPositions)
            {
                if (GridToArray(pos) < m_result.Length && GridToArray(pos) >= 0)
                {
                    m_result[GridToArray(pos)] = MapValue;
                }
            }
            //returning true tells the calling method that it doesn't need to generate it's path
            //as we've done it above.
            return true;
        }


        private List<GridShape> GetLineBetweenPoints(Vector2Int a, Vector2Int b)
        {
            List<GridShape> result = new List<GridShape>();
            List<Vector2Int> allPoints = GetPointsOnLine(a, b);
            foreach (Vector2Int vi in allPoints)
            {
                GridShape gs = new GridShape();
                gs.shape = HallwayShape;
                gs.position = vi;
                int size = Random.Range(HallwayMinSize, HallwayMaxSize);
                gs.width = size;
                if (!HallWayUniformSize)
                {
                    size = Random.Range(HallwayMinSize, HallwayMaxSize);
                }
                gs.height = size;

                result.Add(gs);
            }

            return result;
        }

        public static List<Vector2Int> GetPointsOnLine(Vector2Int p1, Vector2Int p2)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            int x0 = p1.x;
            int y0 = p1.y;
            int x1 = p2.x;
            int y1 = p2.y;

            bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Mathf.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                result.Add(new Vector2Int((steep ? y : x), (steep ? x : y)));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }

            return result;
        }
    }
}