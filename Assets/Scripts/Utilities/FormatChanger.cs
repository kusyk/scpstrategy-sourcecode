using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormatChanger : MonoBehaviour
{
    public static string TimeToString(int value)
    {
        string tempString = value.ToString();

        if (tempString.Length <= 1)
            tempString = "0" + tempString;

        return tempString;
    }

    public static string HugeNumerToString(int value)
    {
        return HugeNumerToString(value.ToString());
    }

    public static string HugeNumerToString(uint value)
    {
        return HugeNumerToString(value.ToString());
    }

    public static string HugeNumerToString(long value)
    {
        return HugeNumerToString(value.ToString());
    }

    public static string HugeNumerToString(ulong value)
    {
        return HugeNumerToString(value.ToString());
    }

    public static string HugeNumerToString(string value)
    {
        for (int i = 1; i < value.Length; i++)
        {
            int tempPosition = value.Length - (i + (i / 3) - 1);
            if (i % 3 == 0 && tempPosition > 0)
            {
                value = value.Insert(tempPosition, " ");
            }
        }

        return value;
    }

    public static string VectorToCoordinates(Vector2 vector)
    {
        string tempString = "";

        tempString += DecimalToDegrees(vector.x);

        if (vector.x < 0)
            tempString += "S ";
        else
            tempString += "N ";

        tempString += DecimalToDegrees(vector.y);

        if (vector.y < 0)
            tempString += "W";
        else
            tempString += "E";

        return tempString;
    }

    public static float Round(float value, int nAfterPoints)
    {
        float helper = Mathf.Pow(10, nAfterPoints);
        value *= helper;
        value = (int)value;
        value /= helper;
        return value;
    }

    private static string DecimalToDegrees(float value)
    {
        value = Mathf.Abs(value);
        int sec = (int)Mathf.Round(value * 3600);
        int deg = sec / 3600;
        sec = Mathf.Abs(sec % 3600);
        int min = sec / 60;
        sec %= 60;

        return deg + "°" + min + "'" + sec + "\"";
    }
}
