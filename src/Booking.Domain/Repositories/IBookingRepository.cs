public interface IBookingRepository
{
    Booking? GetById(Guid id);
    void Add(Booking booking);
    IEnumerable<Booking> GetAll();
}