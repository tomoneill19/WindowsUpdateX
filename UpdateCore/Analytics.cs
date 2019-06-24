using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateCore
{
    public class Analytics
    {
        public static void DoAnalysis()   // PLANNED: Check for issue patterns with data
        {
            Database db = new Database();
            db.Write("dbo.DoAnalysis");
        }
    }

}
