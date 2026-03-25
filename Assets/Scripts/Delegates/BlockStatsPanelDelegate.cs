public class BlockStatsPanelDelegate<T>
{
    public delegate void Event(T evt);
    public static Event OnEvent;

    public static void Raise(T evt) => OnEvent?.Invoke(evt);
}