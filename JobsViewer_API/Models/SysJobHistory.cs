namespace JobsViewer_API.Models
{
    public class SysJobHistory
    {
        public int instance_Id { get; set; }
        public int Run_Date { get; set; }
        public int Run_Time { get; set; }
        public Guid job_id { get; set; }
        public string job_name { get; set; }
        public string Message { get; set; }
        public int Run_Status { get; set; }
        public int Run_Duration { get; set; }
        public string Server { get; set; }
        public string Step_Name { get; set; }

    }
}
