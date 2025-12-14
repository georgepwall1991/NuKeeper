<p align="center"><img src="./assets/NuKeeperTopBar.jpg"></p>

[![Build Status](https://dev.azure.com/nukeeper/NuKeeper/_apis/build/status/NuKeeper%20PR%20Build?branchName=master)](https://dev.azure.com/nukeeper/NuKeeper/_build/latest?definitionId=4&branchName=master)
[![Gitter](https://img.shields.io/gitter/room/NuKeeperDotNet/Lobby.js.svg?maxAge=2592000)](https://gitter.im/NuKeeperDotNet/Lobby)
[![NuGet](https://img.shields.io/nuget/v/NuKeeper.svg?maxAge=3600)](https://www.nuget.org/packages/NuKeeper/)
[![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/nukeeper/NuKeeper/4.svg)](https://dev.azure.com/nukeeper/NuKeeper/_build?definitionId=4)

## NuKeeper

Automagically update NuGet packages in all .NET projects.

> **Note:** This project was originally archived.
> See [Why is NuKeeper Archived](https://github.com/NuKeeperDotNet/NuKeeper/issues/1155) for context.

### What's New in v0.36

- **Modernized to .NET 10** - Now targets the latest .NET runtime
- **Improved async patterns** - Added `ConfigureAwait(false)` throughout for better performance
- **Enhanced error handling** - Consistent error reporting across all platform providers
- **Updated dependencies** - All NuGet packages updated to latest versions

---

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Git installed and available in PATH
- Platform-specific access token (GitHub, Azure DevOps, GitLab, Bitbucket, or Gitea)

> **Note:** For updating legacy .NET Framework projects on Linux/macOS, [Mono](https://www.mono-project.com/) is
> required.

---

## Installation

### As a Global Tool (Recommended)

```bash
dotnet tool install nukeeper --global
```

### Verify Installation

```bash
nukeeper --version
```

### Update to Latest Version

```bash
dotnet tool update nukeeper --global
```

### Uninstall

```bash
dotnet tool uninstall nukeeper --global
```

---

## Quick Start

### 1. Inspect Local Projects for Updates

Check what packages can be updated without making changes:

```bash
nukeeper inspect /path/to/your/solution
```

### 2. Update Local Projects

Apply package updates to your local project:

```bash
nukeeper update /path/to/your/solution
```

### 3. Create Pull Requests for a Repository

Automatically create PRs for a GitHub repository:

```bash
nukeeper repo https://github.com/owner/repo --token YOUR_GITHUB_TOKEN
```

---

## Commands

### `inspect` - Check for Available Updates

Scans local projects and reports available NuGet package updates without making changes.

```bash
# Inspect current directory
nukeeper inspect

# Inspect specific path
nukeeper inspect /path/to/solution

# Output as CSV
nukeeper inspect --output csv

# Only show major version updates
nukeeper inspect --change Major
```

### `update` - Apply Updates Locally

Applies NuGet package updates to local projects.

```bash
# Update all packages in current directory
nukeeper update

# Update specific path
nukeeper update /path/to/solution

# Limit number of updates
nukeeper update --maxpackageupdates 5

# Only apply patch updates (safest)
nukeeper update --change Patch

# Update specific package only
nukeeper update --include PackageName
```

### `repo` - Create PRs for a Single Repository

Creates pull requests with package updates for a single repository.

```bash
# GitHub
nukeeper repo https://github.com/owner/repo --token ghp_xxxxx

# Azure DevOps
nukeeper repo https://dev.azure.com/org/project/_git/repo --token pat_xxxxx

# GitLab
nukeeper repo https://gitlab.com/owner/repo --token glpat_xxxxx

# Bitbucket
nukeeper repo https://bitbucket.org/owner/repo --token xxxxx

# Gitea
nukeeper repo https://gitea.example.com/owner/repo --token xxxxx
```

**Common Options:**

```bash
# Limit PRs created
nukeeper repo URL --token TOKEN --maxpackageupdates 3

# Target specific branch
nukeeper repo URL --token TOKEN --targetbranch develop

# Only update specific packages
nukeeper repo URL --token TOKEN --include Newtonsoft.Json

# Exclude packages from updates
nukeeper repo URL --token TOKEN --exclude Microsoft.Extensions.*

# Add labels to PRs
nukeeper repo URL --token TOKEN --label dependencies --label automated

# Set PR reviewers (Azure DevOps)
nukeeper repo URL --token TOKEN --reviewer user@example.com
```

### `org` - Create PRs for an Organization

Creates pull requests for all repositories in a GitHub organization.

```bash
nukeeper org MyOrganization --token ghp_xxxxx

# Limit to specific repos
nukeeper org MyOrganization --token TOKEN --include repo1,repo2

# Exclude repos
nukeeper org MyOrganization --token TOKEN --exclude legacy-repo
```

### `global` - Create PRs for All Accessible Repositories

Creates pull requests for all repositories the token has access to.

```bash
nukeeper global --token ghp_xxxxx
```

---

## Configuration Options

### Change Level

Control which types of updates to apply:

| Level   | Description           | Example       |
|---------|-----------------------|---------------|
| `Major` | Major version updates | 1.0.0 → 2.0.0 |
| `Minor` | Minor version updates | 1.0.0 → 1.1.0 |
| `Patch` | Patch version updates | 1.0.0 → 1.0.1 |
| `None`  | No updates            | -             |

```bash
nukeeper update --change Patch
```

### Age Threshold

Only update packages that have been published for a certain period (reduces risk from newly published packages):

```bash
# Only update packages published at least 14 days ago
nukeeper update --age 14
```

### Package Filters

```bash
# Include only specific packages
nukeeper update --include Newtonsoft.Json,Serilog

# Include packages matching pattern
nukeeper update --include Microsoft.*

# Exclude specific packages
nukeeper update --exclude EntityFramework

# Exclude packages matching pattern
nukeeper update --exclude *.Preview
```

### Consolidation

Update the same package across all projects in a solution to the same version:

```bash
nukeeper update --consolidate
```

### Verbosity

```bash
nukeeper update --verbosity Detailed
nukeeper update --verbosity Quiet
```

---

## Platform Support

### Project Types

| Project Type        | Supported |
|:--------------------|:---------:|
| .NET 8/9/10         |    Yes    |
| .NET Core 3.1+      |    Yes    |
| .NET Standard       |    Yes    |
| .NET Framework      |    Yes    |
| Private NuGet Feeds |    Yes    |

### Git Platforms

| Platform         | Supported | Token Type            |
|:-----------------|:---------:|:----------------------|
| GitHub           |    Yes    | Personal Access Token |
| Azure DevOps     |    Yes    | Personal Access Token |
| GitLab           |    Yes    | Personal Access Token |
| Bitbucket Cloud  |    Yes    | App Password          |
| Bitbucket Server |    Yes    | Personal Access Token |
| Gitea            |    Yes    | Access Token          |

---

## Docker

Run NuKeeper in a container:

```bash
# Pull the image
docker pull nukeeper/nukeeper:latest

# Run inspection
docker run --rm -v /path/to/solution:/repo nukeeper/nukeeper inspect /repo

# Create PRs
docker run --rm nukeeper/nukeeper repo https://github.com/owner/repo --token TOKEN
```

### Build Docker Image Locally

```bash
docker build -t nukeeper -f Docker/SDK10.0/Dockerfile .
```

---

## Building from Source

### Prerequisites

- .NET 10 SDK

### Build

```bash
# Clone repository
git clone https://github.com/NuKeeperDotNet/NuKeeper.git
cd NuKeeper

# Build
dotnet build

# Run tests
dotnet test

# Create NuGet package
dotnet pack NuKeeper/NuKeeper.csproj -o ./artifacts

# Install local build
dotnet tool install nukeeper --global --add-source ./artifacts
```

---

## Examples

### CI/CD Integration

#### GitHub Actions

```yaml
- name: Update NuGet Packages
  run: |
    dotnet tool install nukeeper --global
    nukeeper repo ${{ github.repository }} --token ${{ secrets.GITHUB_TOKEN }}
```

#### Azure Pipelines

```yaml
- script: |
    dotnet tool install nukeeper --global
    nukeeper repo $(System.TeamFoundationCollectionUri)$(System.TeamProject)/_git/$(Build.Repository.Name) --token $(System.AccessToken)
  displayName: 'Update NuGet Packages'
```

### Private NuGet Feeds

```bash
# Use custom NuGet source
nukeeper update --source https://nuget.example.com/v3/index.json

# Multiple sources
nukeeper update --source https://api.nuget.org/v3/index.json --source https://nuget.example.com/v3/index.json
```

### Scheduled Updates

Create a scheduled job to check for updates weekly:

```bash
# Cron job example (every Monday at 9am)
0 9 * * 1 /usr/local/bin/nukeeper repo https://github.com/owner/repo --token TOKEN --maxpackageupdates 5
```

---

## Troubleshooting

### Common Issues

**"Unable to find package"**

- Ensure you have access to the NuGet feed
- Check if the package requires authentication

**"401 Unauthorized"**

- Verify your token has the correct permissions
- For GitHub: needs `repo` scope
- For Azure DevOps: needs `Code (Read & Write)` scope

**"No updates found"**

- Try with `--verbosity Detailed` to see what's happening
- Check your `--change` level setting
- Verify packages aren't excluded by age threshold

### Debug Mode

```bash
nukeeper update --verbosity Detailed --logfile nukeeper.log
```

---

## Licensing

NuKeeper is licensed under the [Apache License](http://opensource.org/licenses/apache.html)

### Dependencies

* [LibGit2Sharp](https://github.com/libgit2/libgit2sharp/) - MIT
* [Octokit](https://github.com/octokit/octokit.net) - MIT
* [NuGet.Protocol](https://github.com/NuGet/NuGet.Client) - Apache 2.0
* [McMaster.Extensions.CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils) - Apache 2.0

---

## Acknowledgements

Logos by [area55](https://github.com/area55git), licensed
under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/).

<p align="center">
  <img src="https://github.com/NuKeeperDotNet/NuKeeper/blob/master/assets/Footer.svg" />
</p>
