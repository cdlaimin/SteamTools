#nullable enable
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable SA1634 // File header should show copyright
#pragma warning disable CS8601 // 引用类型赋值可能为 null。
#pragma warning disable CS0108 // 成员隐藏继承的成员；缺少关键字 new
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由包 BD.Common.Settings.V4.SourceGenerator.Tools 源生成。
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using static BD.WTTS.Settings.Abstractions.IASFSettings;

// ReSharper disable once CheckNamespace
namespace BD.WTTS.Settings;

[JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyProperties = true)]
[JsonSerializable(typeof(ASFSettings_))]
internal partial class ASFSettingsContext : JsonSerializerContext
{
    static ASFSettingsContext? instance;

    public static ASFSettingsContext Instance
        => instance ??= new ASFSettingsContext(ISettings.GetDefaultOptions());
}

[MPObj, MP2Obj(SerializeLayout.Explicit)]
public sealed partial class ASFSettings_ : IASFSettings, ISettings, ISettings<ASFSettings_>
{
    public const string Name = nameof(ASFSettings);

    static string ISettings.Name => Name;

    static JsonSerializerContext ISettings.JsonSerializerContext
        => ASFSettingsContext.Instance;

    static JsonTypeInfo ISettings.JsonTypeInfo
        => ASFSettingsContext.Instance.ASFSettings_;

    static JsonTypeInfo<ASFSettings_> ISettings<ASFSettings_>.JsonTypeInfo
        => ASFSettingsContext.Instance.ASFSettings_;

    /// <summary>
    /// ASF 路径
    /// </summary>
    [MPKey(0), MP2Key(0), JsonPropertyOrder(0)]
    public string? ArchiSteamFarmExePath { get; set; } = IASFSettings.DefaultArchiSteamFarmExePath;

    /// <summary>
    /// 程序启动时自动运行 ASF
    /// </summary>
    [MPKey(1), MP2Key(1), JsonPropertyOrder(1)]
    public bool AutoRunArchiSteamFarm { get; set; } = IASFSettings.DefaultAutoRunArchiSteamFarm;

    /// <summary>
    /// 检查文件安全性
    /// </summary>
    [MPKey(2), MP2Key(2), JsonPropertyOrder(2)]
    public bool CheckArchiSteamFarmExe { get; set; } = IASFSettings.DefaultCheckArchiSteamFarmExe;

    /// <summary>
    /// 控制台默认最大行数
    /// </summary>
    [MPKey(3), MP2Key(3), JsonPropertyOrder(3)]
    public int ConsoleMaxLineDefault { get; set; } = IASFSettings.DefaultConsoleMaxLineDefault;

    /// <summary>
    /// 控制台默认最大行数范围最小值
    /// </summary>
    [MPKey(4), MP2Key(4), JsonPropertyOrder(4)]
    public int MinRangeConsoleMaxLine { get; set; } = IASFSettings.DefaultMinRangeConsoleMaxLine;

    /// <summary>
    /// 控制台默认最大行数范围最大值
    /// </summary>
    [MPKey(5), MP2Key(5), JsonPropertyOrder(5)]
    public int MaxRangeConsoleMaxLine { get; set; } = IASFSettings.DefaultMaxRangeConsoleMaxLine;

    /// <summary>
    /// 控制台最大行数
    /// </summary>
    [MPKey(6), MP2Key(6), JsonPropertyOrder(6)]
    public int ConsoleMaxLine { get; set; } = IASFSettings.DefaultConsoleMaxLine;

    /// <summary>
    /// 
    /// </summary>
    [MPKey(7), MP2Key(7), JsonPropertyOrder(7)]
    public int ConsoleFontSizeDefault { get; set; } = IASFSettings.DefaultConsoleFontSizeDefault;

    /// <summary>
    /// 
    /// </summary>
    [MPKey(8), MP2Key(8), JsonPropertyOrder(8)]
    public int MinRangeConsoleFontSize { get; set; } = IASFSettings.DefaultMinRangeConsoleFontSize;

