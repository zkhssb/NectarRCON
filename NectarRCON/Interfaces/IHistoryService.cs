namespace NectarRCON.Interfaces;

public class HistoryNode
{ 
    public string? Cmd { get; set; }

    public HistoryNode? Prev { get; set; }

    public HistoryNode? Next { get; set; }
}

public interface IHistoryService
{
    /// <summary>
    /// 获取前一条命令
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    HistoryNode? Prev(HistoryNode? current);

    /// <summary>
    /// 获取后一条命令
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    HistoryNode? Next(HistoryNode? current);

    /// <summary>
    /// 输入命令
    /// </summary>
    /// <param name="cmd"></param>
    HistoryNode? InputCmd(string cmd);
}
