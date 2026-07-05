public record EligibilityResponse(
    int IdReferendum,
    int IdQuestion,
    int IdVotante,
    bool Asignado,
    bool HaVotado,
    bool PuedeVotar,
    string Mensaje
);