﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Reflection;

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
    }

    public enum TripItemEnums
    {
        UpdateTripName = 0,
        UpdateTripMileage,
    }


    public static class Logger
    {
        // logger used for log4net debug logging on IIS
        public static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}