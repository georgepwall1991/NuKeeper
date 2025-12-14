using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using NuKeeper.Abstractions;
using NuKeeper.Abstractions.Logging;
using NuKeeper.Gitea.Model;

namespace NuKeeper.Gitea;

public class GiteaRestClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _client;
    private readonly INuKeeperLogger _logger;

    public GiteaRestClient(IHttpClientFactory clientFactory, string token, INuKeeperLogger logger,
        Uri apiBaseAddress)
    {
        _logger = logger;

        _client = clientFactory.CreateClient();
        _client.BaseAddress = apiBaseAddress;
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
    }

    /// <summary>
    ///     GET /user https://try.gitea.io/api/swagger#/user/userGetCurrent
    /// </summary>
    /// <returns>returns the current user</returns>
    public async Task<User> GetCurrentUser()
    {
        return await GetResource<User>("user").ConfigureAwait(false);
    }

    /// <summary>
    ///     GET /repos/{owner}/{repo} https://try.gitea.io/api/swagger#/repository/repoGet
    /// </summary>
    /// <param name="ownerName"></param>
    /// <param name="repositoryName"></param>
    /// <returns></returns>
    public async Task<Repository> GetRepository(string ownerName, string repositoryName)
    {
        var encodedProjectName = $"{ownerName}/{repositoryName}";
        return await GetResource<Repository>($"repos/{encodedProjectName}").ConfigureAwait(false);
    }

    /// <summary>
    ///     GET /admin/orgs https://try.gitea.io/api/swagger#/admin/adminGetAllOrgs
    /// </summary>
    /// <returns></returns>
    public async Task<List<Organization>> GetOrganizations()
    {
        return await GetResource<List<Organization>>("admin/orgs").ConfigureAwait(false);
    }

    /// <summary>
    ///     /GET /orgs/{org}/repos List an organization's repos https://try.gitea.io/api/swagger#/organization/orgListRepos
    /// </summary>
    /// <param name="orgaName">name of the organization</param>
    /// <returns>list of repos </returns>
    public async Task<List<Repository>> GetOrganizationRepositories(string orgaName)
    {
        return await GetResource<List<Repository>>($"/orgs/{orgaName}/repos").ConfigureAwait(false);
    }

    /// <summary>
    ///     /GET /repos/{owner}/{repo}/branches/{branch} https://try.gitea.io/api/swagger#/repository/repoGetBranch
    /// </summary>
    /// <param name="userName">the owner</param>
    /// <param name="repositoryName">the repo name</param>
    /// <param name="branchName">branch to check</param>
    public async Task<BranchInfo> GetRepositoryBranch(string userName, string repositoryName,
        string branchName)
    {
        var encodedProjectName = $"{userName}/{repositoryName}";
        var encodedBranchName = HttpUtility.UrlEncode(branchName);

        return await GetResource(
            $"repos/{encodedProjectName}/branches/{encodedBranchName}",
            statusCode => statusCode == HttpStatusCode.NotFound
                ? Result<BranchInfo>.Success(null)
                : Result<BranchInfo>.Failure());
    }

    /// <summary>
    ///     /POST /repos/{owner}/{repo}/forks https://try.gitea.io/api/swagger#/repository/createFork
    /// </summary>
    /// <param name="ownerName">owner of the repo to fork</param>
    /// <param name="repositoryName">name of the repo to fork</param>
    /// <returns></returns>
    public Task<Repository> ForkRepository(string ownerName, string repositoryName, string organizationName)
    {
        var encodedProjectName = $"{ownerName}/{repositoryName}";
        var content = new StringContent(JsonSerializer.Serialize(new ForkInfo(organizationName), JsonOptions),
            Encoding.UTF8,
            "application/json");

        return PostResource<Repository>($"repos/{encodedProjectName}/forks", content);
    }


    /// <summary>
    ///     /POST /repos/{owner}/{repo}/pulls Create a pull request
    /// </summary>
    /// <param name="owner">owner of the repo</param>
    /// <param name="repositoryName">name of the repository</param>
    /// <returns></returns>
    public async Task<IEnumerable<PullRequest>> GetPullRequests(
        string owner,
        string repositoryName,
        string headBranch,
        string baseBranch)
    {
        // we cannot filter on branch so we have to get them all and find the branches
        var encodedProjectName = $"{owner}/{repositoryName}";
        var response = new List<PullRequest>();
        var prReceived = 0;
        var page = 1;
        do
        {
            var result =
                await GetResource<List<PullRequest>>($"repos/{encodedProjectName}/pulls?state=open&page={page}");
            response.AddRange(result);
            prReceived = result.Count;
            page++;
        } while (prReceived > 0);

        return response.Where(pr =>
            pr.Base.Ref.Equals(baseBranch, StringComparison.InvariantCultureIgnoreCase)
            && pr.Head.Ref.Equals(headBranch, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    ///     /POST /repos/{owner}/{repo}/pulls Create a pull request
    /// </summary>
    /// <param name="owner">owner of the repo</param>
    /// <param name="repositoryName">name of the repository</param>
    /// <param name="pullRequest">pull request information</param>
    /// <returns></returns>
    public async Task<PullRequest> OpenPullRequest(string owner, string repositoryName,
        CreatePullRequestOption pullRequest)
    {
        var encodedProjectName = $"{owner}/{repositoryName}";

        var content = new StringContent(JsonSerializer.Serialize(pullRequest, JsonOptions), Encoding.UTF8,
            "application/json");
        return await PostResource<PullRequest>($"repos/{encodedProjectName}/pulls", content).ConfigureAwait(false);
    }

    private async Task<T> GetResource<T>(string url, Func<HttpStatusCode, Result<T>> customErrorHandling = null,
        [CallerMemberName] string caller = null)
        where T : class
    {
        var fullUrl = new Uri(url, UriKind.Relative);
        _logger.Detailed($"{caller}: Requesting {fullUrl}");

        var response = await _client.GetAsync(fullUrl).ConfigureAwait(false);
        return await HandleResponse(response, customErrorHandling, caller).ConfigureAwait(false);
    }

    private async Task<T> PostResource<T>(string url, HttpContent content,
        Func<HttpStatusCode, Result<T>> customErrorHandling = null, [CallerMemberName] string caller = null)
        where T : class
    {
        _logger.Detailed($"{caller}: Requesting {url}");

        var response = await _client.PostAsync(url, content).ConfigureAwait(false);
        return await HandleResponse(response, customErrorHandling, caller).ConfigureAwait(false);
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response,
        Func<HttpStatusCode, Result<T>> customErrorHandling,
        [CallerMemberName] string caller = null) where T : class
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
                    msg = $"{caller}: Error {response.StatusCode}";
                    _logger.Error($"{caller}: Error {response.StatusCode}");
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
