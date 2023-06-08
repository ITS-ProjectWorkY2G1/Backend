using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WatchModels
{
    public class FullSessionViewModel : Session
    {
        public List<Smartwatch>? Smartwatches { get; set; }
    }
}
