using Avalonia.Automation;
using BD.SteamClient.Models;
using BD.SteamClient.Models.Idle;
using BD.SteamClient.Services;
using System.Linq;

namespace BD.WTTS.UI.ViewModels;

public sealed class IdleAppsPageViewModel : ViewModelBase
{
    readonly ISteamService SteamTool = ISteamService.Instance;
    readonly ISteamIdleCardService IdleCard = ISteamIdleCardService.Instance;

    public IdleAppsPageViewModel()
    {
        this.WhenPropertyChanged(x => x.IsAutoNextOn)
            .Subscribe(x =>
            {
                RunOrStopAutoNext(x.Value);
                this.IsAutoNextOnTxt = x.Value ? Strings.Idle_StopAutoNext : Strings.Idle_OpenAutoNext;
            });
    }

    [Reactive]
    public bool RunLoaingState { get; set; }

    [Reactive]
    public bool RunState { get; set; }

    [Reactive]
    public string? RuningCountTxt { get; set; }

    [Reactive]
    public ObservableCollection<SteamApp> IdleGameList { get; set; } = new();

    /// <summary>
    /// 当前挂卡游戏 
    /// </summary>
    [Reactive]
    public SteamApp? CurrentIdle { get; set; }

    /// <summary>
    /// 挂卡规则
    /// </summary>
    [Reactive]
    public IdleRule IdleRule { get; set; }

    /// <summary>
    /// 挂卡顺序
    /// </summary>
    [Reactive]
    public IdleSequentital IdleSequentital { get; set; }

    /// <summary>
    /// 自动运行下一个游戏
    /// </summary>
    [Reactive]
    public bool IsAutoNextOn { get; set; }

    /// <summary>
    /// 自动运行下一个游戏文本展示
    /// </summary>
    [Reactive]
    public string? IsAutoNextOnTxt { get; set; }

    #region 魔改

    /// <summary>
    /// 最少游戏时间 hours
    /// </summary>
    [Reactive]
    private double MinRunTime { get; set; } = 2;

    /// <summary>
    /// 自动切换游戏时间间隔 ms
    /// </summary>
    [Reactive]
    private double SwitchTime { get; set; } = 500;
    #endregion

    public override void Activation()
    {
        base.Activation();

        IdleRunStartOrStop_Click();
    }

    /// <summary>
    /// 启动或停止挂卡
    /// </summary>
    public async void IdleRunStartOrStop_Click()
    {
        if (SteamConnectService.Current.IsConnectToSteam) // 是否登录
        {
            if (SteamTool.IsRunningSteamProcess)
            {
                if (!RunLoaingState)
                {
                    RunLoaingState = true;
                    RunState = !RunState;

                    if (RunState)
                    {
                        await ReadyToGoIdle();
                    }
                    else
                    {
                        StopIdle();
                        RunOrStopAutoNext(false);
                    }
                    RunLoaingState = false;
                    Toast.Show(ToastIcon.Success, Strings.Idle_OperationSuccess);
                }
                else
                    Toast.Show(ToastIcon.Warning, Strings.Idle_LoaingTips);
            }
            else
                await MessageBox.ShowAsync(Strings.Idle_SteamNotRuning, button: MessageBox.Button.OK);
        }
        else
            await MessageBox.ShowAsync(Strings.Idle_NeedLoginSteam, button: MessageBox.Button.OK);
    }

    /// <summary>
    /// 手动切换下一个游戏
    /// </summary>
    public void ManualRunNext()
    {
        RunNextIdle();
    }

    #region PrivateFields

    /// <summary>
    /// 用户徽章和卡片数据
    /// </summary>
    private IEnumerable<Badge> Badges = Enumerable.Empty<Badge>();

    /// <summary>
    /// 最大并行运行游戏数量
    /// </summary>
    private int MaxIdleCount = 32;

    /// <summary>
    /// 暂停游戏自动切换
    /// </summary>
    private bool IsAutoNextPaused = false;

