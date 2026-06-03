namespace Karakatsiya.Data.Entities.Showcase
{
    public class EventPerformer
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public Guid PerformerId { get; set; }
        public Performer? Performer { get; set; }
    }
}
