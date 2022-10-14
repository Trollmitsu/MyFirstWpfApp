using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWpfApp.MVVM.ViewModels
{
    internal class MainMenuViewModel : ObservableObject
    {
        public MainMenuViewModel()
        {
            KitchenViewModel = new KitchenViewModel();
            BedRoomViewModel = new BedRoomViewModel();
            LivingRoomViewModel = new LivingRoomViewModel();

            KitchenViewCommand = new RelayCommand(x => { CurrentView = KitchenViewModel; });
            BedRoomViewCommand = new RelayCommand(x => { CurrentView = BedRoomViewModel; });
            LivingRoomViewCommand = new RelayCommand(x => { CurrentView = LivingRoomViewModel; });

            CurrentView = KitchenViewModel;
        }

        private object _currentView;

        public RelayCommand KitchenViewCommand { get; set; }
        public KitchenViewModel KitchenViewModel { get; set; }
        public RelayCommand BedRoomViewCommand { get; set; }
        public BedRoomViewModel BedRoomViewModel { get; set; }
        public RelayCommand LivingRoomViewCommand { get; set; }
        public LivingRoomViewModel LivingRoomViewModel { get; set; }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
    }
}
