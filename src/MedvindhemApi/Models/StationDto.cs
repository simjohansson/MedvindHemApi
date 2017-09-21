namespace MedvindhemApi.Models
{
    public class StationDto
    {
        public Coordinate Coordinate { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string WindDirection { get; set; }
        public string WindSpeed { get; set; }
    }
}
