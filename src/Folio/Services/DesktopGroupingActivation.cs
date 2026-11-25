namespace Folio.Core.Persistence;

internal sealed record DesktopGroupingRequest(bool SplitByType, string[] Paths);

internal sealed class DesktopGroupingActivation(string directory)
{
    internal const string OneWidget = "--group-desktop-one";
    internal const string SplitByType = "--group-desktop-by-type";

    internal static DesktopGroupingRequest? Parse(string[] args)
    {
        if (args.Length < 2 || args[0] is not (OneWidget or SplitByType))
        {
            return null;
        }

        string[] paths = args.Skip(1)
            .Where(path => Path.IsPathFullyQualified(path) && path.IndexOfAny(['\r', '\n', '\0']) < 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return paths.Length == 0 ? null : new(args[0] == SplitByType, paths);
    }

    internal void Enqueue(DesktopGroupingRequest request)
    {
        Directory.CreateDirectory(directory);
        string filename = Path.Combine(directory, $"{DateTime.UtcNow.Ticks:D19}-{Guid.NewGuid():N}");
        string temporary = filename + ".tmp";
        try
        {
            File.WriteAllLines(temporary,
                [request.SplitByType ? SplitByType : OneWidget, .. request.Paths]);
            File.Move(temporary, filename + ".request");
        }
        finally
        {
            File.Delete(temporary);
        }
    }

    internal IReadOnlyList<DesktopGroupingRequest> Drain()
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        var requests = new List<DesktopGroupingRequest>();
        foreach (string file in Directory.GetFiles(directory, "*.request").Order(StringComparer.Ordinal))
        {
            var request = Parse(File.ReadAllLines(file));
            File.Delete(file);
            if (request is not null)
            {
                requests.Add(request);
            }
        }
        return requests;
    }
}
