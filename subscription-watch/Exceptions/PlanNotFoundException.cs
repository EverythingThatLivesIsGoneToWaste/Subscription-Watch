namespace subscription_watch.Exceptions
{
    public class PlanNotFoundException : Exception
    {
        public PlanNotFoundException(int id)
        : base($"Plan with id '{id}' not found") { }
    }
}
