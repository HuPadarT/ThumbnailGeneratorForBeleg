using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThumbnailGeneratorForBeleg.Enums
{
    public enum State
    {
        Init = 0,
        Processing = 1,
        Saving = 2,
        Finised = 3,
        Error = 4,
    }
}