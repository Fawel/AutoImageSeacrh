using System;
using System.Collections.Generic;
using System.Text;

namespace AIS.Application.PictureSearchers.Models
{
    public class IqdbSearchSettings
    {
        public ImagePoolSite[] SitesToSearch { get; private set; }
        public int MinimumSimularity { get; private set; }
        public bool ShowOnlyBestMatch { get; private set; }
    }
}
