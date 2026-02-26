namespace subscription_watch.Exceptions
{
    public class PlanAlreadyActiveException : Exception
    {
        public PlanAlreadyActiveException(string title)
        : base($"Plan with title '{title}' already active") { }
    }
}
