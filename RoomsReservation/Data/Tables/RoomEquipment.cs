namespace RoomsReservation.Data.Tables
{
    public class RoomEquipment
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int EquipmentId { get; set; }
        public bool IsPresent { get; set; }
    }
}
