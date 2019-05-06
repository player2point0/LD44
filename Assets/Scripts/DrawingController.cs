using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrawingController : MonoBehaviour
{
    public TextAsset Drawings;
    public GameObject lineObject;
    public float width;
    public float scale;
    public int currentIndex;

    private string[] jsonArray;
    
    void Start()
    {
        jsonArray = Drawings.text.Split('\n');
        drawRandomDrawing(jsonArray);
    } 

    public void drawRandomDrawing(string[] jsonArray)
    {
        currentIndex = Random.Range(0, jsonArray.Length-1);
        string bee = jsonArray[currentIndex];

        getDrawingData(bee);
    }

    public void getDrawingData(string json)
    {
        json = json.Replace('{', ' ');
        json = json.Replace('}', ' ');
        int startIndex = json.IndexOf("drawing") + 9;
        json = json.Substring(startIndex);

        string[][] data = arrayMaker(json);

        drawDrawingLines(data);
    }

    private Vector2 centrePoint(string[][] data)
    {
        float xMin = 100000;
        float xMax = -100000;
        float yMin = 100000;
        float yMax = -100000;

        for (int i = 0; i < data.Length - 1; i += 2)
        {
            string[] xPoints = data[i];
            string[] yPoints = data[i + 1];
            int pointsLength = xPoints.Length;
            List<Vector2> drawingPoints = new List<Vector2>();

            for (int j = 0; j < pointsLength; j++)
            {
                float x;
                float y;

                float.TryParse(xPoints[j], out x);
                float.TryParse(yPoints[j], out y);

                x *= scale;
                y *= scale;

                x += this.transform.position.x;
                y += this.transform.position.y;

                if (x < xMin) xMin = x;
                else if (x > xMax) xMax = x;

                if (y < yMin) yMin = y;
                else if (y > yMax) yMax = y;
            }
        }

        float xDiff = xMax - xMin;
        float yDiff = yMax - yMin;

        return new Vector2(xDiff / 2.0f, yDiff / 2.0f);
    }

    public void drawDrawingLines(string[][] data)
    {
        Vector2 centre = centrePoint(data);

        for (int i = 0; i < data.Length - 1; i += 2)
        {
            string[] xPoints = data[i];
            string[] yPoints = data[i + 1];
            int pointsLength = xPoints.Length;
            List<Vector2> drawingPoints = new List<Vector2>();

            for (int j = 0; j < pointsLength; j++)
            {
                float x;
                float y;

                float.TryParse(xPoints[j], out x);
                float.TryParse(yPoints[j], out y);

                x *= scale;
                y *= scale;

                x += this.transform.position.x;
                y += this.transform.position.y;

                Vector2 pos = new Vector2(x, y);

                drawingPoints.Add(pos - centre);
            }
            spawnLine(drawingPoints);
        }
    }

    private void spawnLine(List<Vector2> drawingPoints)
    {
        Vector2 spawnPos = calcSpawnPos(drawingPoints);
        GameObject temp = Instantiate(lineObject, spawnPos, Quaternion.identity, this.transform);
        LineController drawable = temp.GetComponent<LineController>();

        drawable.setUp(drawingPoints, width);
    }

    private Vector2 calcSpawnPos(List<Vector2> drawingPoints)
    {
        //center the points
        float xMax = -1000000;
        float xMin = 1000000;
        float yMax = -1000000;
        float yMin = 1000000;

        foreach (Vector2 v in drawingPoints)
        {
            if (v.x > xMax) xMax = v.x;
            else if (v.x < xMin) xMin = v.x;

            if (v.y > yMax) yMax = v.y;
            else if (v.y < yMin) yMin = v.y;
        }

        float xOffset = (xMax + xMin) / 2;
        float yOffset = (yMax + yMin) / 2;
        Vector2 offset = new Vector2(xOffset, yOffset);

        //modifies the drawingPoints input apparently
        for (int i = 0; i < drawingPoints.Count; i++)
        {
            drawingPoints[i] -= offset;
        }

        return offset;
    }

    public string[][] arrayMaker(string array)
    {
        List<string[]> tempArr = new List<string[]>();

        for (int i = 0; i < array.Length - 1; i++)
        {
            char nextChar = array[i + 1];

            if (nextChar != '[' && char.IsDigit(nextChar))
            {
                string arr = "";

                for (int j = i; j < array.Length; j++)
                {
                    char currentChar = array[j];

                    if (currentChar != ']')
                    {
                        arr += currentChar;
                    }

                    else
                    {
                        i = j;
                        break;
                    }
                }

                arr = arr.Replace('[', ' ');
                arr = arr.Replace(']', ' ');

                tempArr.Add(arr.Split(','));
            }
        }

        return tempArr.ToArray();
    }
}
