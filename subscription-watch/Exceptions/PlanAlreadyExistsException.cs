namespace subscription_watch.Exceptions
{
    public class PlanAlreadyExistsException : Exception
    {
        public PlanAlreadyExistsException(string title)
        : base($"Plan with title '{title}' already exists") { }
    }
}
