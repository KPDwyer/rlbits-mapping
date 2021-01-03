using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridShape
{
    public enum Shape
    {
        Point = 0,
        Rectangle = 1,
        Ellipse = 2
    }

    public Shape shape;
    public Vector2Int position;
    public int width;
    public int height;


    private int halfHeight
    {
        get
        {
            return (int)(height * 0.5f);
        }
    }
    private int halfWidth
    {
        get
        {
            return (int)(width * 0.5f);
        }
    }

    public int MinX()
    {
        switch (shape)
        {
            default:
            case Shape.Point:
                return position.x;
            case Shape.Ellipse:
            case Shape.Rectangle:
                return position.x - halfWidth;
        }
    }
    public int MaxX()
    {
        switch (shape)
        {
            default:
            case Shape.Point:
                return position.x;
            case Shape.Ellipse:
            case Shape.Rectangle:
                return position.x + halfWidth;
        }
    }
    public int MinY()
    {
        switch (shape)
        {
            default:
            case Shape.Point:
                return position.y;
            case Shape.Ellipse:
            case Shape.Rectangle:
                return position.y - halfHeight;
        }
    }
    public int MaxY()
    {
        switch (shape)
        {
            default:
            case Shape.Point:
                return position.y;
            case Shape.Ellipse:
            case Shape.Rectangle:
                return position.y + halfHeight;
        }
    }

    public void SetPosition(Vector3Int pos)
    {
        position = new Vector2Int(pos.x, pos.y);
    }

    public bool isPointInShape(Vector2Int point)
    {
        switch (shape)
        {
            default:
            case Shape.Point:
                return point == position;
            case Shape.Ellipse:
                return isPointInEllipse(point);
            case Shape.Rectangle:
                return isPointInRectangle(point);
        }
    }

    public void GetIndicesInShape(ref List<Vector2Int> indices, int buffer = 0)
    {
        switch (shape)
        {
            default:
            case Shape.Point:
                indices.Add(position);
                break;
            case Shape.Ellipse:
                GetIndicesInEllipse(ref indices, buffer);
                break;
            case Shape.Rectangle:
                GetIndicesInRectangle(ref indices, buffer);
                break;
        }
    }

    public void GetIndicesInShape(ref List<Vector3Int> indices, int buffer = 0)
    {
        switch (shape)
        {
            default:
            case Shape.Point:
                indices.Add((Vector3Int)position);
                break;
            case Shape.Ellipse:
                GetIndicesInEllipse(ref indices, buffer);
                break;
            case Shape.Rectangle:
                GetIndicesInRectangle(ref indices, buffer);
                break;
        }
    }

    public Vector2Int GetRandomIndexInShape()
    {
        switch (shape)
        {
            default:
            case Shape.Point:
                return position;
            case Shape.Rectangle:
                return new Vector2Int(
                    Random.Range(position.x - halfWidth, position.x + halfWidth),
                    Random.Range(position.y - halfHeight, position.y + halfHeight));
            case Shape.Ellipse:
                List<Vector2Int> indices = new List<Vector2Int>();
                GetIndicesInEllipse(ref indices);
                return indices[Random.Range(0, indices.Count)];
        }
    }

    private void GetIndicesInRectangle(ref List<Vector2Int> indices, int buffer = 0)
    {
        for (int y = position.y - halfHeight - buffer; y < position.y + halfHeight + buffer; y++)
        {
            for (int x = position.x - halfWidth - buffer; x < position.x + halfWidth + buffer; x++)
            {
                indices.Add(new Vector2Int(x, y));
            }
        }
    }

    private void GetIndicesInRectangle(ref List<Vector3Int> indices, int buffer = 0)
    {
        for (int y = position.y - halfHeight - buffer; y < position.y + halfHeight + buffer; y++)
        {
            for (int x = position.x - halfWidth - buffer; x < position.x + halfWidth + buffer; x++)
            {
                indices.Add(new Vector3Int(x, y, 0));
            }
        }
    }

    private void GetIndicesInEllipse(ref List<Vector2Int> indices, int buffer = 0)
    {
        for (int y = position.y - halfHeight - buffer; y <= position.y + halfHeight + buffer; y++)
        {
            for (int x = position.x - halfWidth - buffer; x <= position.x + halfWidth + buffer; x++)
            {
                if (isPointInEllipse(new Vector2Int(x, y)))
                {
                    indices.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    private void GetIndicesInEllipse(ref List<Vector3Int> indices, int buffer = 0)
    {
        for (int y = position.y - halfHeight - buffer; y <= position.y + halfHeight + buffer; y++)
        {
            for (int x = position.x - halfWidth - buffer; x <= position.x + halfWidth + buffer; x++)
            {
                if (isPointInEllipse(new Vector2Int(x, y)))
                {
                    indices.Add(new Vector3Int(x, y, 0));
                }
            }
        }
    }

    private bool isPointInEllipse(Vector2Int point)
    {
        // checking the equation of 
        // ellipse with the given point 
        float p = (Mathf.Pow((point.x - position.x), 2) /
                 Mathf.Pow(halfWidth, 2)) +
                (Mathf.Pow((point.y - position.y), 2) /
                 Mathf.Pow(halfHeight, 2));

        return p <= 1.0f;
    }

    private bool isPointInRectangle(Vector2Int point)
    {
        return
            point.x <= position.x + halfWidth &&
            point.x >= position.x - halfWidth &&
            point.y <= position.y + halfHeight &&
            point.y >= position.y - halfHeight;
    }
}
