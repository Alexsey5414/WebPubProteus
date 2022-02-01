using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebTestProteus.Classes.Dalayer;

namespace WebTestProteus.Interfaces

{
    public interface IDelayer: IDisposable
    {
         event DalayerHandler OnDelay;
    }
}
