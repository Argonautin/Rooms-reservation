namespace RoomsReservation.Data.Tables
{
    public class Reservation
    {
        public int Id { get; set; }

        public DateOnly DateReservation { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int Time_ReservationId { get; set; }
        public Time_Reservation Time_Reservation { get; set; } = null!;

        // do wysyłania maila przypominającego o rezerwacji
        public bool ReminderSent { get; set; }
    }
}
