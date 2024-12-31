using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This file contains a collection of auxiliary functions which are not related and can be used globally
 */

public static class Helper
{
    public static Vector3 GetCursorWorldPosition(float z)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        position.z = z;
        return position;
    }

    public static Color ChangeHSVValue(Color rgbColor, float deltaHSVValue)
    {
        float h, s, v;
        Color.RGBToHSV(rgbColor, out h, out s, out v);
        v += deltaHSVValue;
        return Color.HSVToRGB(h, s, v);
    }

    public static void LookAtTarget(GameObject gameObject, GameObject target, float angleTweak = -(Mathf.PI / 2))
    {
        Vector2 v = target.transform.position - gameObject.transform.position;

        float x = v.x;
        float y = v.y;

        float sin = Mathf.Sin(angleTweak);
        float cos = Mathf.Cos(angleTweak);

        gameObject.transform.right = new Vector2(cos * x - sin * y, sin * x + cos * y);
    }

    public static Vector2 Rotate(Vector2 vector, float angleDegrees)
    {
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        float x = vector.x;
        float y = vector.y;

        float sin = Mathf.Sin(angleRadians);
        float cos = Mathf.Cos(angleRadians);

        return new Vector2(cos * x - sin * y, sin * x + cos * y);
    }
}
