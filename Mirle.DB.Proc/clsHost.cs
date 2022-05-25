﻿using System.Collections.Generic;
using Mirle.DataBase;
using Mirle.Def;

namespace Mirle.DB.Proc
{
    public class clsHost
    {
        private readonly clsCmd_Mst CMD_MST;
        private readonly clsCmd_Dtl cmd_Dtl;
        private readonly clsSno SNO;
        private readonly clsLocMst LocMst;
        private readonly clsLoc_Dtl Loc_Dtl;
        private readonly clsAlarmData ALARMDATA;
        private readonly clsEQ_Alarm EQ_Alarm;
        private readonly clsUnitStsLog unitStsLog;
        private readonly clsUnitModeDef unitModeDef;
        private readonly clsL2LCount L2LCount;
        private readonly clsProc proc;
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
            CMD_MST = new clsCmd_Mst(config);
            cmd_Dtl = new clsCmd_Dtl(config);
            SNO = new clsSno(config);
            LocMst = new clsLocMst(config);
            Loc_Dtl = new clsLoc_Dtl(config);
            ALARMDATA = new clsAlarmData(config);
            unitStsLog = new clsUnitStsLog(config);
            unitModeDef = new clsUnitModeDef(config);
            EQ_Alarm = new clsEQ_Alarm(config);
            L2LCount = new clsL2LCount(config);
            proc = new clsProc(config);
        }

        public clsCmd_Mst GetCmd_Mst()
        {
            return CMD_MST;
        }

        public clsCmd_Dtl GetCmd_Dtl() => cmd_Dtl;
        public clsLocMst GetLocMst()
        {
            return LocMst;
        }

        public clsLoc_Dtl GetLoc_Dtl() => Loc_Dtl;
        public clsSno GetSNO()
        {
            return SNO;
        }

        public clsAlarmData GetAlarmData()
        {
            return ALARMDATA;
        }

        public clsEQ_Alarm GetEQ_Alarm()
        {
            return EQ_Alarm;
        }

        public clsUnitStsLog GetUnitStsLog()
        {
            return unitStsLog;
        }

        public clsUnitModeDef GetUnitModeDef()
        {
            return unitModeDef;
        }

        public clsL2LCount GetL2LCount()
        {
            return L2LCount;
        }

        public clsProc GetProc() => proc;
    }
}