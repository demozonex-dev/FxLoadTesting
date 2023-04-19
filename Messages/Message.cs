namespace Messages
{
    public class DemoMessage
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public DateTime? InjectorDate { get; set; }
        public DateTime? ReceiverDate { get; set; }
        public string? Injector { get; set; }
        public string? Receiver { get; set; }
        public string? Host { get; set; }

        public string? ClientIP { get; set; }
        public string? ForwardedFor { get; set;}
    }
}


