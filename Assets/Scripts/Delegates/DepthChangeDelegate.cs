public static class DepthChangeDelegate
{
    public delegate void Event(int currentDepth);
    public static Event OnEvent;

    public static void Raise(int currentDepth) => OnEvent?.Invoke(currentDepth);
}