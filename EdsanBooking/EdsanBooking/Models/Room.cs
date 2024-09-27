using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    /// <summary>
    /// Represents a room.
    /// </summary>
    [Table("room")]
    public class Room
    {
        [Key]
        [Column("roomid")]
        public string RoomId { get; set; }

        [Column("descr")]
        [Required(ErrorMessage = "Description is required")]
        public string Descr { get; set; }

        [Column("featurename")]
        public string FeatureName { get; set; } // New property

        [Column("typename")]
        public string TypeName { get; set; } // New property

        [Column("hourtype")]
        [Range(1, 24, ErrorMessage = "Hour Type must be between 1 and 24")]
        public int HourType { get; set; }

        //[Column("roomrate")]
        //[Required(ErrorMessage = "Room rate is required")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "Room rate must be greater than zero")]
        //public decimal RoomRate { get; set; }

        [Column("statusname")]
        public string StatusName { get; set; } // New property

        [Column("classification")]
        public string Classification { get; set; }

        [Column("remarks")]
        public string Remarks { get; set; }
        [Column("loc")]
        public string Location { get; set; }
    }



    /// <summary>
    /// Represents the room load for stored procedure.
    /// </summary>
    [Table("SPRoomLoad")]
    public class SPRoomLoad
    {
        [Key]
        [Column("roomid")]
        public string RoomId { get; set; }
        [Column("descr")]
        public string Descr { get; set; }

        [Column("roomfeatureid")]
        public int FeatureId { get; set; }

        [Column("featurename")]
        public string FeatureName { get; set; }

        [Column("roomtypeid")]
        public int TypeId { get; set; }

        [Column("typename")]
        public string TypeName { get; set; }

        [Column("hourtype")]
        public decimal HourType { get; set; }

        //[Column("roomrate")]
        //public decimal RoomRate { get; set; }

        [Column("statusid")]
        public int StatusId { get; set; }

        [Column("statusname")]
        public string StatusName { get; set; }

        [Column("remarks")]
        public string Remarks { get; set; }
    }

    /// <summary>
    /// Represents a room feature.
    /// </summary>
    [Table("roomfeature")]
    public class RoomFeature
    {
        [Key]
        [Column("roomfeatureid")]
        public int RoomFeatureId { get; set; }

        [Column("featurename")]
        public string FeatureName { get; set; }
    }

    /// <summary>
    /// Represents a room type.
    /// </summary>
    [Table("roomtype")]
    public class RoomType
    {
        [Key]
        [Column("roomtypeid")]
        public int RoomTypeId { get; set; }

        [Column("typename")]
        public string TypeName { get; set; }
    }

    /// <summary>
    /// Represents a room status.
    /// </summary>
    [Table("status")]
    public class RoomStatus
    {
        [Key]
        [Column("statusid")]
        public int StatusId { get; set; }

        [Column("statusname")]
        public string StatusName { get; set; }
    }
}
