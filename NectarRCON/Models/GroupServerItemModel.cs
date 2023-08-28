using CommunityToolkit.Mvvm.ComponentModel;

namespace NectarRCON.Models;
public partial class GroupServerItemModel : ObservableObject
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private GroupModel _baseModel;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public GroupServerItemModel(string name, GroupModel baseModel)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {
        Name = name;
        BaseModel = baseModel;
    }
}
