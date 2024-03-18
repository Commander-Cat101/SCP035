using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCP035
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        public int ChanceToSpawn { get; set; } = 80;

        public SCP035 Scp035Config { get; set; } = new SCP035();

        public SCP035Item Scp035ItemConfig { get; set; } = new SCP035Item();
    }
}
