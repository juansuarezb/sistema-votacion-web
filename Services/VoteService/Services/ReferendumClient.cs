using System.Net.Http.Json;
using VoteService.Models;

namespace VoteService.Services;

public class ReferendumClient
{
    private readonly HttpClient _httpClient;

    public ReferendumClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EligibilityResponse?> CheckEligibilityAsync(
        int idReferendum,
        int idQuestion,
        int idVotante,
        CancellationToken ct)
    {
        return await _httpClient.GetFromJsonAsync<EligibilityResponse>(
            $"/api/referendums/{idReferendum}/questions/{idQuestion}/voters/{idVotante}/eligibility",
            ct);
    }

    public async Task<bool> MarkVotedAsync(
        int idReferendum,
        int idQuestion,
        int idVotante,
        CancellationToken ct)
    {
        var response = await _httpClient.PatchAsync(
            $"/api/referendums/{idReferendum}/questions/{idQuestion}/voters/{idVotante}/mark-voted",
            content: null,
            ct);

        return response.IsSuccessStatusCode;
    }
}