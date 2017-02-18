﻿using log4net;

namespace Utility
{
    public enum UserItemEnums
    {
        UpdateName = 0,
        UpdateUserName,
        UpdateEmail,
        UpdateBirthDate,
        UpdateNumberOfTrips,
        UpdateTrips,
        UpdateMilesTraveled,
        UpdatePassword,
        UpdateFriends,
        InvalidUpdate,
    }

    public enum TripItemEnums
    {
        UpdateTripName = 0,
        UpdateParticipants,
        UpdateTripMileage,
        InvalidUpdate,
    }

    public enum FolloMeErrorCodes
    {
        NoAccountExists = 0,
        AccountExists,
        APIVerified,
        AWSVerified,
        AWSAndAPIVerified,
        NotAWSVerified,
        NotAPIVerified,
        Uninitialized,
        MultipleUsersFound,
        NoCredentialsProvided,
        NoError,
        LogInFailed,
        LogInSucceded,
    }


    public static class Tools
    {
        // logger used for log4net debug logging on IIS
        public static readonly ILog logger = LogManager.GetLogger(typeof(Tools));

        public static UserItemEnums GetUserUpdateEnum(string val)
        {
            switch (val)
            {
                case "Name":
                    return UserItemEnums.UpdateName;
                case "Email":
                    return UserItemEnums.UpdateEmail;
                case "BirthDate":
                    return UserItemEnums.UpdateBirthDate;
                case "UserName":
                    return UserItemEnums.UpdateUserName;
                case "NumberOfTrips":
                    return UserItemEnums.UpdateNumberOfTrips;
                case "Password":
                    return UserItemEnums.UpdatePassword;
                case "TotalMilesTraveled":
                    return UserItemEnums.UpdateMilesTraveled;
                case "TripIds":
                    return UserItemEnums.UpdateTrips;
                case "Friends":
                    return UserItemEnums.UpdateFriends;
                default:
                    return UserItemEnums.InvalidUpdate;
            }
        }

        public static TripItemEnums GetTripItemEnum(string val)
        {
            switch (val)
            {
                case "Name":
                    return TripItemEnums.UpdateTripName;
                case "TripMile":
                    return TripItemEnums.UpdateTripMileage;
                case "Participants":
                    return TripItemEnums.UpdateParticipants; 
                default:
                    return TripItemEnums.InvalidUpdate;
            }
        }
    }

    public class FolloMeResponse
    {
        public int ErrorCode { get; set; }
        public FolloMeErrorCodes FolloMeErrorCode { get; set; }
        public string Message { get; set; }

        public FolloMeResponse()
        {
            ErrorCode = 000;
            FolloMeErrorCode = 000;
            Message = "Uninitialized error message";
        }

        public FolloMeResponse(int err, FolloMeErrorCodes FMerr, string msg)
        {
            ErrorCode = err;
            FolloMeErrorCode = FMerr;
            Message = msg;
        }
    }

}