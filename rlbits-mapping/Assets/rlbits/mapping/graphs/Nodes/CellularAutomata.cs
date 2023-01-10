using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace RLBits.Mapping.Graphs
{

    [CreateNodeMenu("Cells/CellularAutomata")]
    [NodeTint(0.35f, 0.05f, 0.6f)]
    public class CellularAutomata : PCGNode
    {
        [Input] public int[] m_input;
        [Input] public int IterationCount = 0;
        [Tooltip("The value a value must be above to be considered `on`")]
        [Input] public int ThresholdValue = 200;
        [Input] public bool UseWrap = false;
        [Output] public int[] m_actualResult;

        [Space]

        [Header("Rules")]
        public bool UseNearRule = true;
        [Range(0, 8)]
        public int NearLessThanCount = 4;
        [Range(0, 8)]
        public int NearGreaterThanCount = 4;
        [Space]
        public bool UseMidRule = true;
        [Range(0, 24)]
        public int MidLessThanCount = 10;
        [Range(0, 24)]
        public int MidGreaterThanCount = 14;
        [Space]
        public bool UseFarRule = false;
        [Range(0, 48)]
        public int FarLessThanCount = 10;
        [Range(0, 48)]
        public int FarGreaterThanCount = 14;
        //ok thats a lot of stuff, but mostly I'm hoping to keep it flat and dumb
        //isntead of a list of rules w/ distances.  There may be value in a simple/complex toggle.
        //should see how easy custom node editor is.

        [Output] public int[,] m_result;

        protected int m_NoiseParentSizeX, m_NoiseParentSizeY;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "m_actualResult")
            {
                if (m_actualResult != null)
                {
                    if (m_actualResult.Length != NoiseGraph.TotalCells)
                    {
                        UpdateData();
                    }
                    return m_actualResult;
                }
            }
            return null;
        }

        public override void UpdateData(bool withOutputs = true)
        {
            m_NoiseParentSizeX = NoiseGraph.Size.x;
            m_NoiseParentSizeY = NoiseGraph.Size.y;

            m_input = GetPort("m_input").GetInputValue<int[]>();

            if (m_input == null)
            {
                return;
            }
            if (m_input.Length != m_NoiseParentSizeX * m_NoiseParentSizeY)
            {
                return;
            }

            m_result = new int[m_NoiseParentSizeX, m_NoiseParentSizeY];

            for (int y = 0; y < m_NoiseParentSizeY; y++)
            {
                for (int x = 0; x < m_NoiseParentSizeX; x++)
                {
                    m_result[x, y] = m_input[x + (y * m_NoiseParentSizeX)];
                }
            }


            //m_input.CopyTo(m_result, 0);

            Random.InitState(NoiseGraph.Seed);
            BeginAutomata();

            m_actualResult = new int[m_NoiseParentSizeX * m_NoiseParentSizeY];
            for (int y = 0; y < m_NoiseParentSizeY; y++)
            {
                for (int x = 0; x < m_NoiseParentSizeX; x++)
                {
                    //m_result[x, y] = m_input[x + (y * m_NoiseParentSizeX)];
                    m_actualResult[x + (y * m_NoiseParentSizeX)] = m_result[x, y];
                }
            }


            base.UpdateData(withOutputs);
        }

        public void BeginAutomata()
        {
            if (IterationCount == -1)
            {
                int tries = 0;
                bool makeAdjustment = PerformPass();
                while (makeAdjustment)
                {
                    makeAdjustment = PerformPass();
                    tries++;
                    if (tries > 100)
                        makeAdjustment = false;
                }
            }
            else
            {
                for (int i = 0; i < IterationCount; i++)
                {
                    PerformPass();
                }
            }

        }

        public bool PerformPass()
        {
            bool changeHappened = false;
            //int cacheg2a;
            for (int y = 0; y < m_NoiseParentSizeY; y++)
            {
                for (int x = 0; x < m_NoiseParentSizeX; x++)
                {
                    //cacheg2a = GridToArray(x, y);
                    bool state = m_result[x, y] >= ThresholdValue;
                    int neighbourCount;
                    //----------------CLose
                    if (UseNearRule)
                    {
                        neighbourCount = GetNeightbourFillCount(x, y);
                        if (state && neighbourCount < NearLessThanCount)
                        {
                            changeHappened = true;
                            m_result[x, y] = 0;
                            continue;
                        }
                        if (!state && neighbourCount > NearGreaterThanCount)
                        {
                            changeHappened = true;
                            m_result[x, y] = 255;
                            continue;
                        }
                    }
                    //--------------------Mid
                    if (UseMidRule)
                    {
                        neighbourCount = GetNeightbourFillCount(x, y, 2);
                        if (state && neighbourCount < MidLessThanCount)
                        {
                            changeHappened = true;
                            m_result[x, y] = 0;
                            continue;
                        }
                        if (!state && neighbourCount > MidGreaterThanCount)
                        {
                            changeHappened = true;
                            m_result[x, y] = 255;
                            continue;
                        }
                    }
                    //-------------------Far
                    if (UseFarRule)
                    {
                        neighbourCount = GetNeightbourFillCount(x, y, 3);
                        if (state && neighbourCount < FarLessThanCount)
                        {
                            changeHappened = true;
                            m_result[x, y] = 0;
                            continue;
                        }
                        if (!state && neighbourCount > FarGreaterThanCount)
                        {
                            changeHappened = true;
                            m_result[x, y] = 255;
                            continue;
                        }
                    }

                }
            }
            return changeHappened;

        }

        private int GetNeightbourFillCount(int inX, int inY, int dist = 1)
        {
            int result = 0;
            for (int y = inY - dist; y <= inY + dist; y++)
            {
                int actualY = y;
                if (y < 0 || y >= m_NoiseParentSizeY)
                {
                    if (UseWrap)
                    {
                        actualY = (y + m_NoiseParentSizeY) % m_NoiseParentSizeY;
                    }
                    else
                    {
                        continue;
                    }
                }

                for (int x = inX - dist; x <= inX + dist; x++)
                {
                    int actualX = x;
                    if (x < 0 || x >= m_NoiseParentSizeX)
                    {
                        if (UseWrap)
                        {
                            actualX = (x + m_NoiseParentSizeX) % m_NoiseParentSizeX;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (actualX == inX && actualY == inY)
                        continue;

                    if (m_result[actualX, actualY] >= ThresholdValue)
                    {
                        result++;
                    }
                }
            }
            return result;
        }
    }
}