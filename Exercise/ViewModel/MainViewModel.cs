using Exercise.Core.Client.Services;
using Exercise.Helpers;
using Exercise.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MvvmDialogs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Exercise.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IUserDataService _userDataService;
        private readonly IAvatarDataService _avatarDataService;
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IUserDataService userDataService, IAvatarDataService avatarDataService,
            IDialogService dialogService)
        {
            if (IsInDesignMode)
            {
                Title = "MainWindow (Design Mode)";
                // Code runs in Blend --> create design time data.
            }
            else
            {
                Title = "MainWindow";
                // Code runs "for real"
            }

            DispatcherHelper.Initialize();
            Users = new ObservableCollection<UserModel>();

            LoadedCommand = new RelayCommand(OnLoaded);
            RefreshCommand = new RelayCommand(OnRefresh);
            AddUserCommand = new RelayCommand(OnAddUser);
            EditUserCommand = new RelayCommand(OnEditUser);
            DeleteUserCommand = new RelayCommand(OnDeleteUser);

            _userDataService = userDataService;
            _avatarDataService = avatarDataService;
            _dialogService = dialogService;

            Messenger.Default.Register<bool>(this, OnUsersChanged);
        }

        public string Title { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public RelayCommand LoadedCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddUserCommand { get; }
        public RelayCommand EditUserCommand { get; }
        public RelayCommand DeleteUserCommand { get; }
        public ObservableCollection<UserModel> Users { get; }
        public UserModel SelectedUser { get; set; }

        private void OnLoaded()
        {
            LoadUsers();
        }

        private void OnRefresh()
        {
            LoadUsers();
        }

        private void OnAddUser()
        {
            Messenger.Default.Send(
                new OpenWindowMessage<UserModel>
                {
                    Type = WindowType.Modal,
                    Parameter = new UserModel()
                });
        }

        private void OnEditUser()
        {
            if (SelectedUser != null)
            {
                Messenger.Default.Send(
                    new OpenWindowMessage<UserModel>
                    {
                        Type = WindowType.Modal,
                        Parameter = SelectedUser
                    });
            }
        }

        private async void OnDeleteUser()
        {
            if (SelectedUser != null)
            {
                var result = _dialogService.ShowMessageBox(this, "Are you sure you want to delete the user?",
                    string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Yes)
                {
                    var deleted = await _userDataService.DeleteUser(SelectedUser.ToUser());
                    if (deleted)
                    {
                        await LoadUsers();
                    }
                }
            }
            return;
        }

        private Task LoadUsers()
        {
            IsBusy = true;
            ThreadPool.QueueUserWorkItem(async o =>
            {
                var users = await _userDataService.GetUsers();

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Users.Clear();
                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            if (user.AvatarId.HasValue)
                            {
                                LoadAvatar(user.Id, user.AvatarId.Value);
                            }
                            Users.Add(new UserModel(user));
                        }
                    }
                    IsBusy = false;
                });
            });
            return Task.CompletedTask;
        }

        private Task LoadAvatar(int userId, int avatarId)
        {
            ThreadPool.QueueUserWorkItem(async o =>
            {
                var avatarPath = await _avatarDataService.GetAvatar(avatarId);

                if (!string.IsNullOrWhiteSpace(avatarPath))
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Users.FirstOrDefault(x => x.Id == userId).Avatar = avatarPath;
                    });
                }
            });
            return Task.CompletedTask;
        }

        private void OnUsersChanged(bool changed)
        {
            if (changed)
            {
                LoadUsers();
            }
        }
    }
}