namespace ERPSystem.Service.Protocol
{
    public class ThreadExportProtocolData : ProtocolData<ExportToFileDetailData>
    {
        public string Type { get; set; }
    }

    public class ExportToFileDetailData
    {
        public Room Room { get; set; }
        public int UserId { get; set; }
    }

    public class Room
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRunning { get; set; }
        public int TotalPeople { get; set; }
        public int CurrentPeople { get; set; }
        public double Price { get; set; }

    }
    
}


