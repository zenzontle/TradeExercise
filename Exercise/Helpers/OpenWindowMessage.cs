namespace Exercise.Helpers
{
    public class OpenWindowMessage<T>
    {
        public WindowType Type { get; set; }
        public T Parameter { get; set; }
    }
}