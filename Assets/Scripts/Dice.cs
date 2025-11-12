using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Transform diceVisual;
    public float spinDuration = 1.0f;

    public int Roll()
    {
        return Random.Range(1, 7);
    }

    private static readonly Vector3[] FaceEulerAngles =
    {
        Vector3.zero,              // unused (index 0)
        new Vector3(270f, 0f, 0f), // face 1
        new Vector3(180f, 0f, 0f), // face 2
        new Vector3(0f, 90f, 0f),  // face 3
        new Vector3(0f, 270f, 0f), // face 4
        new Vector3(0f, 0f, 0f),   // face 5
        new Vector3(90f, 0f, 0f)   // face 6
    };

    public IEnumerator PlayRollAnimation(System.Action<int> callback)
    {
        if (diceVisual == null)
        {
            int r = Roll();
            callback?.Invoke(r);
            yield break;
        }

        float t = 0f;
        int targetResult = Roll();
        Vector3 targetEuler = GetEulerForResult(targetResult);
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        while (t < spinDuration)
        {
            t += Time.deltaTime;
            diceVisual.Rotate(Vector3.up * 720f * Time.deltaTime, Space.World);
            diceVisual.Rotate(Vector3.right * 360f * Time.deltaTime, Space.World);
            yield return null;
        }

        diceVisual.rotation = targetRotation;
        Vector3 normalizedEuler = diceVisual.eulerAngles;
        normalizedEuler.x = NormalizeAngle(normalizedEuler.x);
        normalizedEuler.y = NormalizeAngle(normalizedEuler.y);
        normalizedEuler.z = 0f;
        diceVisual.rotation = Quaternion.Euler(normalizedEuler);

        int result = GetResultFromEuler(normalizedEuler);
        if (result < 1 || result > 6)
        {
            result = targetResult;
        }
        callback?.Invoke(result);
    }

    private static Vector3 GetEulerForResult(int result)
    {
        int index = Mathf.Clamp(result, 1, 6);
        return FaceEulerAngles[index];
    }

    private static int GetResultFromEuler(Vector3 euler)
    {
        float x = NormalizeAngle(euler.x);
        float y = NormalizeAngle(euler.y);
        float z = NormalizeAngle(euler.z);

        if (!Approximately(z, 0f))
        {
            return -1;
        }

        if (Approximately(x, 90f))
        {
            return 6;
        }

        if (Approximately(x, 270f))
        {
            return 1;
        }

        if (Approximately(x, 180f) || Approximately(y, 180f))
        {
            return 2;
        }

        if (Approximately(y, 90f))
        {
            return 3;
        }

        if (Approximately(y, 270f))
        {
            return 4;
        }

        if (Approximately(x, 0f) && Approximately(y, 0f))
        {
            return 5;
        }

        return -1;
    }

    private static float NormalizeAngle(float angle)
    {
        return Mathf.Repeat(angle, 360f);
    }

    private static bool Approximately(float a, float b)
    {
        return Mathf.Abs(Mathf.DeltaAngle(a, b)) <= 1f;
    }
}
