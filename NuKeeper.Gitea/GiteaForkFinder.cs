using NuKeeper.Abstractions.CollaborationModels;
using NuKeeper.Abstractions.CollaborationPlatform;
using NuKeeper.Abstractions.Configuration;
using NuKeeper.Abstractions.Logging;
using NuKeeper.GitHub;

namespace NuKeeper.Gitea;

public class GiteaForkFinder : IForkFinder
{
    private readonly ICollaborationPlatform _collaborationPlatform;
    private readonly ForkMode _forkMode;
    private readonly INuKeeperLogger _logger;

    public GiteaForkFinder(ICollaborationPlatform collaborationPlatform, INuKeeperLogger logger, ForkMode forkMode)
    {
        _collaborationPlatform = collaborationPlatform;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _forkMode = forkMode;

        _logger.Detailed($"FindPushFork. Fork Mode is {_forkMode}");
    }

    public async Task<ForkData> FindPushFork(string userName, ForkData fallbackFork)
    {
        if (fallbackFork == null) throw new ArgumentNullException(nameof(fallbackFork));

        switch (_forkMode)
        {
            case ForkMode.PreferFork:
                return await FindUserForkOrUpstream(userName, fallbackFork).ConfigureAwait(false);

            case ForkMode.PreferSingleRepository:
                return await FindUpstreamRepoOrUserFork(userName, fallbackFork).ConfigureAwait(false);

            case ForkMode.SingleRepositoryOnly:
                return await FindUpstreamRepoOnly(fallbackFork).ConfigureAwait(false);

            default:
                throw new ArgumentOutOfRangeException($"Unknown fork mode: {_forkMode}");
        }
    }

    private async Task<ForkData> FindUserForkOrUpstream(string userName, ForkData pullFork)
    {
        var userFork = await TryFindUserFork(userName, pullFork).ConfigureAwait(false);
        if (userFork != null) return userFork;

        // as a fallback, we want to pull and push from the same origin repo.
        var canUseOriginRepo = await IsPushableRepo(pullFork).ConfigureAwait(false);
        if (canUseOriginRepo)
        {
            _logger.Normal(
                $"No fork for user {userName}. Using upstream fork for user {pullFork.Owner} at {pullFork.Uri}");
            return pullFork;
        }

        NoPushableForkFound(pullFork.Name);
        return null;
    }

    private async Task<ForkData> FindUpstreamRepoOrUserFork(string userName, ForkData pullFork)
    {
        // prefer to pull and push from the same origin repo.
        var canUseOriginRepo = await IsPushableRepo(pullFork).ConfigureAwait(false);
        if (canUseOriginRepo)
        {
            _logger.Normal($"Using upstream fork as push, for user {pullFork.Owner} at {pullFork.Uri}");
            return pullFork;
        }

        // fall back to trying a fork
        var userFork = await TryFindUserFork(userName, pullFork).ConfigureAwait(false);
        if (userFork != null) return userFork;

        NoPushableForkFound(pullFork.Name);
        return null;
    }

    private async Task<ForkData> FindUpstreamRepoOnly(ForkData pullFork)
    {
        // Only want to pull and push from the same origin repo.
        var canUseOriginRepo = await IsPushableRepo(pullFork).ConfigureAwait(false);
        if (canUseOriginRepo)
        {
            _logger.Normal($"Using upstream fork as push, for project {pullFork.Owner} at {pullFork.Uri}");
            return pullFork;
        }

        NoPushableForkFound(pullFork.Name);
        return null;
    }

    private void NoPushableForkFound(string name)
    {
        _logger.Error($"No pushable fork found for {name} in mode {_forkMode}");
    }

    private async Task<bool> IsPushableRepo(ForkData originFork)
    {
        var originRepo = await _collaborationPlatform.GetUserRepository(originFork.Owner, originFork.Name)
            .ConfigureAwait(false);
        return originRepo?.UserPermissions.Push == true;
    }

    private async Task<ForkData> TryFindUserFork(string userName, ForkData originFork)
    {
        var userFork = await _collaborationPlatform.GetUserRepository(userName, originFork.Name).ConfigureAwait(false);
        if (userFork != null)
        {
            var isMatchingFork = RepoIsForkOf(userFork, originFork.Uri);
            var forkIsPushable = userFork.UserPermissions.Push;
            if (isMatchingFork && forkIsPushable)
                // the user has a pushable fork
                return RepositoryToForkData(userFork);

            // the user has a repo of that name, but it can't be used. 
            // Don't try to create it
            _logger.Normal(
                $"User '{userName}' fork of '{originFork.Name}' exists but is unsuitable. Matching: {isMatchingFork}. Pushable: {forkIsPushable}");
            return null;
        }

        // no user fork exists, try and create it as a fork of the main repo
        var newFork = await _collaborationPlatform.MakeUserFork(originFork.Owner, originFork.Name)
            .ConfigureAwait(false);
        if (newFork != null) return RepositoryToForkData(newFork);

        return null;
    }

    private static bool RepoIsForkOf(Repository userRepo, Uri originRepo)
    {
        if (!userRepo.Fork) return false;

        if (userRepo.Parent?.CloneUrl == null) return false;

        if (originRepo == null) return false;

        var userParentUrl = GithubUriHelpers.Normalise(userRepo.Parent.CloneUrl);
        var originUrl = GithubUriHelpers.Normalise(originRepo);

        return userParentUrl.Equals(originUrl);
    }

    private static ForkData RepositoryToForkData(Repository repo)
    {
        return new ForkData(repo.CloneUrl, repo.Owner.Login, repo.Name);
    }
}
