using Microsoft.EntityFrameworkCore;
using RoomsReservation.Data;


namespace RoomsReservation.Services
{
    public class ReservationReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReservationReminderService> _logger;

        public ReservationReminderService(
            IServiceScopeFactory scopeFactory,
            ILogger<ReservationReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    // Rezerwacje, które zaczynają się za 24h i jeszcze nie wysłaliśmy maila
                    var dueReservations = db.Reservations
                        .Include(r => r.User)
                        .Where(r => !r.ReminderSent
                            && r.DateReservation <= DateOnly.FromDateTime(DateTime.Now.AddDays(1))
                            && r.DateReservation > DateOnly.FromDateTime(DateTime.Now))
                        .ToList();

                    foreach (var reservation in dueReservations)
                    {
                        bool ok = await emailService.SendAsync(
                            to: "reservationroom123@gmail.com",
                            subject: "Przypomnienie: rezerwacja jutro",
                            htmlBody: $@"<h2>Cześć {reservation.User.FirstName}!</h2>
                                        <p>Twoja rezerwacja pokoju zaczyna się <strong>jutro</strong>.</p>"
                        );

                        if (ok)
                        {
                            reservation.ReminderSent = true;
                            _logger.LogInformation("Przypomnienie wysłane do {Email}", reservation.User.Email);
                        }
                    }

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd ReservationReminderService");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
