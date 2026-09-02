namespace RoomsReservation.Services
{
    public interface IEmailService
    {
        Task<bool> SendAsync(string to, string subject, string htmlBody);
    }
}
