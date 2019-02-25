using Exercise.Core.Client.Services;
using Exercise.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;

namespace Exercise.ViewModel
{
    public class AddEditUserViewModel : ViewModelBase
    {
        private readonly IUserDataService _userDataService;
        private readonly IAvatarDataService _avatarDataService;
        private readonly IDialogService _dialogService;

        public AddEditUserViewModel(IUserDataService userDataService, IAvatarDataService avatarDataService,
            IDialogService dialogService)
        {
            _userDataService = userDataService;
            _avatarDataService = avatarDataService;
            _dialogService = dialogService;

            SelectAvatarCommand = new RelayCommand(OnSelectAvatar);
            ClearAvatarCommand = new RelayCommand(OnClearAvatar);
            SaveCommand = new RelayCommand(OnSaveCommand);
            CancelCommand = new RelayCommand(OnCancelCommand);
        }

        public bool IsEditMode => User.Id != 0;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public UserModel User { get; set; }

        public RelayCommand SelectAvatarCommand { get; }
        public RelayCommand ClearAvatarCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private bool _closeTrigger;
        public bool CloseTrigger
        {
            get => _closeTrigger;
            set => Set(ref _closeTrigger, value);
        }

        private bool _result;

        public bool Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        private void OnSelectAvatar()
        {
            var openFileSettings = new OpenFileDialogSettings
            {
                Filter = "Image files (*.jpg)|*.jpg"
            };
            var result = _dialogService.ShowOpenFileDialog(this, openFileSettings);
            if (result.HasValue && result.Value)
            {
                //TODO Validate file is lower than 256kb
                User.Avatar = openFileSettings.FileName;
            }
        }

        private void OnClearAvatar()
        {
            User.Avatar = string.Empty;
        }

        private async void OnSaveCommand()
        {
            if (string.IsNullOrWhiteSpace(User.Name))
            {
                _dialogService.ShowMessageBox(this, "Name is empty", string.Empty, System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(User.Email))
            {
                _dialogService.ShowMessageBox(this, "Email is empty", string.Empty, System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }
            if (!IsEditMode)
            {
                var createdUser = await _userDataService.CreateUser(User.ToUser());
                if (createdUser == null)
                {
                    _dialogService.ShowMessageBox(this, "Could not create user", string.Empty,
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                var avatarCreated = await _avatarDataService.CreateAvatar(createdUser.Id, User.Avatar);
                if (avatarCreated == null)
                {
                    _dialogService.ShowMessageBox(this,
                        "User created but could not create avatar, try updating it later", string.Empty,
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            else
            {
                var updatedUser = await _userDataService.UpdateUser(User.ToUser());
                if (updatedUser == null)
                {
                    _dialogService.ShowMessageBox(this, "Could not update user", string.Empty,
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                if (updatedUser.AvatarId.HasValue)
                {
                    if (string.IsNullOrWhiteSpace(User.Avatar))
                    {
                        var deleted = await _avatarDataService.DeleteAvatar(updatedUser.AvatarId.Value);
                        if (!deleted)
                        {
                            _dialogService.ShowMessageBox(this, "Could not delete avatar", string.Empty,
                                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        var avatarUpdated = await _avatarDataService.UpdateAvatar(updatedUser.Id, User.Avatar);
                        if (avatarUpdated == null)
                        {
                            _dialogService.ShowMessageBox(this, "Could not update avatar", string.Empty,
                                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                            return;
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(User.Avatar))
                {
                    var avatarCreated = await _avatarDataService.CreateAvatar(updatedUser.Id, User.Avatar);
                    if (avatarCreated == null)
                    {
                        _dialogService.ShowMessageBox(this,
                            "Could not create avatar, try updating it later", string.Empty,
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
            Result = true;
            CloseTrigger = true;
        }

        private void OnCancelCommand()
        {
            CloseTrigger = true;
        }
    }
}