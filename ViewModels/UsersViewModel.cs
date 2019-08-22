using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace WpfApp1.ViewModels
{
    class UsersViewModel
    {
        public ObservableCollection<UserViewModel> Users { get; set; }

        public UsersViewModel()
        {
            Users = new ObservableCollection<UserViewModel>();
        }
    }
}
