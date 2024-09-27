using Microsoft.AspNetCore.Mvc.Rendering;

namespace EdsanBooking.Utilities
{
    public static class RoomData
    {
        public static readonly List<SelectListItem> RoomFeatures = new List<SelectListItem>
    {
        new SelectListItem { Value = "Aircon", Text = "Aircon" },
        new SelectListItem { Value = "Ceiling Fan", Text = "Ceiling Fan" }
    };

        public static readonly List<SelectListItem> RoomTypes = new List<SelectListItem>
    {
        new SelectListItem { Value = "Single", Text = "Single" },
        new SelectListItem { Value = "Double", Text = "Double" }
    };

        public static readonly List<SelectListItem> ResortPackages = new List<SelectListItem>
    {
        new SelectListItem { Value = "Function Hall", Text = "Function Hall" },
        new SelectListItem { Value = "Room w/ Pool Access", Text = "Room w/ Pool Access" },
        new SelectListItem { Value = "Exclusive", Text = "Exclusive" },
        new SelectListItem { Value = "Open Cottage", Text = "Open Cottage" }
    };
    }
}
