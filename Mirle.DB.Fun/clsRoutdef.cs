﻿using Mirle.DataBase;
using Mirle.DB.Fun.Events;
using Mirle.Def;
using Mirle.Def.U2NMMA30;
using Mirle.MapController;
using Mirle.Middle;
using Mirle.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.WebAPI;
using Mirle.WebAPI.V2BYMA30.ReportInfo;

namespace Mirle.DB.Fun
{
    public class clsRoutdef
    {
        public delegate void NeedShelfToShelfEventHandler(object sender, NeedShelfToShelfArgs e);
        public event NeedShelfToShelfEventHandler OnNeedShelfToShelfEvent;
        private bool reportedFlag = false;
        protected readonly object _Lock = new object();
        private clsTool tool = new clsTool();
        private clsLocMst LocMst = new clsLocMst();
        private clsCmd_Mst Cmd_Mst = new clsCmd_Mst();
        private WebAPI.V2BYMA30.clsHost api = new WebAPI.V2BYMA30.clsHost();

        public string GetLocaionByCmdMode(string sCmdMode, string sCmdSts, string sDeviceID, string sLocation, DataBase.DB db)
        {
            string location = ""; bool IsTeach = false; int iRet = DBResult.Initial;
            if (sCmdSts == clsConstValue.CmdSts.strCmd_Cancel_Wait)
            {
                switch (sCmdMode)
                {
                    case clsConstValue.CmdMode.StockOut:
                    case clsConstValue.CmdMode.L2L:
                        iRet = LocMst.CheckIsTeach(sDeviceID, sLocation, ref IsTeach, db);
                        if (iRet != DBResult.Exception)
                            location = IsTeach ? Location.LocationID.Teach.ToString() : Location.LocationID.Shelf.ToString();
                        break;
                    default:
                        location = sLocation;
                        break;
                }
            }
            else
            {
                switch (sCmdMode)
                {
                    case clsConstValue.CmdMode.StockIn:
                    case clsConstValue.CmdMode.L2L:
                        iRet = LocMst.CheckIsTeach(sDeviceID, sLocation, ref IsTeach, db);
                        if (iRet != DBResult.Exception)
                            location = IsTeach ? Location.LocationID.Teach.ToString() : Location.LocationID.Shelf.ToString();
                        break;
                    default:
                        location = sLocation;
                        break;
                }
            }

            return location;
        }

