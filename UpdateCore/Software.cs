using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateCore
{
    /***************************************************************
     * Abstract Class for types of software E.G Updates or Programs
     * -> All software objects need populating with data from the DB
     * -> All software types need to be able to save back to the DB
     ***************************************************************/
    public abstract class Software 
    {
        abstract protected void populate();
        abstract protected void save();

    }
}
