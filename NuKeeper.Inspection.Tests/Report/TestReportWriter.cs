using System.Text;
using NuKeeper.Inspection.Report;

namespace NuKeeper.Inspection.Tests.Report;

public sealed class TestReportWriter : IReportWriter
{
    private readonly StringBuilder _data = new();

    public void WriteLine(string value = "")
    {
        _data.AppendLine(value);
    }

    public void Dispose()
    {
    }

    public string Data()
    {
        return _data.ToString().TrimEnd();
    }
}
