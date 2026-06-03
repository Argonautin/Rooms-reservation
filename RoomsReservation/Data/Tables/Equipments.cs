namespace RoomsReservation.Data.Tables
{
    public class Equipments
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; } = string.Empty;

        public List<RoomEquipment> RoomEquipments { get; set; } = new();
    }
}
