using NectarRCON.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NectarRCON.Services;

public class HistoryService : IHistoryService
{

    /// <summary>
    /// 双向链表保存历史记录
    /// </summary>
    private HistoryNode? _head;
    private HistoryNode? _tail;

    private readonly Dictionary<string, HistoryNode> _map;
    
    /// <summary>
    /// 限制记录条数
    /// </summary>
    private int _limit = 20;

    public HistoryService()
    {
        _map = [];
        if (!Path.Exists("./logs"))
        { 
            Directory.CreateDirectory("./logs");
        }

        var stream = File.Open($"./logs/history", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        using StreamReader reader = new StreamReader(stream);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (!string.IsNullOrEmpty(line))
            {
                AppendNode(line);
            }
        }
    }

    public HistoryNode? InputCmd(string cmd)
    {
        var node = AppendNode(cmd);
        Save();
        return node;
    }

    private HistoryNode? AppendNode(string cmd)
    {
        cmd = cmd.Trim();
        HistoryNode node;
        if (_map.ContainsKey(cmd))
        {
            // 已存在的记录，将其移动到链表尾部
            node = _map[cmd];
            if (node == _tail)
            {
                return null;
            }
            else if (node == _head)
            {
                _head = _head.Next;
            }

            if (node.Prev != null)
            {
                node.Prev.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Prev = node.Prev;
            }
            node.Prev = null;
            node.Next = null;
        }
        else
        {
            node = new HistoryNode()
            {
                Cmd = cmd,
            };
            _map.Add(cmd, node);
        }

        if (_head == null || _tail == null)
        {
            _head = _tail = node;
        }
        else
        {
            _tail.Next = node;
            node.Prev = _tail;
            _tail = node;
        }

        // 超出限制，移除链表头部记录
        if (_map.Count() > _limit)
        {
            var head = _head;
            _head = _head.Next;
            if (_head != null)
            { 
                _head.Prev = null;
            }
            if (!string.IsNullOrEmpty(head.Cmd))
            {
                _map.Remove(head.Cmd);
            }
        }

        return null;
    }

    public HistoryNode? Next(HistoryNode? current)
    {
        if (current == null)
        {
            return null;
        }
        return current.Next;
    }

    public HistoryNode? Prev(HistoryNode? current)
    {
        if (current == null)
        {
            return _tail;
        }
        else if (current.Prev == null)
        {
            return current;
        }
        return current.Prev;
    }

    private void Save()
    {
        using var stream = File.Open($"./logs/history", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        stream.Seek(0, SeekOrigin.Begin);
        HistoryNode? node = _head;
        while (node != null) {
            if (!string.IsNullOrEmpty(node.Cmd))
            {
                string cmd = node.Cmd + "\n";
                stream.Write(Encoding.UTF8.GetBytes(cmd));
            }
            node = node.Next;
        }
    }
}
