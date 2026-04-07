namespace RoomsReservation.Data.Tables
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public bool Islocked { get; set; }
        public string Professor { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
