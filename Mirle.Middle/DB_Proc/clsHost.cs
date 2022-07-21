﻿using System.Collections.Generic;
using Mirle.Def;

namespace Mirle.Middle.DB_Proc
{
    public class clsHost
    {
        private clsMiddleCmd middleCmd;
        private static object _Lock = new object();
        private static bool _IsConn = false;
        public static bool IsConn
        {
            get { return _IsConn; }
            set
            {
                lock(_Lock)
                {
                    _IsConn = value;
                }
            }
        }

        public clsHost(clsDbConfig config)
        {
            middleCmd = new clsMiddleCmd(config);
        }

        public clsMiddleCmd GetMiddleCmd() => middleCmd;
    }
}
