using System.Net.Http.Json;
using ResultService.Models;

namespace ResultService.Services;

public class VoteClient
{
    private readonly HttpClient _httpClient;

    public VoteClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ReferendumSummaryFromVoteService?> GetReferendumSummaryAsync(
        int idReferendum,
        CancellationToken ct)
    {
        return await _httpClient.GetFromJsonAsync<ReferendumSummaryFromVoteService>(
            $"/api/votes/referendums/{idReferendum}/summary",
            ct);
    }
}