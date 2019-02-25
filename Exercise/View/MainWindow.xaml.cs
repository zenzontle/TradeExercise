using Exercise.Helpers;
using Exercise.Model;
using Exercise.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace Exercise.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<OpenWindowMessage<UserModel>>(this, OnOpenUserView);
        }

        private void OnOpenUserView(OpenWindowMessage<UserModel> obj)
        {
            if (obj.Type == WindowType.Modal)
            {
                var addEditUserViewModel = SimpleIoc.Default.GetInstance<AddEditUserViewModel>();
                addEditUserViewModel.User = obj.Parameter;
                var modalWindow = new AddEditUserView
                {
                    DataContext = addEditUserViewModel
                };
                modalWindow.ShowDialog();
                Messenger.Default.Send(addEditUserViewModel.Result);
                SimpleIoc.Default.Unregister(addEditUserViewModel);
            }
        }
    }
}