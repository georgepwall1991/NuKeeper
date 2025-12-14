using System.Text.RegularExpressions;
using NuKeeper.Abstractions.Git;
using NuKeeper.Abstractions.Logging;
using NuKeeper.Update.ProcessRunner;

namespace NuKeeper.Git;

public class GitCmdDiscoveryDriver : IGitDiscoveryDriver
{
    private readonly INuKeeperLogger _logger;
    private readonly string _pathGit;

    public GitCmdDiscoveryDriver(string pathToGit, INuKeeperLogger logger)
    {
        if (string.IsNullOrWhiteSpace(pathToGit)) throw new ArgumentNullException(nameof(pathToGit));

        if (Path.GetFileNameWithoutExtension(pathToGit) != "git")
            throw new InvalidOperationException($"Invalid path '{pathToGit}'. Path must point to 'git' cmd");

        _logger = logger;
        _pathGit = pathToGit;
    }

    public async Task<Uri> DiscoverRepo(Uri repositoryUri)
    {
        if (repositoryUri == null) throw new ArgumentNullException(nameof(repositoryUri));

        var result = await StartGitProcess("config --get remote.origin.url", true, repositoryUri.LocalPath)
            .ConfigureAwait(false);
        return new Uri(result);
    }

    public async Task<string> GetCurrentHead(Uri repositoryUri)
    {
        if (repositoryUri == null) throw new ArgumentNullException(nameof(repositoryUri));

        var getBranchHead = await StartGitProcess("symbolic-ref -q --short HEAD", true, repositoryUri.LocalPath)
            .ConfigureAwait(false);
        return string.IsNullOrEmpty(getBranchHead)
            ? await StartGitProcess("rev-parse HEAD", true, repositoryUri.LocalPath)
            : getBranchHead;
    }

    public async Task<GitRemote> GetRemoteForPlatform(Uri repositoryUri, string platformHost)
    {
        var remotes = await GetRemotes(repositoryUri).ConfigureAwait(false);
        return remotes.FirstOrDefault(rm => rm.Url.Host.Contains(platformHost, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<GitRemote>> GetRemotes(Uri repositoryUri)
    {
        if (repositoryUri == null) throw new ArgumentNullException(nameof(repositoryUri));

        if (!await IsGitRepo(repositoryUri)) return Enumerable.Empty<GitRemote>();

        var result = await StartGitProcess("remote -v", true, repositoryUri.LocalPath).ConfigureAwait(false);

        // result should look like "origin\thttps://github.com/nukeeper/NuKeeper.git (fetch)\norigin\thttps://github.com/nukeeper/NuKeeper.git (push)"
        if (!string.IsNullOrWhiteSpace(result))
        {
            var remoteList = new List<GitRemote>();
            var remotes = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var remote in remotes)
            {
                var gitRemote = CreateGitRemoteFromString(remote);
                if (gitRemote != null && !remoteList.Any(x => x.Name == gitRemote.Name)) remoteList.Add(gitRemote);
            }

            return remoteList;
        }

        return null;
    }

    public async Task<bool> IsGitRepo(Uri repositoryUri)
    {
        var discovered = await DiscoverRepo(repositoryUri).ConfigureAwait(false);
        if (discovered == null) return false;

        return true;
    }

    internal async Task<string> StartGitProcess(string arguments, bool ensureSuccess, string workingFolder)
    {
        var process = new ExternalProcess(_logger);
        var output = await process.Run(workingFolder, _pathGit, arguments, ensureSuccess).ConfigureAwait(false);
        return output.Output.TrimEnd(Environment.NewLine.ToCharArray());
    }

    private GitRemote CreateGitRemoteFromString(string remote)
    {
        var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var match = linkParser.Match(remote);
        if (match.Success)
        {
            if (Uri.TryCreate(match.Value, UriKind.Absolute, out var repositoryUri))
            {
                var remoteName = remote.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(remoteName))
                    return new GitRemote
                    {
                        Name = remoteName,
                        Url = repositoryUri
                    };
            }
            else
            {
                _logger.Normal($"Cannot parse {match.Value} to URI. SSH remote is currently not supported");
            }
        }

        return null;
    }
}