    /// <summary>
    /// 定时自动切换 CancelToken
    /// </summary>
    private CancellationTokenSource CancellationTokenSource = new();

    /// <summary>
    /// 重新加载
    /// </summary>
    private bool IsReloaded;

    #endregion

    #region Private Method
    private void ChangeRunTxt()
    {
        var count = IdleGameList.Count(x => x.Process != null);
        RuningCountTxt = Strings.Idle_RuningCount.Format(count, IdleGameList.Count);
        RunState = count > 0;
    }

    private async Task SteamAppsSort()
    {
        if (IdleSequentital == IdleSequentital.Default)
            IdleGameList.Add(SteamConnectService.Current.SteamApps.Items);
        else
        {
            Badges = await IdleCard.GetBadgesAsync(SteamConnectService.Current.CurrentSteamUser!.SteamId64.ToString());
            Badges = Badges.Where(x => x.CardsRemaining != 0); // 过滤可掉落卡片的游戏
            var appid_sorts = Enumerable.Empty<int>();
            switch (IdleSequentital)
            {
                case IdleSequentital.LeastCards:
                    appid_sorts = Badges.OrderBy(o => o.CardsRemaining).Select(s => s.AppId);
                    break;
                case IdleSequentital.Mostcards:
                    appid_sorts = Badges.OrderByDescending(o => o.CardsRemaining).Select(s => s.AppId);
                    break;
                case IdleSequentital.Mostvalue:
                    appid_sorts = Badges.OrderByDescending(o => o.RegularAvgPrice).Select(s => s.AppId);
                    break;
                default:
                    break;
            }
            var apps = SteamConnectService.Current.SteamApps.Items.OrderBy(o => appid_sorts.ToList().FindIndex(x => x == o.AppId)).ToList();
            IdleGameList.Add(apps);
        }

    }

    private async Task ReadyToGoIdle()
    {
        await SteamAppsSort();
        StartIdle();
        ChangeRunTxt();
    }

    /// <summary>
    /// 开始挂卡
    /// </summary>
    private void StartIdle()
    {
        if (!IdleGameList.Any())
            IdleComplete();

        if (IdleRule == IdleRule.OnlyOneGame)
        {
            CurrentIdle = IdleGameList.First();
            StartSoloIdle(CurrentIdle);
        }
        else
        {
            if (IdleRule == IdleRule.OneThenMany)
            {
                var canIdles = Badges.Where(z => z.MinutesPlayed / 60 >= MinRunTime).Select(s => s.AppId);
                var multi = IdleGameList.Where(x => canIdles.Contains((int)x.AppId));
                if (multi.Count() >= 1)
                {
                    PauseAutoNext(false);
                    StartSoloIdle(multi.First());
                }
                else
                {
                    PauseAutoNext(true);
                    StartMultipleIdle();
                }
            }
            else
            {
                var canIdles = Badges.Where(z => z.MinutesPlayed / 60 < MinRunTime).Select(s => s.AppId);
                var multi = IdleGameList.Where(x => canIdles.Contains((int)x.AppId));
                if (multi.Count() >= 2)
                {
                    PauseAutoNext(false);
                    StartSoloIdle(multi.First());
                }
                else
                {
                    PauseAutoNext(true);
                    StartMultipleIdle();
                }
            }
        }
    }

    private void RunNextIdle()
    {
        if (RunState)
        {
            StopIdle();
            if (CurrentIdle != null)
                IdleGameList.Remove(CurrentIdle);
            StartIdle();
        }

    }

    /// <summary>
    /// 单独运行游戏
    /// </summary>
    /// <param name="item"></param>
    private void StartSoloIdle(SteamApp item)
    {
        SteamConnectService.Current.RuningSteamApps.TryGetValue(item.AppId, out var runState);
        if (runState == null)
        {
            item.StartSteamAppProcess();
            SteamConnectService.Current.RuningSteamApps.TryAdd(item.AppId, item);
        }
        else
        {
            if (runState.Process == null || !runState.Process.HasExited)
            {
                runState.StartSteamAppProcess();
            }
            else
            {
                item.Process = runState.Process;
            }
        }
    }

