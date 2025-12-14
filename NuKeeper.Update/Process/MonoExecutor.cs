using NuGet.Common;
using NuKeeper.Abstractions.Logging;
using NuKeeper.Update.ProcessRunner;

namespace NuKeeper.Update.Process;

public class MonoExecutor : IMonoExecutor
{
    private readonly AsyncLazy<bool> _checkMono;
    private readonly IExternalProcess _externalProcess;
    private readonly INuKeeperLogger _logger;

    public MonoExecutor(INuKeeperLogger logger, IExternalProcess externalProcess)
    {
        _logger = logger;
        _externalProcess = externalProcess;
        _checkMono = new AsyncLazy<bool>(CheckMonoExists);
    }

    public async Task<bool> CanRun()
    {
        return await _checkMono;
    }

    public async Task<ProcessOutput> Run(string workingDirectory, string command, string arguments, bool ensureSuccess)
    {
        _logger.Normal($"Using Mono to run '{command}'");

        if (!await CanRun())
        {
            _logger.Error($"Cannot run '{command}' on Mono since Mono installation was not found");
            throw new InvalidOperationException("Mono installation was not found");
        }

        return await _externalProcess.Run(workingDirectory, "mono", $"{command} {arguments}", ensureSuccess)
            .ConfigureAwait(false);
    }

    private async Task<bool> CheckMonoExists()
    {
        var result = await _externalProcess.Run("", "mono", "--version", false).ConfigureAwait(false);

        return result.Success;
    }
}
