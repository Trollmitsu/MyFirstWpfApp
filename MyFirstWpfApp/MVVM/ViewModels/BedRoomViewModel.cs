using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWpfApp.MVVM.ViewModels
{
    internal class BedRoomViewModel
    {
        public string Title { get; set; } = "BedRoom";

        public string Temperature { get; set; } = "23°C";

        public string Humidity { get; set; } = "33%";
    }
}