    private void StartMultipleIdle()
    {
        foreach (var item in IdleGameList)
        {
            var badge = Badges.FirstOrDefault(x => x.AppId == item.AppId);

            if (badge == null)
                continue;

            if (badge.MinutesPlayed / 60 >= MinRunTime)
                StopSoloIdle(item);

            if (badge.MinutesPlayed / 60 < MinRunTime && IdleGameList.Count(x => x.Process != null) < MaxIdleCount)
                StartSoloIdle(item);
        }

        if (!IdleGameList.Any(x => x.Process != null))
            StartIdle();

    }

    /// <summary>
    /// 停止所有挂卡游戏
    /// </summary>
    private void StopIdle()
    {
        foreach (var item in IdleGameList)
        {
            SteamConnectService.Current.RuningSteamApps.TryGetValue(item.AppId, out var runState);
            if (runState != null)
            {
                runState.Process?.KillEntireProcessTree();
                SteamConnectService.Current.RuningSteamApps.TryRemove(item.AppId, out var remove);
                item.Process = null;
            }
            else
            {
                item.Process = null;
                SteamConnectService.Current.RuningSteamApps.TryAdd(item.AppId, item);
            }
        }
    }

    private void StopSoloIdle(SteamApp item)
    {
        SteamConnectService.Current.RuningSteamApps.TryGetValue(item.AppId, out var runState);
        if (runState != null)
        {
            runState.Process?.KillEntireProcessTree();
            SteamConnectService.Current.RuningSteamApps.TryRemove(item.AppId, out var remove);
            item.Process = null;
        }
        else
        {
            item.Process = null;
            SteamConnectService.Current.RuningSteamApps.TryAdd(item.AppId, item);
        }
    }

    /// <summary>
    /// 暂停自动切换下一个游戏
    /// </summary>
    /// <param name="b"></param>
    private void PauseAutoNext(bool b)
    {
        if (IsAutoNextOn && b && !IsAutoNextPaused)
        {
            IsAutoNextOn = false;
            IsAutoNextPaused = true;
        }
        else if (!IsAutoNextOn && !b && IsAutoNextPaused)
        {
            IsAutoNextPaused = false;
        }
    }

    /// <summary>
    /// 自动切换游戏开启或停止
    /// </summary>
    /// <param name="b"></param>
    private void RunOrStopAutoNext(bool b)
    {
        if (b)
        {
            if (RunState)
            {
                Task.Run(async () =>
                {
                    while (!CancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            await AutoNextTask();
                            await Task.Delay(TimeSpan.FromSeconds(SwitchTime), CancellationTokenSource.Token);
                        }
                        catch (Exception ex)
                        {
                            Toast.LogAndShowT(ex);
                        }
                    }
                });
            }
            Toast.Show(ToastIcon.Info, Strings.Idle_PleaseStartIdle);
        }
        else
        {
            if (IsAutoNextPaused)
                IsAutoNextPaused = false;
        }
    }

    /// <summary>
    /// 定时自动切换任务
    /// </summary>
    private async Task AutoNextTask()
    {
        if (IsAutoNextOn == false || IsAutoNextPaused == true)
        {
            CancellationTokenSource.Cancel();
            return;
        }
        if (Badges.Where(x => IdleGameList.Select(s => (int)s.AppId).Contains(x.AppId)).Sum(s => s.CardsRemaining) == 0)
        {
            CancellationTokenSource.Cancel();
            if (IsReloaded == false)
            {
                IsReloaded = true;
                await ReadyToGoIdle();
            }
            else
            {
                IsAutoNextOn = false;
                IdleComplete();
            }
            return;
        }
        else
        {
            IsReloaded = false;
            RunNextIdle();
        }
    }

    /// <summary>
    /// 挂卡完毕，没有需要挂卡的游戏
    /// </summary>
    private void IdleComplete()
    {
        MessageBox.Show(Strings.Idle_Complete, button: MessageBox.Button.OK);
    }
    #endregion

}