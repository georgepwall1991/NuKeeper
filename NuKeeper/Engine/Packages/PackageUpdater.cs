using NuKeeper.Abstractions.CollaborationModels;
using NuKeeper.Abstractions.CollaborationPlatform;
using NuKeeper.Abstractions.Configuration;
using NuKeeper.Abstractions.Git;
using NuKeeper.Abstractions.Logging;
using NuKeeper.Abstractions.NuGet;
using NuKeeper.Abstractions.RepositoryInspection;
using NuKeeper.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuKeeper.Engine.Packages
{
    public class PackageUpdater : IPackageUpdater
    {
        private readonly ICollaborationFactory _collaborationFactory;
        private readonly IExistingCommitFilter _existingCommitFilter;
        private readonly INuKeeperLogger _logger;
        private readonly IUpdateRunner _updateRunner;

        public PackageUpdater(
            ICollaborationFactory collaborationFactory,
            IExistingCommitFilter existingCommitFilter,
            IUpdateRunner localUpdater,
            INuKeeperLogger logger)
        {
            _collaborationFactory = collaborationFactory;
            _existingCommitFilter = existingCommitFilter;
            _updateRunner = localUpdater;
            _logger = logger;
        }

        public async Task<(int UpdatesMade, bool ThresholdReached)> MakeUpdatePullRequests(
            IGitDriver git,
            RepositoryData repository,
            IReadOnlyCollection<PackageUpdateSet> updates,
            NuGetSources sources,
            SettingsContainer settings
        )
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (git == null)
            {
                throw new ArgumentNullException(nameof(git));
            }

            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var openPrs = await _collaborationFactory.CollaborationPlatform.GetNumberOfOpenPullRequests(
                repository.Pull.Owner,
                repository.Pull.Name
            );

            var allowedPrs = settings.UserSettings.MaxOpenPullRequests;

            if (openPrs >= allowedPrs)
            {
                _logger.Normal("Number of open pull requests equals or exceeds allowed number of open pull requests.");
                return (0, true);
            }

            int totalCount = 0;

            var groups = UpdateConsolidator.Consolidate(updates,
                settings.UserSettings.ConsolidateUpdatesInSinglePullRequest);

            foreach (var updateSets in groups)
            {
                var (updatesMade, pullRequestCreated) = await MakeUpdatePullRequests(
                    git, repository,
                    sources, settings, updateSets);

                totalCount += updatesMade;

                if (pullRequestCreated)
                    openPrs++;

                if (openPrs == allowedPrs)
                {
                    return (totalCount, true);
                }
            }

            return (totalCount, false);
        }

        private async Task<(int UpdatesMade, bool PullRequestCreated)> MakeUpdatePullRequests(
            IGitDriver git, RepositoryData repository,
            NuGetSources sources, SettingsContainer settings,
            IReadOnlyCollection<PackageUpdateSet> updates)
        {
            _logger.Normal(UpdatesLogger.OldVersionsToBeUpdated(updates));

            await git.Checkout(repository.DefaultBranch).ConfigureAwait(false);

            // branch
            var branchWithChanges = BranchNamer.MakeName(updates, settings.BranchSettings.BranchNameTemplate);
            _logger.Detailed($"Using branch name: '{branchWithChanges}'");

            var ditCheckOut = await git.CheckoutNewBranch(branchWithChanges).ConfigureAwait(false);
            if (!ditCheckOut)
            {
                await git.CheckoutRemoteToLocal(branchWithChanges).ConfigureAwait(false);
            }

            var filteredUpdates = await _existingCommitFilter.Filter(git, updates, repository.DefaultBranch, branchWithChanges).ConfigureAwait(false);

            foreach (var filtered in updates.Where(u => !filteredUpdates.Contains(u)))
            {
                var commitMessage = _collaborationFactory.CommitWorder.MakeCommitMessage(filtered);
                _logger.Normal($"Commit '{commitMessage}' already in branch '{branchWithChanges}'");
            }

            var haveUpdates = filteredUpdates.Any();
            foreach (var updateSet in filteredUpdates)
            {
                var commitMessage = _collaborationFactory.CommitWorder.MakeCommitMessage(updateSet);

                await _updateRunner.Update(updateSet, sources).ConfigureAwait(false);

                await git.Commit(commitMessage).ConfigureAwait(false);
            }

            bool pullRequestCreated = false;

            // bug: pr might not have been created yet
            if (haveUpdates)
            {
                await git.Push(repository.Remote, branchWithChanges).ConfigureAwait(false);

                string qualifiedBranch;
                if (!repository.IsFork) //check if we are on a fork, if so qualify the branch name
                {
                    qualifiedBranch = branchWithChanges;
                }
                else
                {
                    qualifiedBranch = repository.Push.Owner + ":" + branchWithChanges;
                }

                bool pullRequestExists = await _collaborationFactory.CollaborationPlatform.PullRequestExists(repository.Pull, qualifiedBranch, repository.DefaultBranch).ConfigureAwait(false);

                if (!pullRequestExists)
                {
                    var title = _collaborationFactory.CommitWorder.MakePullRequestTitle(updates);
                    var body = _collaborationFactory.CommitWorder.MakeCommitDetails(updates);

                    var pullRequestRequest = new PullRequestRequest(qualifiedBranch, title, repository.DefaultBranch, settings.BranchSettings.DeleteBranchAfterMerge, settings.SourceControlServerSettings.Repository.SetAutoMerge) { Body = body };

                    await _collaborationFactory.CollaborationPlatform.OpenPullRequest(repository.Pull, pullRequestRequest, settings.SourceControlServerSettings.Labels).ConfigureAwait(false);

                    pullRequestCreated = true;
                }
                else
                {
                    _logger.Normal($"A pull request already exists for {repository.DefaultBranch} <= {qualifiedBranch}");
                }
            }
            await git.Checkout(repository.DefaultBranch).ConfigureAwait(false);
            return (filteredUpdates.Count, pullRequestCreated);
        }
    }
}