    /// <summary>
    /// 
    /// </summary>
    [MPKey(9), MP2Key(9), JsonPropertyOrder(9)]
    public int MaxRangeConsoleFontSize { get; set; } = IASFSettings.DefaultMaxRangeConsoleFontSize;

    /// <summary>
    /// 控制台字体大小
    /// </summary>
    [MPKey(10), MP2Key(10), JsonPropertyOrder(10)]
    public int ConsoleFontSize { get; set; } = IASFSettings.DefaultConsoleFontSize;

    /// <summary>
    /// 
    /// </summary>
    [MPKey(11), MP2Key(11), JsonPropertyOrder(11)]
    public int IPCPortIdValue { get; set; } = IASFSettings.DefaultIPCPortIdValue;

    /// <summary>
    /// IPC 端口号，默认值为 <see cref="DefaultIPCPortIdValue" />
    /// </summary>
    [MPKey(12), MP2Key(12), JsonPropertyOrder(12)]
    public int IPCPortId { get; set; } = IASFSettings.DefaultIPCPortId;

    /// <summary>
    /// IPC 端口号被占用时是否随机一个未使用的端口号，默认值 <see langword="true" />
    /// </summary>
    [MPKey(13), MP2Key(13), JsonPropertyOrder(13)]
    public bool IPCPortOccupiedRandom { get; set; } = IASFSettings.DefaultIPCPortOccupiedRandom;

}

public static partial class ASFSettings
{
    /// <summary>
    /// ASF 路径
    /// </summary>
    public static SettingsProperty<string, ASFSettings_> ArchiSteamFarmExePath { get; }
        = new(DefaultArchiSteamFarmExePath);

    /// <summary>
    /// 程序启动时自动运行 ASF
    /// </summary>
    public static SettingsStructProperty<bool, ASFSettings_> AutoRunArchiSteamFarm { get; }
        = new(DefaultAutoRunArchiSteamFarm);

    /// <summary>
    /// 检查文件安全性
    /// </summary>
    public static SettingsStructProperty<bool, ASFSettings_> CheckArchiSteamFarmExe { get; }
        = new(DefaultCheckArchiSteamFarmExe);

    /// <summary>
    /// 控制台默认最大行数
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> ConsoleMaxLineDefault { get; }
        = new(DefaultConsoleMaxLineDefault);

    /// <summary>
    /// 控制台默认最大行数范围最小值
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> MinRangeConsoleMaxLine { get; }
        = new(DefaultMinRangeConsoleMaxLine);

    /// <summary>
    /// 控制台默认最大行数范围最大值
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> MaxRangeConsoleMaxLine { get; }
        = new(DefaultMaxRangeConsoleMaxLine);

    /// <summary>
    /// 控制台最大行数
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> ConsoleMaxLine { get; }
        = new(DefaultConsoleMaxLine);

    /// <summary>
    /// 
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> ConsoleFontSizeDefault { get; }
        = new(DefaultConsoleFontSizeDefault);

    /// <summary>
    /// 
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> MinRangeConsoleFontSize { get; }
        = new(DefaultMinRangeConsoleFontSize);

    /// <summary>
    /// 
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> MaxRangeConsoleFontSize { get; }
        = new(DefaultMaxRangeConsoleFontSize);

    /// <summary>
    /// 控制台字体大小
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> ConsoleFontSize { get; }
        = new(DefaultConsoleFontSize);

    /// <summary>
    /// 
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> IPCPortIdValue { get; }
        = new(DefaultIPCPortIdValue);

    /// <summary>
    /// IPC 端口号，默认值为 <see cref="DefaultIPCPortIdValue" />
    /// </summary>
    public static SettingsStructProperty<int, ASFSettings_> IPCPortId { get; }
        = new(DefaultIPCPortId);

    /// <summary>
    /// IPC 端口号被占用时是否随机一个未使用的端口号，默认值 <see langword="true" />
    /// </summary>
    public static SettingsStructProperty<bool, ASFSettings_> IPCPortOccupiedRandom { get; }
        = new(DefaultIPCPortOccupiedRandom);

}
