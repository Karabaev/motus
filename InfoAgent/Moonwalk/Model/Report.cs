namespace InfoAgent.Moonwalk.Model
{
    public class Report
    {
        public string report_name { get; set; }
        public int? generated_at { get; set; }
        public int? total_count { get; set; }
        public Serial[] serials { get; set; }
        public Movie[] movies { get; set; }
    }

}
