using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface IReservationService
    {
        Task<int> GetTotalReservationCountAsync(string searchTerm = null, string loc = null);
        Task<List<ReservationViewModel>> GetReservationsAsync(int pageNumber, string searchTerm = null, string loc = null);
        Task<Reservation> GetReservationByIdAsync(string id);
        Task<bool> ReservationExistsAsync(string id);
        Task<string> GenerateNextReservationIdAsync();
        Task<bool> ConfirmTransientReservationAsync(string id);
        Task<bool> ConfirmResortReservationAsync(string id);
        Task<decimal> GetTransientReservationTotalAmountAsync(string id, string location);
        Task<Reservation> SaveOrUpdateReservationAsync(Reservation reservation, string featureName, string typeName,
                                                 int hourType, string pkgName, bool isNew);
        Task<List<GuestDetailViewModel>> GetGuestsByReservationIdAsync(string reservationId);
        Task DeleteComReservedRecordsAsync(string reservationId);
    }
}
