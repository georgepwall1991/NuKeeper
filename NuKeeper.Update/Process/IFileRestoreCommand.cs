using NuKeeper.Abstractions.NuGet;

namespace NuKeeper.Update.Process;

public interface IFileRestoreCommand : IPackageCommand
{
    Task Invoke(FileInfo file, NuGetSources sources);
}
