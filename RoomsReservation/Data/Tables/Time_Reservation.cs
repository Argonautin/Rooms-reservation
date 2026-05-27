namespace RoomsReservation.Data.Tables
{
    public class Time_Reservation
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
