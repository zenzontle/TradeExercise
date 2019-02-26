using Exercise.Core.Entity;
using GalaSoft.MvvmLight;

namespace Exercise.Model
{
    public class UserModel : ViewModelBase
    {
        public UserModel()
        : this(new User())
        {
        }

        public UserModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            AvatarId = user.AvatarId ?? 0;
        }

        private int _id;
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        private int _avatarId;
        public int AvatarId
        {
            get => _avatarId;
            set => Set(ref _avatarId, value);
        }

        private string _avatar;
        public string Avatar
        {
            get => _avatar;
            set => Set(ref _avatar, value);
        }

        public User ToUser()
        {
            return new User
            {
                Id = Id,
                Name = Name,
                Email = Email,
                AvatarId = AvatarId != 0 ? AvatarId : (int?)null
            };
        }

        public UserModel ShallowCopy()
        {
            return (UserModel)MemberwiseClone();
        }
    }
}