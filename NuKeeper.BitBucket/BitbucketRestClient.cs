using NuKeeper.Abstractions;
using NuKeeper.Abstractions.Logging;
using NuKeeper.BitBucket.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace NuKeeper.BitBucket
{
    public class BitbucketRestClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private readonly HttpClient _client;
        private readonly INuKeeperLogger _logger;

        public BitbucketRestClient(IHttpClientFactory clientFactory, INuKeeperLogger logger, string username,
            string appPassword, Uri apiBaseAddress)
        {
            _logger = logger;

            _client = clientFactory.CreateClient();
            _client.BaseAddress = apiBaseAddress;
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{appPassword}");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<T> GetResource<T>(string url, [CallerMemberName] string caller = null)
        {
            _logger.Detailed($"{caller}: Getting from BitBucket url {url}");
            var response = await _client.GetAsync(url).ConfigureAwait(false);
            return await HandleResponse<T>(response, caller).ConfigureAwait(false);
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response, [CallerMemberName] string caller = null)
        {
            string msg;
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.Detailed($"Response {response.StatusCode} is not success, body:\n{responseBody}");

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        msg = $"{caller}: Unauthorised, ensure username and app password are correct";
                        _logger.Error(msg);
                        throw new NuKeeperException(msg);

                    case HttpStatusCode.Forbidden:
                        msg = $"{caller}: Forbidden, ensure user has appropriate permissions";
                        _logger.Error(msg);
                        throw new NuKeeperException(msg);

                    default:
                        msg = $"{caller}: Error {response.StatusCode}";
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
                msg = $"{caller}: Failed to parse json to {typeof(T)}: {ex.Message}";
                _logger.Error(msg);
                throw new NuKeeperException(msg, ex);
            }
        }

        public async Task<IEnumerable<ProjectInfo>> GetProjects(string account)
        {
            var response = await GetResource<IteratorBasedPage<ProjectInfo>>($"teams/{account}/projects/").ConfigureAwait(false);
            return response.values;
        }

        public async Task<IEnumerable<Repository>> GetGitRepositories(string account)
        {
            var response = await GetResource<IteratorBasedPage<Repository>>($"repositories/{account}").ConfigureAwait(false);
            return response.values;
        }

        public async Task<Repository> GetGitRepository(string account, string repositoryName)
        {
            return await GetResource<Repository>($"repositories/{account}/{repositoryName}").ConfigureAwait(false);
        }

        public async Task<IEnumerable<Ref>> GetRepositoryRefs(string account, string repositoryId)
        {
            var response = await GetResource<IteratorBasedPage<Ref>>($"repositories/{account}/{repositoryId}/refs").ConfigureAwait(false);
            return response.values;
        }

        // https://developer.atlassian.com/bitbucket/api/2/reference/meta/filtering#query-pullreq
        public async Task<PullRequestsInfo> GetPullRequests(
            string account,
            string repositoryName,
            string headBranch,
            string baseBranch)
        {
            var filter = $"state =\"open\" AND source.branch.name = \"{headBranch}\" AND destination.branch.name = \"{baseBranch}\"";

            return await GetResource<PullRequestsInfo>($"repositories/{account}/{repositoryName}/pullrequests?q={HttpUtility.UrlEncode(filter)}").ConfigureAwait(false);
        }

        public async Task<PullRequest> CreatePullRequest(string account, string repositoryName, PullRequest request, [CallerMemberName] string caller = null)
        {
            var content = new StringContent(JsonSerializer.Serialize(request, JsonOptions), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"repositories/{account}/{repositoryName}/pullrequests", content).ConfigureAwait(false);
            return await HandleResponse<PullRequest>(response, caller).ConfigureAwait(false);
        }
    }
}
