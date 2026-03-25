using UnityEngine;

public static class PositionChangeDelegate
{
    public delegate void Event(int currentDepth, Vector3 playerPosition);
    public static Event OnEvent;

    public static void Raise(int currentDepth, Vector3 playerPosition) => OnEvent?.Invoke(currentDepth, playerPosition);
}