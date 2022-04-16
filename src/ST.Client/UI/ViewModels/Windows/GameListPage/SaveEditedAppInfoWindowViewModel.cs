using DynamicData;
using DynamicData.Binding;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System.Application.Models;
using System.Application.Services;
using System.Application.Settings;
using System.Application.UI.Resx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Properties;
using System.Reactive.Linq;

namespace System.Application.UI.ViewModels
{
    public class SaveEditedAppInfoWindowViewModel : WindowViewModel
    {
        public static string DisplayName => AppResources.GameList_EditedAppsSaveManger;

        public SaveEditedAppInfoWindowViewModel()
        {
            Title = GetTitleByDisplayName(DisplayName);


            SteamConnectService.Current.SteamApps
              .Connect()
              .Filter(x => x.IsEdited)
              .ObserveOn(RxApp.MainThreadScheduler)
              .Sort(SortExpressionComparer<SteamApp>.Ascending(x => x.AppId))
              .Bind(out _SteamEditedApps)
              .Subscribe(_ => this.RaisePropertyChanged(nameof(IsSteamEditedAppsEmpty)));

            LoadSteamEditedApps();
        }

        
        readonly ReadOnlyObservableCollection<SteamApp> _SteamEditedApps;
        public ReadOnlyObservableCollection<SteamApp> SteamEditedApps => _SteamEditedApps;


        public bool IsSteamEditedAppsEmpty => !SteamEditedApps.Any_Nullable();


        public void LoadSteamEditedApps()
        {
            SteamConnectService.Current.SteamApps.Refresh();
        }

        public async void SaveSteamEditedApps()
        {
            if (await ISteamService.Instance.SaveAppInfosToSteam())
            {
                if (await MessageBox.ShowAsync("修改的数据已保存到 Steam 本地文件中，是否立即重启Steam生效？", ThisAssembly.AssemblyTrademark, MessageBox.Button.OKCancel) == MessageBox.Result.OK)
                {
                    ISteamService.Instance.TryKillSteamProcess();
                    ISteamService.Instance.StartSteam();
                }
            }
        }

        public async void ClearSteamEditedApps()
        {
            if (await MessageBox.ShowAsync("确定要清空所有的已修改数据吗？(该操作不可还原)", ThisAssembly.AssemblyTrademark, MessageBox.Button.OKCancel) == MessageBox.Result.OK)
            {
                foreach (var item in SteamEditedApps) 
                {
                    
                }
            }
        }

        public static void EditSteamApp(SteamApp app) => GameListPageViewModel.EditAppInfoClick(app);

        public static void NavAppToSteamView(SteamApp app) => GameListPageViewModel.NavAppToSteamView(app);

        public static void OpenFolder(SteamApp app) => GameListPageViewModel.OpenFolder(app);

        public static void OpenAppStoreUrl(SteamApp app) => GameListPageViewModel.OpenAppStoreUrl(app);

        public static void OpenSteamDBUrl(SteamApp app) => GameListPageViewModel.OpenSteamDBUrl(app);

        public static void OpenSteamCardUrl(SteamApp app) => GameListPageViewModel.OpenSteamCardUrl(app);
    }
}