using Manufacturing.WinApp.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manufacturing.WinApp.Views.UserManagment;
using Windows.UI.Xaml;
using System.Windows.Input;

namespace Manufacturing.WinApp.ViewModels
{
    // Driver of the user management page
    [MenuScreen(typeof(UserListPage), "User Management")]
    [RequiredRole("Administrator")]
    public class UserListViewModel : BaseViewModel
    {
        private ApiClient _apiClient;

        private bool _editing;
        public bool Editing
        {
            get { return _editing; }
            set
            {
                if (value != _editing)
                {
                    _editing = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _adding;
        public bool Adding
        {
            get { return _adding; }
            set
            {
                if (value != _adding)
                {
                    _adding = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private UserDetailViewModel _editUser;
        public UserDetailViewModel EditedUser
        {
            get { return _editUser; }
            set
            {
                if (value != _editUser)
                {
                    _editUser = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private UserSummaryViewModel _selectedUser;
        public UserSummaryViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (value != _selectedUser)
                {
                    _selectedUser = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<UserSummaryViewModel> _userList = new ObservableCollection<UserSummaryViewModel>();
        public ObservableCollection<UserSummaryViewModel> UserList
        {
            get { return _userList; }
            set
            {
                if (value != _userList)
                {
                    _userList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public UserListViewModel()
        {
            var apiUrl = string.Format("{0}/api/", ConfigSettings.ApiServiceUrl);
            _apiClient = new ApiClient(apiUrl, App.BearerToken);

            AddUserCommand = new RelayCommand(AddUser);
            EditUserCommand = new RelayCommand(EditUser);            
        }

        public async void Load()
        {
            var users = await _apiClient.GetData<IEnumerable<ApiUserSummary>>("user");
            //var users = new[] {
            //new ApiUserSummary { Id = "un1", Name = "ABC 123" },
            //new ApiUserSummary { Id = "un2", Name = "DEF 789" },
            //new ApiUserSummary { Id = "un3", Name = "GHI 456" } };
            
            foreach (var u in users)
                _userList.Add(new UserSummaryViewModel(u));
        }        

        public ICommand AddUserCommand { get; private set; }
        public ICommand EditUserCommand { get; private set; }
        
        private async void EditUser(object obj)
        {
            // Grab list of roles
            var availableRoles = await _apiClient.GetData<IEnumerable<ApiRole>>("role");
            //var availableRoles = new [] { new ApiRole { Id = "r1", Name = "R1" }, new ApiRole { Id = "r2", Name = "R2"}, new ApiRole { Id = "r3", Name = "R3" }};

            // Go out and grab the user details for editing
            var userDetail = await _apiClient.GetData<ApiUserDetail>(string.Format("user/{0}", SelectedUser.Id));
            //var userDetail = new ApiUserDetail { Id = SelectedUser.Id, Name = "UserX", UserName = "UserName", Roles = new [] {
            //    new ApiRole { Id = "r1", Name = "R1" }, new ApiRole { Id = "r3", Name = "R3"}
            //}            };

            EditedUser = new UserDetailViewModel(userDetail, availableRoles);
            Editing = true;
        }

        private async void AddUser(object obj)
        {
            var availableRoles = await _apiClient.GetData<IEnumerable<ApiRole>>("role");
            //var availableRoles = new[] { new ApiRole { Id = "r1", Name = "R1" }, new ApiRole { Id = "r2", Name = "R2" }, new ApiRole { Id = "r3", Name = "R3" } };

            EditedUser = new UserDetailViewModel(null, availableRoles);
            Editing = true;
            Adding = true;
        }

        public async void EditUserCommit()
        {            
            var apiUrl = string.Format("{0}/api/", ConfigSettings.ApiServiceUrl);
            var apiClient = new ApiClient(apiUrl, App.BearerToken);

            if (Adding)
            {
                var result = await apiClient.PostData<ApiUserCreate, ApiUserDetail>("user", EditedUser.ToApiCreate());
                UserList.Add(new UserSummaryViewModel(result));
                //UserList.Add(new UserSummaryViewModel { Id = "new", Name = "New guy" });
            }
            else
            {
                var result = await apiClient.PutData<ApiUserUpdate>(string.Format("user/{0}", EditedUser.Id),EditedUser.ToApiUpdate());
                var summary = UserList.First(x => x.Id == EditedUser.Id);
                summary.Name = EditedUser.Name;
            }
            
            Editing = false;
            Adding = false;
        }
    }

    // Bind to UI to select roles for a user
    public class RoleViewModel : BaseViewModel
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }

    // Bind to UI to add user or edit existing user
    public class UserDetailViewModel : BaseViewModel
    {
        public UserDetailViewModel()
        {
        }

        public UserDetailViewModel(ApiUserDetail userInfo, IEnumerable<ApiRole> roles)
        {
            if (userInfo != null)
            {
                Id = userInfo.Id;
                Name = userInfo.Name;
                UserName = userInfo.UserName;
            }
            RoleList = new ObservableCollection<RoleViewModel>(
                roles.Select(ar => new RoleViewModel
                {
                    Id = ar.Id,
                    Name = ar.Name,
                    IsSelected = userInfo != null && userInfo.Roles.Any(r => r.Id == ar.Id)
                }));
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<RoleViewModel> _roleList = new ObservableCollection<RoleViewModel>();
        public ObservableCollection<RoleViewModel> RoleList
        {
            get { return _roleList; }
            set
            {
                if (value != _roleList)
                {
                    _roleList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ApiUserCreate ToApiCreate()
        {
            var toCreate = new ApiUserCreate
            {
                Name = this.Name,
                MailNickName = this.UserName,
                Password = this.Password,
                DesiredRoleIds = this.RoleList.Where(x => x.IsSelected).Select(x => x.Id)
            };

            return toCreate;
        }

        public ApiUserUpdate ToApiUpdate()
        {
            var toUpdate = new ApiUserUpdate
            {
                Id = this.Id,
                Name = this.Name,
                DesiredRoleIds = this.RoleList.Where(x => x.IsSelected).Select(x => x.Id)
            };

            return toUpdate;
        }
    }

    // Smaller representations of a user that appear in the list
    // for user to choose from
    public class UserSummaryViewModel : BaseViewModel
    {
        public UserSummaryViewModel()
        {
        }

        public UserSummaryViewModel(ApiUserDetail dtl)
        {
            Id = dtl.Id;
            Name = dtl.Name;
        }

        public UserSummaryViewModel(ApiUserSummary apiUser)
        {
            Id = apiUser.Id;
            Name = apiUser.Name;
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }

    // Classes to map what we get back from the api 
    public class ApiUserDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public IEnumerable<ApiRole> Roles { get; set; }
    }
    public class ApiRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class ApiUserSummary
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class ApiUserCreate
    {
        public string Name { get; set; }
        public string MailNickName { get; set; } // e.g., MailNickName@domain
        public string Password { get; set; }
        public IEnumerable<string> DesiredRoleIds { get; set; }
    }
    public class ApiUserUpdate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> DesiredRoleIds { get; set; }
    }
}
