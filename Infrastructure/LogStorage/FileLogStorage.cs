using System.IO;

namespace projektRPG.Infrastructure.LogStorage;

/// <summary>
/// File-based implementation of <see cref="ILogStorage"/>.
/// Creates a timestamped log file per session (server or client).
/// </summary>
public class FileLogStorage : ILogStorage, IDisposable
{
    private readonly string fullPath;
    private readonly StreamWriter writer;

    /// <summary>
    /// Opens (or creates) a log file named after the player/session and writes a header line.
    /// </summary>
    /// <param name="directory">Target folder (created automatically if missing).</param>
    /// <param name="playerName">Used in the file name and header (e.g. "SERVER" or the client's nick).</param>
    public FileLogStorage(string directory, string playerName)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"{playerName}_{timestamp}.txt";

        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        fullPath = Path.Combine(directory, fileName);

        writer = new StreamWriter(fullPath, append: true) { AutoFlush = true };

        if (new FileInfo(fullPath).Length == 0)
        {
            writer.WriteLine($"--- EVENT LOG: {playerName} ---");
        }
    }

    /// <summary>Appends a single log line to the open file.</summary>
    public void Write(string message)
    {
        writer.WriteLine(message);
    }

    public void Dispose()
    {
        writer?.Dispose();
    }
}
