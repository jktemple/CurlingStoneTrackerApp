using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurlingStoneTrackerApp
{
    internal class UUIDGetter
    {
        public static string NameFromUUID(string uuid)
        {
            if(uuid == "b8566365-220e-4b83-bf0b-8770b381f8e2")
            {
                return "Accelerometer";
            } else if (uuid == "78f0e4b1-889c-4c82-a73d-aa967131282f")
            {
                return "Linear Accelerometer";
            } else if (uuid == "5cf23815-03e2-4385-b484-213d78255c81")
            {
                return "Orientation";
            } else if(uuid == "22f8fb96-b11b-4862-8cbf-971ed338ef4c")
            {
                return "Angular Velocity";
            } else
            {
                return string.Empty;
            }
        }
    }
}
