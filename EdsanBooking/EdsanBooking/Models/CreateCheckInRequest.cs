﻿namespace EdsanBooking.Models
{
    public class CreateCheckInRequest
    {
        public CheckIn CheckIn { get; set; }
        public string FeatureName { get; set; }
        public string TypeName { get; set; }
        public int HourType { get; set; }
        public string pkgName { get; set; }
        public bool isNew { get; set; }
    }
}
