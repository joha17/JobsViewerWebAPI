namespace JobViewerWebApp.Models
{
    public class SysJob
    {
        public Guid Job_Id { get; set; }
        public string Name { get; set; }
        public byte Enabled { get; set; }
        public DateTime Date_Created { get; set; }
        public DateTime Date_Modified { get; set; }
        public string Server { get; set; }
    }
}
