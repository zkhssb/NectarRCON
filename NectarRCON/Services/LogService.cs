using NectarRCON.Interfaces;
using NectarRCON.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NectarRCON.Services;
public class LogService : ILogService
{
    private FileStream? _logFileStream;
    public LogService()
    {
        if (!Path.Exists("./logs"))
            Directory.CreateDirectory("./logs");
    }
    public void Clear()
    {
        _logFileStream?.SetLength(0);
        _logFileStream?.Flush();
    }
    public string GetText()
    {
        using(MemoryStream ms = new MemoryStream())
        {
            _logFileStream?.CopyTo(ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
    public string Log(string message)
    {
        string logText = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}\n";
        _logFileStream?.Write(Encoding.UTF8.GetBytes(logText));
        _logFileStream?.Flush();
        return logText;
    }
    public void SetServer(ServerInformation server)
    {
        string serverNameEncode = UrlEncoder.Default.Encode(server.Name);
        if (!File.Exists($"./logs/{serverNameEncode}.log"))
            File.Create($"./logs/{serverNameEncode}.log").Close();
        _logFileStream?.Close();
        _logFileStream?.Dispose();
        _logFileStream = File.Open($"./logs/{serverNameEncode}.log", FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
    }
}