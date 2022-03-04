using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portable_BMU_App
{
    public enum SavedDataType
    {
        Volume = 0x01,
        TransVerseImage = 0x02,
        LocationData = 0x04,
        CoronalImage = 0x08,
        NavigationData =  0x10
    }

}
