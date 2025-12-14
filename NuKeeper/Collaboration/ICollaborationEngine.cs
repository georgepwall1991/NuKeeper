using NuKeeper.Abstractions.Configuration;

namespace NuKeeper.Collaboration;

public interface ICollaborationEngine
{
    Task<int> Run(SettingsContainer settings);
}
