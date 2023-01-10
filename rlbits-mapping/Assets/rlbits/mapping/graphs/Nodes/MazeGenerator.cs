using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{
    [CreateNodeMenu("Cells/Maze Generator")]
    [NodeTint(0.35f, 0.05f, 0.6f)]
    public class MazeGenerator : PCGNode
    {
        [Output] public int[] m_result;
        [Input] public int[] m_map;
        protected Vector2Int m_NoiseParentSize;

        /// <summary>
        /// How many steps the maze should take (-1 to run until complete)
        /// </summary>
        [Input] public int StepCount = -1;
        public bool AlignedPassages = true;

        public int Sparseness = 0;

        private List<Vector2Int> m_Neighbours;
        private List<Vector2Int> m_PassableCells;
        private List<int> m_DeadEndPassableIndex;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "m_result")
            {
                if (m_result != null)
                {
                    if (m_result.Length != NoiseGraph.TotalCells)
                    {
                        UpdateData();
                    }
                    return m_result;
                }
            }
            return null;
        }

        public override void UpdateData(bool withOutputs = true)
        {
            m_NoiseParentSize = NoiseGraph.Size;
            m_map = GetPort("m_map").GetInputValue<int[]>();

            m_result = new int[m_NoiseParentSize.x * m_NoiseParentSize.y];

            if (m_map != null && m_map.Length == m_NoiseParentSize.x * m_NoiseParentSize.y)
            {
                m_map.CopyTo(m_result, 0);
            }

            Random.InitState(NoiseGraph.Seed);
            m_Neighbours = new List<Vector2Int>();
            m_PassableCells = new List<Vector2Int>();
            Vector2Int StartingPoint = new Vector2Int(Random.Range(1, m_NoiseParentSize.x - 2), Random.Range(1, m_NoiseParentSize.y - 2));
            SetCellEmpty(StartingPoint, true);

            if (StepCount >= 0)
            {
                for (int i = 0; i < StepCount; i++)
                {
                    if (m_Neighbours.Count > 0)
                    {
                        DigMaze();
                    }
                }
                foreach (Vector2Int vi in m_Neighbours)
                {
                    m_result[GridToArray(vi.x, vi.y)] = 127;
                }
            }
            else
            {
                while (m_Neighbours.Count > 0)
                {
                    DigMaze();
                }
            }

            if (Sparseness > 0)
            {

                m_DeadEndPassableIndex = new List<int>();

                for (int sparse = 0; sparse < Sparseness; sparse++)
                {
                    for (int i = 0; i < m_PassableCells.Count; i++)
                    {
                        if (GetAdjacentEmptyCount(m_PassableCells[i]) == 1)
                        {
                            m_DeadEndPassableIndex.Add(i);
                        }
                    }

                    for (int p = m_DeadEndPassableIndex.Count - 1; p >= 0; p--)
                    {
                        FillInDeadEnd(p);
                    }
                    m_DeadEndPassableIndex.Clear();
                }


                base.UpdateData(withOutputs);

            }
        }



        private void DigMaze()
        {
            bool haveGoodTarget = false;
            Vector2Int targetWall = new Vector2Int();
            Vector2Int revealedCell = new Vector2Int();
            int index = -1;
            while (!haveGoodTarget)
            {
                if (m_Neighbours.Count == 0)
                {
                    return;
                }
                index = Random.Range(Mathf.Max(0, m_Neighbours.Count - 3), m_Neighbours.Count);
                targetWall = m_Neighbours[index];

                haveGoodTarget = IsGoodTarget(targetWall, ref revealedCell);
                if (!haveGoodTarget)
                {
                    m_Neighbours.RemoveAt(index);
                }
            }
            if (haveGoodTarget)
            {
                m_Neighbours.RemoveAt(index);
                SetCellEmpty(targetWall, !AlignedPassages);
                SetCellEmpty(revealedCell, true);
            }

        }

        private void FillInDeadEnd(int index)
        {
            m_result[GridToArray(m_PassableCells[m_DeadEndPassableIndex[index]].x, m_PassableCells[m_DeadEndPassableIndex[index]].y)] = 0;
            m_PassableCells.RemoveAt(m_DeadEndPassableIndex[index]);

        }

        private int GetAdjacentEmptyCount(Vector2Int cell)
        {
            int result = 0;

            if (m_result[GridToArray(cell + Vector2Int.up)] >= 255)
                result++;
            if (m_result[GridToArray(cell + Vector2Int.right)] >= 255)
                result++;
            if (m_result[GridToArray(cell + Vector2Int.left)] >= 255)
                result++;
            if (m_result[GridToArray(cell + Vector2Int.down)] >= 255)
                result++;

            return result;
        }

        private bool IsGoodTarget(Vector2Int target, ref Vector2Int nextCell)
        {
            //edges
            if (target.x <= 0 || target.x >= m_NoiseParentSize.x - 1 ||
                target.y <= 0 || target.y >= m_NoiseParentSize.y - 1)
            {
                return false;
            }


            Vector2Int occupiedSide = new Vector2Int();

            int neighbourCount = 0;
            if (m_result[GridToArray(target.x, target.y + 1)] == 255)
            {
                occupiedSide = new Vector2Int(0, 1);
                neighbourCount++;
            }

            if (m_result[GridToArray(target.x + 1, target.y)] == 255)
            {
                occupiedSide = new Vector2Int(1, 0);
                neighbourCount++;
            }

            if (m_result[GridToArray(target.x, target.y - 1)] == 255)
            {
                occupiedSide = new Vector2Int(0, -1);
                neighbourCount++;
            }

            if (m_result[GridToArray(target.x - 1, target.y)] == 255)
            {
                occupiedSide = new Vector2Int(-1, 0);
                neighbourCount++;
            }

            if (neighbourCount > 1)
            {
                return false;
            }

            nextCell = target + (occupiedSide * -1);

            return SurroundedByWalls(nextCell);
        }

        private bool SurroundedByWalls(Vector2Int target)
        {
            //edges
            if (target.x <= 0 || target.x >= m_NoiseParentSize.x - 1 ||
                target.y <= 0 || target.y >= m_NoiseParentSize.y - 1)
            {
                return false;
            }

            if (m_result[GridToArray(target.x, target.y + 1)] == 255)
            {
                return false;
            }

            if (m_result[GridToArray(target.x + 1, target.y + 1)] == 255)
            {
                return false;
            }

            if (m_result[GridToArray(target.x + 1, target.y)] == 255)
            {
                return false;
            }
            if (m_result[GridToArray(target.x + 1, target.y - 1)] == 255)
            {
                return false;
            }

            if (m_result[GridToArray(target.x, target.y - 1)] == 255)
            {
                return false;
            }

            if (m_result[GridToArray(target.x + 1, target.y - 1)] == 255)
            {
                return false;
            }


            if (m_result[GridToArray(target.x - 1, target.y)] == 255)
            {
                return false;
            }

            if (m_result[GridToArray(target.x - 1, target.y - 1)] == 255)
            {
                return false;
            }
            return true;
        }

        private void SetCellEmpty(Vector2Int cell, bool addNeighbours)
        {


            if (addNeighbours)
            {
                //add this cells applicable neighbours
                if (m_result[GridToArray(cell.x, cell.y + 1)] == 0)
                {
                    m_Neighbours.Add(new Vector2Int(cell.x, cell.y + 1));
                }
                if (m_result[GridToArray(cell.x, cell.y - 1)] == 0)
                {
                    m_Neighbours.Add(new Vector2Int(cell.x, cell.y - 1));
                }
                if (m_result[GridToArray(cell.x + 1, cell.y)] == 0)
                {
                    m_Neighbours.Add(new Vector2Int(cell.x + 1, cell.y));
                }
                if (m_result[GridToArray(cell.x - 1, cell.y)] == 0)
                {
                    m_Neighbours.Add(new Vector2Int(cell.x - 1, cell.y));
                }
            }

            //empty the cell.
            m_PassableCells.Add(cell);
            m_result[GridToArray(cell.x, cell.y)] = 255;
        }
    }
}