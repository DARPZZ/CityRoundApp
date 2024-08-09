using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Maps;

namespace Vamdrup_rundt.Services
{
    public class MapPinsUpdatedEvent
    {
        public IEnumerable<Pin> Pins { get; set; }
    }
}
