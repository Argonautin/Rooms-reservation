namespace RoomsReservation.Data.Tables
{
    public class Equipment
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; } = string.Empty;

        public List<RoomEquipment> RoomEquipments { get; set; } = new();
    }
}
