using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using NuKeeper.Abstractions;
using NuKeeper.Abstractions.Logging;
using NuKeeper.Gitlab.Model;

namespace NuKeeper.Gitlab;

public class GitlabRestClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _client;
    private readonly INuKeeperLogger _logger;

    public GitlabRestClient(IHttpClientFactory clientFactory, string token, INuKeeperLogger logger, Uri apiBaseAddress)
    {
        _logger = logger;

        _client = clientFactory.CreateClient();
        _client.BaseAddress = apiBaseAddress;
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Add("Private-Token", token);
    }

    // https://docs.gitlab.com/ee/api/users.html#for-normal-users-1
    // GET /user
    public async Task<User> GetCurrentUser()
    {
        return await GetResource<User>("user").ConfigureAwait(false);
    }

    // https://docs.gitlab.com/ee/api/projects.html#get-single-project
    // GET /projects/:id
    public async Task<Project> GetProject(string projectName, string repositoryName)
    {
        var encodedProjectName = HttpUtility.UrlEncode($"{projectName}/{repositoryName}");
        return await GetResource<Project>($"projects/{encodedProjectName}").ConfigureAwait(false);
    }

    // https://docs.gitlab.com/ee/api/branches.html#get-single-repository-branch
    // GET /projects/:id/repository/branches/:branch
    public async Task<Branch> CheckExistingBranch(string projectName, string repositoryName,
        string branchName)
    {
        var encodedProjectName = HttpUtility.UrlEncode($"{projectName}/{repositoryName}");
        var encodedBranchName = HttpUtility.UrlEncode(branchName);

        return await GetResource(
            $"projects/{encodedProjectName}/repository/branches/{encodedBranchName}",
            statusCode => statusCode == HttpStatusCode.NotFound
                ? Result<Branch>.Success(null)
                : Result<Branch>.Failure());
    }

    // https://docs.gitlab.com/ee/api/merge_requests.html#list-merge-requests
    // GET GET /projects/:id/merge_requests?state=opened&target_branch=<head>&source_branch=<base>
    public async Task<IEnumerable<MergeInfo>> GetMergeRequests(
        string projectName,
        string repositoryName,
        string headBranch,
        string baseBranch)
    {
        var encodedProjectName = HttpUtility.UrlEncode($"{projectName}/{repositoryName}");
        var encodedBaseBranch = HttpUtility.UrlEncode(baseBranch);
        var encodedHeadBranch = HttpUtility.UrlEncode(headBranch);

        return await GetResource(
            $"projects/{encodedProjectName}/merge_requests?state=opened&view=simple&source_branch={encodedHeadBranch}&target_branch={encodedBaseBranch}",
            statusCode => statusCode == HttpStatusCode.NotFound
                ? Result<IEnumerable<MergeInfo>>.Success(null)
                : Result<IEnumerable<MergeInfo>>.Failure());
    }

    // https://docs.gitlab.com/ee/api/merge_requests.html#create-mr
    // POST /projects/:id/merge_requests
    public Task<MergeRequest> OpenMergeRequest(string projectName, string repositoryName, MergeRequest mergeRequest)
    {
        var encodedProjectName = HttpUtility.UrlEncode($"{projectName}/{repositoryName}");

        var content = new StringContent(JsonSerializer.Serialize(mergeRequest, JsonOptions), Encoding.UTF8,
            "application/json");
        return PostResource<MergeRequest>($"projects/{encodedProjectName}/merge_requests", content);
    }

    private async Task<T> GetResource<T>(string url, Func<HttpStatusCode, Result<T>> customErrorHandling = null,
        [CallerMemberName] string caller = null)
    {
        var fullUrl = new Uri(url, UriKind.Relative);
        _logger.Detailed($"{caller}: Requesting {fullUrl}");

        var response = await _client.GetAsync(fullUrl).ConfigureAwait(false);
        return await HandleResponse(response, customErrorHandling, caller).ConfigureAwait(false);
    }

    private async Task<T> PostResource<T>(string url, HttpContent content,
        Func<HttpStatusCode, Result<T>> customErrorHandling = null, [CallerMemberName] string caller = null)
    {
        _logger.Detailed($"{caller}: Requesting {url}");

        var response = await _client.PostAsync(url, content).ConfigureAwait(false);

        return await HandleResponse(response, customErrorHandling, caller).ConfigureAwait(false);
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response,
        Func<HttpStatusCode, Result<T>> customErrorHandling,
        [CallerMemberName] string caller = null)
    {
        string msg;

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Detailed($"Response {response.StatusCode} is not success, body:\n{responseBody}");

            if (customErrorHandling != null)
            {
                var result = customErrorHandling(response.StatusCode);

                if (result.IsSuccessful)
                    return result.Value;
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    msg = $"{caller}: Unauthorised, ensure PAT has appropriate permissions";
                    _logger.Error(msg);
                    throw new NuKeeperException(msg);
                case HttpStatusCode.Forbidden:
                    msg = $"{caller}: Forbidden, ensure PAT has appropriate permissions";
                    _logger.Error(msg);
                    throw new NuKeeperException(msg);
                default:
                    msg = $"{caller}: Error {response.StatusCode}, {responseBody}";
                    _logger.Error(msg);
                    throw new NuKeeperException(msg);
            }
        }

        try
        {
            return JsonSerializer.Deserialize<T>(responseBody, JsonOptions);
        }
        catch (JsonException ex)
        {
            msg = $"{caller} failed to parse json to {typeof(T)}: {ex.Message}";
            _logger.Error(msg);
            throw new NuKeeperException($"Failed to parse json to {typeof(T)}", ex);
        }
    }
}
