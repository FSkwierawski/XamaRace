using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSMapa.BindingModel
{
    public class RaceMapBindingModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<RaceCheckpoint> RaceCheckpoints { get; set; }
    }
}