        public bool FunGetLocation(CmdMstInfo cmd, MapHost Router, ref Location sLoc_Start, ref Location sLoc_End, DataBase.DB db)
        {
            try
            {
                if (cmd.Cmd_Sts == clsConstValue.CmdSts.strCmd_Initial)
                {
                    switch(cmd.Cmd_Mode)
                    {
                        case clsConstValue.CmdMode.S2S:
                        case clsConstValue.CmdMode.StockIn:
                            sLoc_Start = GetCurLoc_Inital_ByStnNo(cmd, Router);
                            break;
                        default:
                            sLoc_Start = GetCurLoc_Inital(cmd, Router, db);
                            break;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(cmd.CurLoc))
                        sLoc_Start = GetCurLocation(cmd, Router, cmd.CurDeviceID, cmd.CurLoc, db);
                    else
                    {
                        string sRemark = $"Error: 執行中的命令{Parameter.clsCmd_Mst.Column.CurLoc}不該為空" +
                                   $" => <{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{cmd.Cmd_Sno}";
                        if (sRemark != cmd.Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                        }

                        return false;
                    }
                }

                if (sLoc_Start == null) return false;
                sLoc_End = GetFinialDestination(cmd, Router, db);
                if (sLoc_End == null) return false;

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public Location GetCurLocation(CmdMstInfo cmd, MapHost Router, string DeviceID, string HostLocation, DataBase.DB db)
        {
            try
            {
                Location Start = Router.GetLocation(DeviceID, HostLocation);
                if (Start == null)
                {
                    string sRemark = $"Error: 取得CurLocation失敗 => <DeviceID> {DeviceID} <Location> {HostLocation}";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return null;
                }

                return Start;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return null;
            }
        }

        public Location GetFinialDestination(CmdMstInfo cmd, MapHost Router, DataBase.DB db)
        {
            try
            {
                Location End = null; string sRemark = "";
                string Loc_End = cmd.Cmd_Mode == clsConstValue.CmdMode.L2L ? cmd.New_Loc : cmd.Loc;
                switch (cmd.Cmd_Mode)
                {
                    case clsConstValue.CmdMode.StockIn:
                        End = Router.GetLocation(cmd.Equ_No, Location.LocationID.Shelf.ToString());
                        break;
                    case clsConstValue.CmdMode.L2L:
                        bool IsTeach = false;
                        int iRet = LocMst.CheckIsTeach(tool.funGetEquNoByLoc(Loc_End).ToString(), Loc_End, ref IsTeach, db);
                        if (iRet == DBResult.Exception)
                        {
                            sRemark = $"Error: 確認是否是校正儲位失敗 => <Loc>{Loc_End}";
                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return null;
                        }

                        string sLocationID = IsTeach ? Location.LocationID.Teach.ToString() : Location.LocationID.Shelf.ToString();
                        End = Router.GetLocation(tool.funGetEquNoByLoc(Loc_End).ToString(), sLocationID);
                        break;
                    case clsConstValue.CmdMode.S2S:
                    case clsConstValue.CmdMode.StockOut:
                        string sBuffername = cmd.Cmd_Mode == clsConstValue.CmdMode.S2S ? cmd.New_Loc : cmd.Stn_No;

                        ConveyorInfo s = new ConveyorInfo();
                        //Case: double destination
                        if(cmd.Stn_No.Contains(','))
                        {
                            bool isFindLocation = false;
                            string[] multi_destination = cmd.Stn_No.Split(',');
                            for(int i = 0; i < multi_destination.Length; i++)
                            {

                                BufferStatusQueryInfo info = new BufferStatusQueryInfo
                                {
                                    bufferId = multi_destination[i],
                                };
                                BufferStatusReply reply = new BufferStatusReply();
                                
                                s = ConveyorDef.GetBuffer(multi_destination[i]);

                                if (api.GetBufferStatusQuery().FunReport(info, s.API.IP, ref reply))
                                {
                                    if (reply.jobId == "00000" || string.IsNullOrEmpty(reply.jobId))
                                    {
                                        sBuffername = s.BufferName;
                                        isFindLocation = true;
                                        break;
                                    }
                                }
                            }
                            if(!isFindLocation)
                            {
                                throw new Exception($"暫時無空撿料口使用！ jobId = {cmd.Cmd_Sno}.");
                            }
                        }
                        else
                        {
                            s = ConveyorDef.GetBuffer(sBuffername);
                        }
                        
                        string sDeviceId = tool.GetDeviceId(s.BufferName);
                        if (sDeviceId == "") 
                            End = Router.GetLocation(s.ControllerID, s.BufferName);
                        else
                            End = Router.GetLocation(sDeviceId, s.BufferName);
                        if (End == null)
                        {
                            for (int i = 0; i < ConveyorDef.DeviceID_AGV_Router.Length; i++)
                            {
                                End = Router.GetLocation(ConveyorDef.DeviceID_AGV_Router[i], s.BufferName);
                                if (End == null) continue;
                                else break;
                            }
                        }

                        break;
                }

                return End;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return null;
            }
        }

        public Location GetCurLoc_Inital(CmdMstInfo cmd, MapHost Router, DataBase.DB db)
        {
            bool IsTeach = false;
            int iRet = LocMst.CheckIsTeach(cmd.Equ_No, cmd.Loc, ref IsTeach, db);
            if (iRet == DBResult.Exception)
            {
                string sRemark = $"Error: 確認是否是校正儲位失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{cmd.Loc}";
                if (sRemark != cmd.Remark)
                {
                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                }

                return null;
            }

            string sLocationID = IsTeach ? Location.LocationID.Teach.ToString() : Location.LocationID.Shelf.ToString();
            return GetCurLocation(cmd, Router, cmd.Equ_No, sLocationID, db);
        }

        public Location GetCurLoc_Inital_ByStnNo(CmdMstInfo cmd, MapHost Router)
        {
            var s = ConveyorDef.GetBuffer(cmd.Stn_No);
            string sDeviceID;
            if (s.BufferName == "E1-04") sDeviceID = ConveyorDef.DeviceID_Tower;
            else sDeviceID = s.DeviceId == "" ? s.ControllerID : s.DeviceId;

            return Router.GetLocation(sDeviceID, s.BufferName);
        }

        public bool CheckSourceIsOK(CmdMstInfo cmd, Location sLoc_Start, MidHost middle, DeviceInfo Device, WMS.Proc.clsHost wms, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                switch (sLoc_Start.LocationTypes)
                {
                    case LocationTypes.Conveyor:
                    case LocationTypes.EQ:
                    case LocationTypes.IO:
                        string sCmdSno_CV = "";
                        if (!middle.CheckIsInReady(Device, sLoc_Start, ref sCmdSno_CV))
                        {
                            sRemark = $"Error: {sLoc_Start.LocationId}沒發送出Ready";

                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }
                        else if(sCmdSno_CV != "00000" && cmd.Cmd_Sno != sCmdSno_CV)
                        {
                            sRemark = $"Error: {sLoc_Start.LocationId}上的任務號不一致 => {sCmdSno_CV}";

                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }

                        break;
                    case LocationTypes.Shelf:
                        bool bCheckOutside = false; string sLocDD = ""; bool IsEmpty_DD = false; string BoxID_DD = "";
                        int iRet = wms.GetLocMst().CheckLocIsOutside(cmd.Loc, ref bCheckOutside, ref sLocDD, ref IsEmpty_DD, ref BoxID_DD);
                        if (iRet == DBResult.Success)
                        {
                            if (bCheckOutside)
                            {
                                CmdMstInfo cmd_DD = new CmdMstInfo();
                                iRet = Cmd_Mst.FunCheckHasCommand(sLocDD, ref cmd_DD, db);
                                if (iRet == DBResult.Success)
                                {
                                    bool bCheckCanDo = true;
                                    if (IsEmpty_DD)
                                    {
                                        if (
                                            cmd_DD.Cmd_Sts == clsConstValue.CmdSts.strCmd_Running &&
                                            (
                                              (cmd_DD.Cmd_Mode == clsConstValue.CmdMode.StockIn && cmd_DD.Loc == sLocDD)
                                              ||
                                              (cmd_DD.Cmd_Mode == clsConstValue.CmdMode.L2L && cmd_DD.New_Loc == sLocDD)
                                            )
                                          )
                                        {
                                            bCheckCanDo = false;
                                        }
                                    }
                                    else
                                    {
                                        bCheckCanDo = false;
                                    }

                                    if (!bCheckCanDo)
                                    {
                                        sRemark = $"Error: 等候內儲位{sLocDD}命令完成 => " +
                                            $"<{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{cmd_DD.Cmd_Sno}";
                                        if (sRemark != cmd.Remark)
                                        {
                                            Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                        }

                                        return false;
                                    }
                                }
                                else if (iRet == DBResult.NoDataSelect && !IsEmpty_DD)
                                {
                                    lock(_Lock)
                                    {
                                        if (!reportedFlag)
                                        {
                                            reportedFlag = true;
                                            OnNeedShelfToShelfEvent?.Invoke(this, new NeedShelfToShelfArgs(Device.DeviceID, sLocDD, BoxID_DD));
                                            reportedFlag = false;
                                            clsWriLog.Log.FunWriLog(WriLog.clsLog.Type.Debug, $"觸發庫對庫Event => " +
                                                $"<{Parameter.clsCmd_Mst.Column.Loc}>{sLocDD} <{Parameter.clsCmd_Mst.Column.BoxID}>{BoxID_DD}");
                                        }
                                    }

                                    sRemark = $"通知WMS產生內儲位{sLocDD}的庫對庫命令";
                                    if (sRemark != cmd.Remark)
                                    {
                                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                    }

                                    return false;
                                }
                                else if (iRet == DBResult.Exception)
                                {
                                    sRemark = $"Error: 找尋儲位命令失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{sLocDD}";
                                    if (sRemark != cmd.Remark)
                                    {
                                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                    }

                                    return false;
                                }
                                else { }
                            }
                        }
                        else
                        {
                            sRemark = $"Error: 取得WMS儲位資料失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{cmd.Loc}";
                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }

                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool CheckSourceIsOK_NonASRS(CmdMstInfo cmd, Location sLoc_Start, MidHost middle, ConveyorInfo buffer, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                switch (sLoc_Start.LocationTypes)
                {
                    case LocationTypes.Conveyor:
                    case LocationTypes.EQ:
                    case LocationTypes.IO:
                        string sCmdSno_CV = "";
                        if (!middle.CheckIsInReady(buffer, ref sCmdSno_CV))
                        {
                            sRemark = $"Error: {sLoc_Start.LocationId}沒發送出Ready";

                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }
                        else if (sCmdSno_CV != "00000" && cmd.Cmd_Sno != sCmdSno_CV)
                        {
                            sRemark = $"Error: {sLoc_Start.LocationId}上的任務號不一致 => {sCmdSno_CV}";

                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }

                        break;
                    case LocationTypes.Shelf:
                        return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool CheckSourceIsOK(CmdMstInfo cmd, Location sLoc_Start, MidHost middle, DeviceInfo Device, WMS.Proc.clsHost wms, 
            ref bool IsDoubleCmd, ref CmdMstInfo cmd_DD, ref string[] sCmdSno_CV, DataBase.DB db)
        {
            try
            {
                string sRemark = ""; int iRet = DBResult.Initial;
                switch (sLoc_Start.LocationTypes)
                {
                    case LocationTypes.Conveyor:
                    case LocationTypes.EQ:
                    case LocationTypes.IO:
                        var buffer = tool.GetBuffer(Device, sLoc_Start);
                        var group = tool.GetGroup(Device, buffer);
                        var lst = group.Where(r => r != buffer);
                        ConveyorInfo buffer_another = new ConveyorInfo();
                        foreach(var c in lst)
                        {
                            buffer_another = c;
                        }

                        if(buffer.DoubleType == DoubleType.Right)
                        {
                            #region 左右不一定都要有東西
                            if (!middle.CheckIsInReady(buffer, ref sCmdSno_CV[1]))
                            {
                                sRemark = $"Error: {buffer.BufferName}沒發送出Ready";

                                if (sRemark != cmd.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                }

                                return false;
                            }

                            if (!middle.CheckIsInReady(buffer_another, ref sCmdSno_CV[0]))
                            {
                                if (sCmdSno_CV[0] == "00000")
                                    IsDoubleCmd = false;
                                /*
                                sRemark = $"Error: {buffer_another.BufferName}沒發送出Ready";

                                if (sRemark != cmd.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                }

                                return false;*/
                            }
                            else
                                IsDoubleCmd = true;
                            if(IsDoubleCmd)
                            {
                                string sCmdSno_DD = middle.GetBufferCmd(buffer_another).ToString().PadLeft(5, '0');
                                bool bFlag = Cmd_Mst.FunGetCommand(sCmdSno_DD, ref cmd_DD, ref iRet, db);
                                if (!bFlag)
                                {
                                    sRemark = $"Error: 取得{buffer_another.BufferName}命令失敗 => " +
                                        $"<{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{sCmdSno_DD}";
                                    if (sRemark != cmd.Remark)
                                    {
                                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                    }

                                    return false;
                                }
                            }
                            #endregion 左右不一定都要有東西
                        }
                        else
                        {
                            if (!middle.CheckIsInReady(buffer, ref sCmdSno_CV[0]))
                            {
                                sRemark = $"Error: {buffer.BufferName}沒發送出Ready";

                                if (sRemark != cmd.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                }

                                return false;
                            }
                            else
                            {
                                bool IsLoad = false;
                                if(middle.CheckIsLoad(buffer_another, ref IsLoad))
                                {
                                    if(IsLoad)
                                    {
                                        //CmdMstInfo cmd_another = new CmdMstInfo();
                                        //if(Cmd_Mst.getcom)
                                        #region 左右都有
                                        IsDoubleCmd = true;
                                        if (!middle.CheckIsInReady(buffer_another, ref sCmdSno_CV[1]))
                                        {
                                            sRemark = $"Error: {buffer_another.BufferName}沒發送出Ready";

                                            if (sRemark != cmd.Remark)
                                            {
                                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                            }

                                            return false;
                                        }

                                        string sCmdSno_DD = middle.GetBufferCmd(buffer_another).ToString().PadLeft(5, '0');
                                        bool bFlag = Cmd_Mst.FunGetCommand(sCmdSno_DD, ref cmd_DD, ref iRet, db);
                                        if (!bFlag)
                                        {
                                            sRemark = $"Error: 取得{buffer_another.BufferName}命令失敗 => " +
                                                $"<{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{sCmdSno_DD}";
                                            if (sRemark != cmd.Remark)
                                            {
                                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                            }

                                            return false;
                                        } 
                                        #endregion 左右都有
                                    }
                                }
                                else
                                {
                                    sRemark = $"Error: 確認{buffer_another.BufferName}荷有狀態失敗";

                                    if (sRemark != cmd.Remark)
                                    {
                                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                    }

                                    return false;
                                }
                            }
                        }

                        break;
                    case LocationTypes.Shelf:
                        bool IsTeach = false;
                        iRet = LocMst.CheckIsTeach(sLoc_Start.DeviceId, cmd.Loc, ref IsTeach, db);
                        if(iRet == DBResult.Exception)
                        {
                            sRemark = $"Error: 確認{cmd.Loc}是否是校正儲位失敗";

                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }

                        if (IsTeach)
                        {
                            IsDoubleCmd = false;
                        }
                        else
                        {
                            bool bCheckOutside = false; string sLocDD = ""; bool IsEmpty_DD = false; string BoxID_DD = "";
                            iRet = wms.GetLocMst().CheckLocIsOutside(cmd.Loc, ref bCheckOutside, ref sLocDD, ref IsEmpty_DD, ref BoxID_DD);
                            if (iRet == DBResult.Success)
                            {
                                cmd_DD = new CmdMstInfo();
                                iRet = Cmd_Mst.FunCheckHasCommand(sLocDD, ref cmd_DD, db);
                                if (iRet == DBResult.Success && cmd_DD.Cmd_Sts == clsConstValue.CmdSts.strCmd_Initial &&
                                    (cmd_DD.Cmd_Mode == clsConstValue.CmdMode.S2S || cmd_DD.Cmd_Mode == clsConstValue.CmdMode.StockOut) &&
                                     (cmd.Cmd_Mode == clsConstValue.CmdMode.S2S || cmd.Cmd_Mode == clsConstValue.CmdMode.StockOut))
                                {
                                    IsDoubleCmd = true;
                                }
                                else
                                {
                                    IsDoubleCmd = false;
                                    if (bCheckOutside)
                                    {
                                        if (iRet == DBResult.Success)
                                        {
                                            bool bCheckCanDo = true;
                                            if (IsEmpty_DD)
                                            {
                                                if (
                                                    cmd_DD.Cmd_Sts == clsConstValue.CmdSts.strCmd_Running &&
                                                    (
                                                      (cmd_DD.Cmd_Mode == clsConstValue.CmdMode.StockIn && cmd_DD.Loc == sLocDD)
                                                      ||
                                                      (cmd_DD.Cmd_Mode == clsConstValue.CmdMode.L2L && cmd_DD.New_Loc == sLocDD)
                                                    )
                                                  )
                                                {
                                                    bCheckCanDo = false;
                                                }
                                            }
                                            else
                                            {
                                                bCheckCanDo = false;
                                            }

                                            if (!bCheckCanDo)
                                            {
                                                sRemark = $"Error: 等候內儲位{sLocDD}命令完成 => " +
                                                    $"<{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{cmd_DD.Cmd_Sno}";
                                                if (sRemark != cmd.Remark)
                                                {
                                                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                                }

                                                return false;
                                            }
                                        }
                                        else if (iRet == DBResult.NoDataSelect && !IsEmpty_DD)
                                        {
                                            lock (_Lock)
                                            {
                                                if (!reportedFlag)
                                                {
                                                    reportedFlag = true;
                                                    OnNeedShelfToShelfEvent?.Invoke(this, new NeedShelfToShelfArgs(Device.DeviceID, sLocDD, BoxID_DD));
                                                    reportedFlag = false;
                                                    clsWriLog.Log.FunWriLog(WriLog.clsLog.Type.Debug, $"觸發庫對庫Event => " +
                                                        $"<{Parameter.clsCmd_Mst.Column.Loc}>{sLocDD} <{Parameter.clsCmd_Mst.Column.BoxID}>{BoxID_DD}");
                                                }
                                            }

                                            sRemark = $"通知WMS產生內儲位{sLocDD}的庫對庫命令";
                                            if (sRemark != cmd.Remark)
                                            {
                                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                            }

                                            return false;
                                        }
                                        else if (iRet == DBResult.Exception)
                                        {
                                            sRemark = $"Error: 找尋儲位命令失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{sLocDD}";
                                            if (sRemark != cmd.Remark)
                                            {
                                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                            }

                                            return false;
                                        }
                                        else { }
                                    }
                                }
                            }
                            else
                            {
                                sRemark = $"Error: 取得WMS儲位資料失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{cmd.Loc}";
                                if (sRemark != cmd.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                }

                                return false;
                            }
                        }

                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool CheckDestinationIsOK(CmdMstInfo cmd, Location sLoc_End, MidHost middle, DeviceInfo Device, WMS.Proc.clsHost wms, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                switch (sLoc_End.LocationTypes)
                {
                    case LocationTypes.Conveyor:
                    case LocationTypes.EQ:
                    case LocationTypes.IO:
                        if (!middle.CheckIsOutReady(Device, sLoc_End))
                        {
                            sRemark = $"Error: {sLoc_End.LocationId}沒發接收Ready";
                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }

                        break;
                    case LocationTypes.Shelf:
                        string sLoc_Check = cmd.Cmd_Mode == clsConstValue.CmdMode.L2L ? cmd.New_Loc : cmd.Loc;
                        bool bCheckOutside = false; string sLocDD = ""; bool IsEmpty_DD = false; string BoxID_DD = "";
                        int iRet = wms.GetLocMst().CheckLocIsOutside(sLoc_Check, ref bCheckOutside, ref sLocDD, ref IsEmpty_DD, ref BoxID_DD);
                        if (iRet == DBResult.Success)
                        {
                            CmdMstInfo cmd_DD = new CmdMstInfo();
                            iRet = Cmd_Mst.FunCheckHasCommand(sLocDD, ref cmd_DD, db);
                            if (iRet == DBResult.Exception)
                            {
                                sRemark = $"Error: 找尋儲位命令失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{sLocDD}";
                                if (sRemark != cmd.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                }

                                return false;
                            }

                            if (bCheckOutside)
                            {
                                if (!IsEmpty_DD)
                                {
                                    if (iRet == DBResult.Success)
                                    {
                                        sRemark = $"Error: 等候內儲位{sLocDD}命令完成 => " +
                                              $"<{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{cmd_DD.Cmd_Sno}";
                                        if (sRemark != cmd.Remark)
                                        {
                                            Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                        }
                                    }
                                    else
                                    {
                                        lock (_Lock)
                                        {
                                            if (!reportedFlag)
                                            {
                                                reportedFlag = true;
                                                OnNeedShelfToShelfEvent?.Invoke(this, new NeedShelfToShelfArgs(Device.DeviceID, sLocDD, BoxID_DD));
                                                reportedFlag = false;
                                                clsWriLog.Log.FunWriLog(WriLog.clsLog.Type.Debug, $"觸發庫對庫Event => " +
                                                    $"<{Parameter.clsCmd_Mst.Column.Loc}>{sLocDD} <{Parameter.clsCmd_Mst.Column.BoxID}>{BoxID_DD}");
                                            }
                                        }

                                        sRemark = $"通知WMS產生內儲位{sLocDD}的庫對庫命令";
                                        if (sRemark != cmd.Remark)
                                        {
                                            Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                        }
                                    }

                                    return false;
                                }
                            }
                            else
                            {
                                if (!IsEmpty_DD)
                                {
                                    if (iRet == DBResult.Success && cmd_DD.Cmd_Sts == clsConstValue.CmdSts.strCmd_Running)
                                    {
                                        sRemark = $"Error: 等候外儲位{sLocDD}命令完成 => " +
                                             $"<{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{cmd_DD.Cmd_Sno}";
                                        if (sRemark != cmd.Remark)
                                        {
                                            Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                        }

                                        return false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            sRemark = $"Error: 取得WMS儲位資料失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{sLoc_Check}";
                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool CheckDestinationIsOK_NonASRS(CmdMstInfo cmd, Location sLoc_End, MidHost middle, ConveyorInfo buffer, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                switch (sLoc_End.LocationTypes)
                {
                    case LocationTypes.Conveyor:
                    case LocationTypes.EQ:
                    case LocationTypes.IO:
                        if (sLoc_End.DeviceId == ConveyorDef.DeviceID_Tower) return true;

                        if (!middle.CheckIsOutReady(buffer))
                        {
                            sRemark = $"Error: {sLoc_End.LocationId}沒發接收Ready";
                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }

                        break;
                    case LocationTypes.Shelf:
                        return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool CheckDestinationIsOK(CmdMstInfo cmd, Location sLoc_End, MidHost middle, DeviceInfo Device, WMS.Proc.clsHost wms, 
            bool IsDoubleCmd, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                switch (sLoc_End.LocationTypes)
                {
                    case LocationTypes.Conveyor:
                    case LocationTypes.EQ:
                    case LocationTypes.IO:
                        if (!middle.CheckIsOutReady(Device, sLoc_End))
                        {
                            sRemark = $"Error: {sLoc_End.LocationId}沒發接收Ready";
                            if (sRemark != cmd.Remark)
                            {
                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                            }

                            return false;
                        }

                        if (IsDoubleCmd)
                        {
                            var buffer = tool.GetBuffer(Device, sLoc_End);
                            var group = tool.GetGroup(Device, buffer);
                            var lst = group.Where(r => r != buffer);
                            ConveyorInfo buffer_another = new ConveyorInfo();
                            foreach (var c in lst)
                            {
                                buffer_another = c;
                            }

                            if (!middle.CheckIsOutReady(buffer_another))
                            {
                                sRemark = $"Error: {buffer_another.BufferName}沒發接收Ready";
                                if (sRemark != cmd.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                }

                                return false;
                            }
                        }

                        break;
                    case LocationTypes.Shelf:
                        if (!IsDoubleCmd)
                        {
                            string sLoc_Check = cmd.Cmd_Mode == clsConstValue.CmdMode.L2L ? cmd.New_Loc : cmd.Loc;
                            bool bCheckOutside = false; string sLocDD = ""; bool IsEmpty_DD = false; string BoxID_DD = "";
                            int iRet = wms.GetLocMst().CheckLocIsOutside(sLoc_Check, ref bCheckOutside, ref sLocDD, ref IsEmpty_DD, ref BoxID_DD);
                            if (iRet == DBResult.Success)
                            {
                                CmdMstInfo cmd_DD = new CmdMstInfo();
                                iRet = Cmd_Mst.FunCheckHasCommand(sLocDD, ref cmd_DD, db);
                                if (iRet == DBResult.Exception)
                                {
                                    sRemark = $"Error: 找尋儲位命令失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{sLocDD}";
                                    if (sRemark != cmd.Remark)
                                    {
                                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                    }

                                    return false;
                                }

                                if (bCheckOutside)
                                {
                                    if (!IsEmpty_DD)
                                    {
                                        if (iRet == DBResult.Success)
                                        {
                                            sRemark = $"Error: 等候內儲位{sLocDD}命令完成 => " +
                                                  $"<{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{cmd_DD.Cmd_Sno}";
                                            if (sRemark != cmd.Remark)
                                            {
                                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                            }
                                        }
                                        else
                                        {
                                            lock (_Lock)
                                            {
                                                if (!reportedFlag)
                                                {
                                                    reportedFlag = true;
                                                    OnNeedShelfToShelfEvent?.Invoke(this, new NeedShelfToShelfArgs(Device.DeviceID, sLocDD, BoxID_DD));
                                                    reportedFlag = false;
                                                    clsWriLog.Log.FunWriLog(WriLog.clsLog.Type.Debug, $"觸發庫對庫Event => " +
                                                        $"<{Parameter.clsCmd_Mst.Column.Loc}>{sLocDD} <{Parameter.clsCmd_Mst.Column.BoxID}>{BoxID_DD}");
                                                }
                                            }

                                            sRemark = $"通知WMS產生內儲位{sLocDD}的庫對庫命令";
                                            if (sRemark != cmd.Remark)
                                            {
                                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                            }
                                        }

                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!IsEmpty_DD)
                                    {
                                        if (iRet == DBResult.Success && cmd_DD.Cmd_Sts == clsConstValue.CmdSts.strCmd_Running)
                                        {
                                            sRemark = $"Error: 等候外儲位{sLocDD}命令完成 => " +
                                                 $"<{Parameter.clsCmd_Mst.Column.Cmd_Sno}>{cmd_DD.Cmd_Sno}";
                                            if (sRemark != cmd.Remark)
                                            {
                                                Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                            }

                                            return false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sRemark = $"Error: 取得WMS儲位資料失敗 => <{Parameter.clsCmd_Mst.Column.Loc}>{sLoc_Check}";
                                if (sRemark != cmd.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                                }

                                return false;
                            }
                        }

                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public string FunGetNextPortID(string sLoc, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string NextPort = sLoc ;
                string strEM = "";
                string strSql = $"select * from {Parameter.clsRoutdef.TableName} where " +
                    $"{Parameter.clsRoutdef.Column.HostPortID} = '{sLoc}'";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success) 
                    NextPort = Convert.ToString(dtTmp.Rows[0][Parameter.clsRoutdef.Column.NextHostPortID]);
                else
                {
                    NextPort += "_GetNextPortFail.";
                    clsWriLog.Log.FunWriLog(WriLog.clsLog.Type.Error, $"{strSql} => {strEM}");
                }

                return NextPort;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return "";
            }
            finally
            {
                dtTmp = null;
            }

        }
    }
}
