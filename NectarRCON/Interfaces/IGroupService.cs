using NectarRCON.Models;
using System.Collections.Generic;

namespace NectarRCON.Interfaces;

public interface IGroupService
{
    /// <summary>
    /// 获取所有组
    /// </summary>
    IReadOnlyList<Group> GetGroups();

    /// <summary>
    /// 通过组名字获取组
    /// </summary>
    /// <param name="name">组名</param>
    Group? FindGroup(string name);

    /// <summary>
    /// 通过组Id获取组
    /// </summary>
    /// <param name="groupId">组Id</param>
    Group? GetGroup(string groupId);

    /// <summary>
    /// 删除组
    /// </summary>
    /// <param name="groupId">组Id</param>
    void Delete(string groupId);

    /// <summary>
    /// 添加一个组
    /// </summary>
    string Add(Group group);
}
