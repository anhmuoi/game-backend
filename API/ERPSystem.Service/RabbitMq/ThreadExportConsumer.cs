using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using AutoMapper;
using ClosedXML.Excel;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.Repository;
using ERPSystem.Service.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace ERPSystem.Service.RabbitMq
{
    public class ThreadExportConsumer
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger _logger;

        private readonly HttpContext? _httpContext;

        private readonly IMapper _mapper;

        public ThreadExportConsumer(
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor,
            IMapper mapper
        )
        {
            _configuration = configuration;
            _httpContext = contextAccessor.HttpContext;
            _mapper = mapper;
            _logger =
                ApplicationVariables
                    .LoggerFactory
                    .CreateLogger<ThreadExportConsumer>();
        }

        public void DoWork(byte[] data)
        {
            var dataInfo = ParseData(data);
            if (dataInfo == null)
            {
                // something wrong data in queue
                return;
            }

            try
            {
                ThreadExportReport (dataInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
            }
        }

        private ExportToFileDetailData ParseData(byte[] data)
        {
            try
            {
                var jsonString = Encoding.UTF8.GetString(data);
                var result =
                    JsonConvert
                        .DeserializeObject
                        <ThreadExportProtocolData>(jsonString);
                return result.Data;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        private void ThreadExportReport(ExportToFileDetailData data)
        {
            string messageError = "";
            IUnitOfWork _unitOfWork =
                HelperService.CreateUnitOfWork(_configuration);
            var meetingRoom =
                _unitOfWork.MeetingRoomRepository.GetById(data.Room.Id);
            if (data.Room.IsRunning == false)
            {
                meetingRoom.CurrentPeople += 1;

                var userIdList = new List<int>();

                if (!string.IsNullOrEmpty(meetingRoom.UserListId))
                {
                    var currentUser =
                        JsonConvert
                            .DeserializeObject<List<int>>(meetingRoom
                                .UserListId);
                    if (currentUser != null)
                    {
                        foreach (var userId in currentUser)
                        {
                            userIdList.Add (userId);
                        }
                    }
                }
                if (userIdList.Contains(data.UserId))
                {
                    meetingRoom.CurrentPeople -= 1;
                }
                else
                {
                    userIdList.Add(data.UserId);
                }
                meetingRoom.UserListId = JsonConvert.SerializeObject(userIdList);
            }
            meetingRoom.IsRunning = data.Room.IsRunning;
            if (meetingRoom.CurrentPeople <= meetingRoom.TotalPeople)
            {
                _unitOfWork.MeetingRoomRepository.Update (meetingRoom);
                _unitOfWork.Save();
            }
            else
            {
                messageError = "Room is fully";
            }

            IQueueService queueService = new QueueService(_configuration);

            // Add notification
            string content = "";

            if (string.IsNullOrEmpty(messageError))
            {
                content = "";
            }
            else
            {
                content = messageError;
            }

            // push message
            string topic = $"{Constants.RabbitMqConfig.NotificationTopic}";
            NotificationProtocolData message =
                new NotificationProtocolData()
                {
                    MsgId = Guid.NewGuid().ToString(),
                    Type = Constants.Protocol.Notification,
                    Data =
                        new NotificationProtocolDataDetail()
                        {
                            MessageType =
                                string.IsNullOrEmpty(messageError)
                                    ? "info"
                                    : "error",
                            Message = content,
                            NotificationType = 1,
                            Room = meetingRoom,
                            UserId = data.UserId
                        }
                };
            queueService.Publish(topic, message.ToString());

            _unitOfWork.Dispose();
            queueService.Dispose();
        }
    }
}
