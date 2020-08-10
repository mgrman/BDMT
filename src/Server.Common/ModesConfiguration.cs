using System;
using System.Collections.Generic;
using System.Text;

namespace BDMT.Server
{
    public class ModesConfiguration
    {
        public List<ModeConfig> SupportedModes { get; } = new List<ModeConfig>();
    }

    public class ModeConfig
    {
        public ModeConfig(string name, string viewPath)
        {
            Name = name;
            ViewPath = viewPath;
        }

        public string Name { get; }
        public string ViewPath { get; }
    }
}