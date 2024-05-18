﻿using ERPSystem.DataAccess.Models;

namespace ERPSystem.Service.Protocol
{
    public class NotificationProtocolData : ProtocolData<NotificationProtocolDataDetail>
    {
    }

    public class NotificationProtocolDataDetail
    {
        public string MessageType { get; set; }
        public int NotificationType { get; set; }
        public string Message { get; set; }
        public MeetingRoom Room { get; set; }
        public int UserId { get; set; }
    }


    public class NotificationVisitorProtocolData : ProtocolData<NotificationVisitorProtocolDataDetail>
    {
    }

    public class NotificationVisitorProtocolDataDetail
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public string CreatedOn { get; set; }
    }
    
    public class NotificationToUserProtocolData : ProtocolData<NotificationToUserProtocolDataDetail>
    {
    }

    public class NotificationToUserProtocolDataDetail
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public string CreatedOn { get; set; }
    }
}
