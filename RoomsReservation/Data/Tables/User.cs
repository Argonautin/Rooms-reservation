namespace RoomsReservation.Data.Tables
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsStudent { get; set; }
        public bool IsProfessor { get; set; }

        public List<Reservation> Reservations { get; set; } = new();
    }
}
