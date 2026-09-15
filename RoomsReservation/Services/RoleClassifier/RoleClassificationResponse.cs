namespace RoomsReservation.Services.RoleClassifier;

public class RoleClassificationResponse
{
    public bool Allowed { get; set; }

    public string? Role { get; set; }

    public double Confidence { get; set; }

    public string Reason { get; set; } = "";
}
