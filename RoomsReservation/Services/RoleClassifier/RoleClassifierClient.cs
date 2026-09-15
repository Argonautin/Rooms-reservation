using System.Net.Http.Json;

namespace RoomsReservation.Services.RoleClassifier;

public class RoleClassifierClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RoleClassifierClient> _logger;

    public RoleClassifierClient(
        HttpClient httpClient,
        ILogger<RoleClassifierClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RoleClassificationResponse> ClassifyAsync(
        RoleClassificationRequest request)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "/classify-role",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<RoleClassificationResponse>();

            // Brak odpowiedzi nie może dopuścić użytkownika do rejestracji.
            return result ?? new RoleClassificationResponse
            {
                Allowed = false,
                Role = null,
                Confidence = 0,
                Reason = "Mikroserwis zwrócił pustą odpowiedź. Rejestracja została zablokowana."
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Nie udało się połączyć z mikroserwisem klasyfikacji ról.");

            // Fail closed: jeśli Clojure nie działa, konta nie tworzymy.
            return new RoleClassificationResponse
            {
                Allowed = false,
                Role = null,
                Confidence = 0,
                Reason =
                    "Usługa weryfikacji adresu e-mail jest obecnie niedostępna. " +
                    "Spróbuj ponownie później."
            };
        }
    }
}
