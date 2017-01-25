using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitoManager
{
    class Program
    {
        static void Main(string[] args)
        {
            CognitoLoginManager logManager = new CognitoLoginManager();
            logManager.Initialize();

        }
    }
}
