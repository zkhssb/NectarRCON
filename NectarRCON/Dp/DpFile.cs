using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NectarRCON.Dp;

/// <summary>
/// 数据持久化文件
/// </summary>
public abstract class DpFile
{
    /// <summary>
    /// 文件名
    /// </summary>
    protected abstract string Name { get; }
    
    /// <summary>
    /// 文件路径
    /// </summary>
    protected virtual string BasePath => string.Empty;

    /// <summary>
    /// 实例映射
    /// </summary>
    private static readonly Dictionary<Type, DpFile> InstanceMapping = [];
    
    /// <summary>
    /// 保存数据
    /// </summary>
    public void Save()
    {
        var json = JsonSerializer.Serialize((object)this);
        var filePath = Path.Combine(AppContext.BaseDirectory,"dp", BasePath, Name);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="name">文件名</param>
    /// <param name="basePath">文件路径</param>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>实例</returns>
    private static T? Load<T>(string name, string? basePath = null)
        where T : DpFile
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "dp", basePath ?? string.Empty, name);
        if (!File.Exists(filePath)) return null;
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }
    
    /// <summary>
    /// 以单例模式加载数据
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>实例</returns>
    public static T LoadSingleton<T>()
        where T:DpFile
    {
        // 先从_instanceMapping拿数据
        if (InstanceMapping.TryGetValue(typeof(T), out var cachedInstance))
        {
            return (T)cachedInstance;
        }
        
        // 如果缓存没有 则使用找到此DPFile的无参构造函数 使用反射实例化后存放到_instanceMapping
        var instance = Activator.CreateInstance<T>();
        // 从instance中获取Name 随后load
        InstanceMapping[typeof(T)] = Load<T>(instance.Name, instance.BasePath) ?? instance;
        return (T)InstanceMapping[typeof(T)];
    }
}