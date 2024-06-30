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
using ERPSystem.DataModel.MeetingLog;
using ERPSystem.DataModel.MeetingRoom;
using ERPSystem.Repository;
using ERPSystem.Service.Protocol;
using ERPSystem.Service.Services;
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

            if (data.Action.Id == (int) ActionType.Join)
            {
                if (data.Room.IsRunning == false)
                {
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
                                var user =
                                    _unitOfWork.UserRepository.GetById(userId);
                                if (user.InGame == 0)
                                {
                                    user.InGame = meetingRoom.Id;
                                    _unitOfWork.UserRepository.Update (user);
                                    _unitOfWork.Save();
                                }
                            }
                        }
                    }
                    if (userIdList.Contains(data.UserId))
                    {
                    }
                    else
                    {
                        var user =
                            _unitOfWork.UserRepository.GetById(data.UserId);
                        if (user.InGame == 0)
                        {
                            userIdList.Add(data.UserId);
                            if (userIdList.Count == 1)
                            {
                                user.OwnerRoom = meetingRoom.Id;
                            }

                            user.IndexPlayer = userIdList.Count;
                            user.InGame = meetingRoom.Id;
                            _unitOfWork.UserRepository.Update (user);
                            _unitOfWork.Save();
                        }
                        else
                        {
                        }
                    }
                    meetingRoom.UserListId =
                        JsonConvert.SerializeObject(userIdList);
                    meetingRoom.CurrentPeople = userIdList.Count;
                }
                else
                {
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
                    }
                    else
                    {
                        messageError = "Room is Running";
                    }
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
            }
            else if (data.Action.Id == (int) ActionType.CreateCustomRoom)
            {
                var newRoom = new MeetingRoom();
                newRoom.Name = data.Room.Name;
                newRoom.Description = data.Room.Description;
                newRoom.PasswordRoom = data.Room.PasswordRoom;
                newRoom.IsRunning = data.Room.IsRunning;
                newRoom.TotalPeople = data.Room.TotalPeople;
                newRoom.CurrentPeople = 0;
                newRoom.CurrentMeetingLogId = 0;
                newRoom.Price = data.Room.Price;
                newRoom.UserListId =
                    JsonConvert.SerializeObject(data.Room.UserListId);
                newRoom.Default = false;
                _unitOfWork.MeetingRoomRepository.Add (newRoom);
                _unitOfWork.Save();
                meetingRoom = newRoom;
            }
            else if (data.Action.Id == (int) ActionType.Out)
            {
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
                            if (userId != data.UserId)
                            {
                                userIdList.Add (userId);
                            }
                        }
                    }
                }

                var user = _unitOfWork.UserRepository.GetById(data.UserId);

                bool ownerOut = false;
                if (user.OwnerRoom == meetingRoom.Id)
                {
                    ownerOut = true;
                }
                user.InGame = 0;
                user.OwnerRoom = 0;
                user.IndexPlayer = 0;
                if (data.Room.IsRunning == true)
                {
                    user.TotalLose += 1;
                }
                _unitOfWork.UserRepository.Update (user);
                _unitOfWork.Save();

                // update index user in room
                if (userIdList.Count > 0)
                {
                    var userListTemp =
                        _unitOfWork
                            .UserRepository
                            .GetAll()
                            .Where(m => m.InGame == meetingRoom.Id)
                            .OrderBy(m => m.IndexPlayer)
                            .ToList();
                    var i = 0;
                    foreach (var userTmp in userListTemp)
                    {
                        if (ownerOut == true && i == 0)
                        {
                            userTmp.OwnerRoom = meetingRoom.Id;
                        }
                        userTmp.IndexPlayer = i + 1;
                        _unitOfWork.UserRepository.Update (userTmp);
                        _unitOfWork.Save();
                        i = i + 1;
                    }
                }

                meetingRoom.UserListId =
                    JsonConvert.SerializeObject(userIdList);
                meetingRoom.CurrentPeople = userIdList.Count;

                meetingRoom.IsRunning = data.Room.IsRunning;
                if (meetingRoom.CurrentPeople <= meetingRoom.TotalPeople)
                {
                    if (meetingRoom.CurrentPeople == 1)
                    {
                        meetingRoom.IsRunning = false;

                        data.Action.GamePlay.OrderWinning = userIdList;

                        if (data.Room.IsRunning == true)
                        {
                            var userWin =
                                _unitOfWork
                                    .UserRepository
                                    .GetById(userIdList.FirstOrDefault());
                            if (userWin != null)
                            {
                                userWin.TotalWin += 1;
                                _unitOfWork.UserRepository.Update (userWin);
                                _unitOfWork.Save();
                            }
                        }

                        var mtLog =
                            _unitOfWork
                                .MeetingLogRepository
                                .GetById(meetingRoom.CurrentMeetingLogId);

                        if (mtLog != null)
                        {
                            var userStatus =
                                string.IsNullOrEmpty(mtLog.UserList)
                                    ? new List<UserData>()
                                    : JsonConvert
                                        .DeserializeObject<List<UserData>>(mtLog
                                            .UserList);

                            data.Action.UserStatus = userStatus;
                            mtLog.GamePlay =
                                JsonConvert
                                    .SerializeObject(data.Action.GamePlay);
                            mtLog.EndDate = DateTime.UtcNow;
                            _unitOfWork.MeetingLogRepository.Update (mtLog);
                            _unitOfWork.Save();
                        }

                        data.Room.IsRunning = false;
                        meetingRoom.CurrentMeetingLogId = 0;
                    }
                    else if (
                        meetingRoom.CurrentPeople == 0 &&
                        meetingRoom.Default == true
                    )
                    {
                        meetingRoom.PasswordRoom = null;
                    }

                    _unitOfWork.MeetingRoomRepository.Update (meetingRoom);
                    _unitOfWork.Save();

                    if (
                        meetingRoom.CurrentPeople == 0 &&
                        meetingRoom.Default == false
                    )
                    {
                        meetingRoom.IsDelete = true;
                        _unitOfWork.MeetingRoomRepository.Update(meetingRoom);
                        _unitOfWork.Save();
                    }
                }
            }
            else if (data.Action.Id == (int) ActionType.StartFirst)
            {
                meetingRoom.IsRunning = false;

                // create meeting log
                var meetingLog = new MeetingLog();
                meetingLog.MeetingRoomId = meetingRoom.Id;
                meetingLog.Title = "";
                meetingLog.Content =
                    JsonConvert.SerializeObject(data.Action.Content);
                meetingLog.StartDate = DateTime.UtcNow;
                meetingLog.UserList =
                    JsonConvert.SerializeObject(data.Action.UserStatus);
                meetingLog.GamePlay =
                    JsonConvert.SerializeObject(data.Action.GamePlay);

                _unitOfWork.MeetingLogRepository.Add (meetingLog);
                _unitOfWork.Save();

                meetingRoom.CurrentMeetingLogId = meetingLog.Id;
                _unitOfWork.MeetingRoomRepository.Update (meetingRoom);
                _unitOfWork.Save();
                data.Action.MeetingLogId = meetingLog.Id;
            }
            else if (data.Action.Id == (int) ActionType.RejectCreateGame)
            {
                meetingRoom.IsRunning = false;

                _unitOfWork
                    .MeetingLogRepository
                    .Delete(m => m.Id == data.Action.MeetingLogId);
                _unitOfWork.Save();

                meetingRoom.CurrentMeetingLogId = 0;
                _unitOfWork.MeetingRoomRepository.Update (meetingRoom);
                _unitOfWork.Save();
            }
            else if (data.Action.Id == (int) ActionType.DoneCreateGame)
            {
                meetingRoom.IsRunning = false;
                data.Action.CreateBattleSuccess = true;

                // update meeting log
                if (data.Action.MeetingLogId != null)
                {
                    var mtLog =
                        _unitOfWork
                            .MeetingLogRepository
                            .GetById(data.Action.MeetingLogId);

                    var userStatus =
                        string.IsNullOrEmpty(mtLog.UserList)
                            ? new List<UserData>()
                            : JsonConvert
                                .DeserializeObject<List<UserData>>(mtLog
                                    .UserList);
                    if (userStatus == null || userStatus.Count == 0)
                    {
                        userStatus = new List<UserData>();
                    }
                    foreach (var user in data.Action.UserStatus)
                    {
                        if (userStatus.Where(x => x.Id == user.Id) != null)
                        {
                            userStatus =
                                userStatus.Where(x => x.Id != user.Id).ToList();
                            userStatus.Add (user);
                        }
                    }

                    mtLog.GamePlay =
                        JsonConvert.SerializeObject(data.Action.GamePlay);
                    mtLog.UserList = JsonConvert.SerializeObject(userStatus);
                    mtLog.CreateBattleSuccess = true;

                    _unitOfWork.MeetingLogRepository.Update (mtLog);
                    _unitOfWork.Save();

                    _unitOfWork.MeetingRoomRepository.Update (meetingRoom);
                    _unitOfWork.Save();
                }
            }
            else if (data.Action.Id == (int) ActionType.Deposit)
            {
                meetingRoom.IsRunning = false;

                // update meeting log
                if (data.Action.MeetingLogId != null)
                {
                    var mtLog =
                        _unitOfWork
                            .MeetingLogRepository
                            .GetById(data.Action.MeetingLogId);

                    var userStatus =
                        string.IsNullOrEmpty(mtLog.UserList)
                            ? new List<UserData>()
                            : JsonConvert
                                .DeserializeObject<List<UserData>>(mtLog
                                    .UserList);
                    if (userStatus == null || userStatus.Count == 0)
                    {
                        userStatus = new List<UserData>();
                    }
                    foreach (var user in data.Action.UserStatus)
                    {
                        if (userStatus.Where(x => x.Id == user.Id) != null)
                        {
                            userStatus =
                                userStatus.Where(x => x.Id != user.Id).ToList();
                            userStatus.Add (user);
                        }
                    }

                    mtLog.GamePlay =
                        JsonConvert.SerializeObject(data.Action.GamePlay);
                    mtLog.UserList = JsonConvert.SerializeObject(userStatus);
                    mtLog.CreateBattleSuccess = true;

                    if (
                        userStatus.Count() ==
                        userStatus.Count(m => m.IsDeposit == true)
                    )
                    {
                        mtLog.DepositDone = true;
                        data.Action.DepositDone = true;
                    }

                    _unitOfWork.MeetingLogRepository.Update (mtLog);
                    _unitOfWork.Save();

                    _unitOfWork.MeetingRoomRepository.Update (meetingRoom);
                    _unitOfWork.Save();
                }
            }
            else if (data.Action.Id == (int) ActionType.DoneReceiveReward)
            {
                data.Action.DoneReceiveReward = true;
            }
            else if (data.Action.Id == (int) ActionType.Chat)
            {
                // update meeting log
                if (data.Action.MeetingLogId != null)
                {
                    var mtLog =
                        _unitOfWork
                            .MeetingLogRepository
                            .GetById(data.Action.MeetingLogId);

                    mtLog.Content =
                        JsonConvert.SerializeObject(data.Action.Content);

                    _unitOfWork.MeetingLogRepository.Update (mtLog);
                    _unitOfWork.Save();
                }
            }
            else if (data.Action.Id == (int) ActionType.AddFriend)
            {
            }
            else if (data.Action.Id == (int) ActionType.AgreeAddFriend)
            {
            }
            else if (data.Action.Id == (int) ActionType.Kick)
            {
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
                            if (userId != data.Action.UserIdIsKick)
                            {
                                userIdList.Add (userId);
                            }
                        }
                    }
                }

                var user = _unitOfWork.UserRepository.GetById(data.Action.UserIdIsKick);

              
                user.InGame = 0;
                user.OwnerRoom = 0;
                user.IndexPlayer = 0;
                
                _unitOfWork.UserRepository.Update (user);
                _unitOfWork.Save();

                // update index user in room
                if (userIdList.Count > 0)
                {
                    var userListTemp =
                        _unitOfWork
                            .UserRepository
                            .GetAll()
                            .Where(m => m.InGame == meetingRoom.Id)
                            .OrderBy(m => m.IndexPlayer)
                            .ToList();
                    var i = 0;
                    foreach (var userTmp in userListTemp)
                    {
                        
                        userTmp.IndexPlayer = i + 1;
                        _unitOfWork.UserRepository.Update (userTmp);
                        _unitOfWork.Save();
                        i = i + 1;
                    }
                }

                meetingRoom.UserListId =
                    JsonConvert.SerializeObject(userIdList);
                meetingRoom.CurrentPeople = userIdList.Count;

                _unitOfWork.MeetingRoomRepository.Update(meetingRoom);
                _unitOfWork.Save();

            }
            else if (data.Action.Id == (int) ActionType.UpdateSettingRoom)
            {
                var room =
                    _unitOfWork.MeetingRoomRepository.GetById(data.Room.Id);
                room.Price = data.Room.Price;
                room.TotalPeople = data.Room.TotalPeople;
                room.Name = data.Room.Name;

                room.PasswordRoom = data.Room.PasswordRoom;
                _unitOfWork.MeetingRoomRepository.Update (room);
                _unitOfWork.Save();
            }
            else if (data.Action.Id == (int) ActionType.Start)
            {
                // update meeting log
                if (data.Action.MeetingLogId != null)
                {
                    var mtLog =
                        _unitOfWork
                            .MeetingLogRepository
                            .GetById(data.Action.MeetingLogId);

                    var userStatus =
                        string.IsNullOrEmpty(mtLog.UserList)
                            ? new List<UserData>()
                            : JsonConvert
                                .DeserializeObject<List<UserData>>(mtLog
                                    .UserList);
                    if (userStatus == null || userStatus.Count == 0)
                    {
                        userStatus = new List<UserData>();
                    }
                    foreach (var user in data.Action.UserStatus)
                    {
                        if (userStatus.Where(x => x.Id == user.Id) != null)
                        {
                            userStatus =
                                userStatus.Where(x => x.Id != user.Id).ToList();
                            userStatus.Add (user);
                        }
                    }

                    mtLog.GamePlay =
                        JsonConvert.SerializeObject(data.Action.GamePlay);
                    mtLog.UserList = JsonConvert.SerializeObject(userStatus);

                    mtLog.StartDate = DateTime.UtcNow;
                    _unitOfWork.MeetingLogRepository.Update (mtLog);
                    _unitOfWork.Save();

                    meetingRoom.IsRunning = true;
                    _unitOfWork.MeetingRoomRepository.Update (meetingRoom);
                    _unitOfWork.Save();
                }
            }
            else if (data.Action.Id == (int) ActionType.ChooseCard)
            {
                // create meeting log
                if (data.Action.MeetingLogId != null)
                {
                    var mtLog =
                        _unitOfWork
                            .MeetingLogRepository
                            .GetById(data.Action.MeetingLogId);

                    mtLog.GamePlay =
                        JsonConvert.SerializeObject(data.Action.GamePlay);
                    mtLog.UserList =
                        JsonConvert.SerializeObject(data.Action.UserStatus);

                    mtLog.StartDate = DateTime.UtcNow;
                    _unitOfWork.MeetingLogRepository.Update (mtLog);
                    _unitOfWork.Save();
                }
            }
            else if (data.Action.Id == (int) ActionType.End)
            {
                if (meetingRoom.IsRunning == true)
                {
                    meetingRoom.IsRunning = false;
                    meetingRoom.CurrentMeetingLogId = 0;
                    _unitOfWork.MeetingRoomRepository.Update (meetingRoom);
                    _unitOfWork.Save();

                    if (data.Action.GamePlay.OrderWinning != null)
                    {
                        foreach (var
                            playerId
                            in
                            data.Action.GamePlay.OrderWinning
                        )
                        {
                            if (playerId == data.Action.GamePlay.OrderWinning[0]
                            )
                            {
                                var userWin =
                                    _unitOfWork
                                        .UserRepository
                                        .GetById(playerId);
                                if (userWin != null)
                                {
                                    userWin.TotalWin += 1;
                                    _unitOfWork.UserRepository.Update (userWin);
                                    _unitOfWork.Save();
                                }
                            }
                            else
                            {
                                var userLose =
                                    _unitOfWork
                                        .UserRepository
                                        .GetById(playerId);
                                if (userLose != null)
                                {
                                    userLose.TotalLose += 1;
                                    _unitOfWork.UserRepository.Update (
                                        userLose
                                    );
                                    _unitOfWork.Save();
                                }
                            }
                        }
                    }

                    // update meeting log
                    var meetingLog =
                        _unitOfWork
                            .MeetingLogRepository
                            .GetById(data.Action.MeetingLogId);
                    meetingLog.GamePlay =
                        JsonConvert.SerializeObject(data.Action.GamePlay);

                    meetingLog.EndDate = DateTime.UtcNow;

                    _unitOfWork.MeetingLogRepository.Update (meetingLog);
                    _unitOfWork.Save();
                }
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

            var meetingResponse = new MeetingRoomResponseModel();
            if (meetingRoom != null)
            {
                meetingResponse.Id = meetingRoom.Id;
                meetingResponse.Name = meetingRoom.Name;
                meetingResponse.Description = meetingRoom.Description;
                meetingResponse.IsRunning = meetingRoom.IsRunning;
                meetingResponse.CurrentMeetingLogId =
                    meetingRoom.CurrentMeetingLogId;
                meetingResponse.TotalPeople = meetingRoom.TotalPeople;
                meetingResponse.Price = meetingRoom.Price;
                meetingResponse.CurrentPeople = meetingRoom.CurrentPeople;
                meetingResponse.UserListId =
                    JsonConvert
                        .DeserializeObject<List<int>>(meetingRoom.UserListId);
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
                            Room = meetingResponse,
                            UserId = data.UserId,
                            Action = data.Action
                        }
                };
            queueService.Publish(topic, message.ToString());

            _unitOfWork.Dispose();
            queueService.Dispose();
        }
    }
}
