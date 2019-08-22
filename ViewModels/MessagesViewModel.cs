using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace WpfApp1.ViewModels
{
    class MessagesViewModel
    {
        public ObservableCollection<MessageViewModel> Messages { get; set; }

        public MessagesViewModel()
        {
            Messages = new ObservableCollection<MessageViewModel>();
        }
    }
}
