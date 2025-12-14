namespace NuKeeper.Inspection.RepositoryInspection;

public class DirectoryExclusions : IDirectoryExclusions
{
    private static readonly List<string> ExcludedDirNames = new()
    {
        ".git",
        ".vs",
        "obj",
        "bin",
        "node_modules",
        "packages"
    };

    public bool PathIsExcluded(string path)
    {
        return ExcludedDirNames.Any(s => PathContains(path, s));
    }

    private static bool PathContains(string fullPath, string dirName)
    {
        var dirInPath = Path.DirectorySeparatorChar + dirName + Path.DirectorySeparatorChar;

        return
            !string.IsNullOrEmpty(fullPath) &&
            fullPath.IndexOf(dirInPath, StringComparison.InvariantCultureIgnoreCase) >= 0;
    }
}
