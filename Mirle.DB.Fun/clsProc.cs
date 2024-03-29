﻿using Mirle.DataBase;
using Mirle.Structure;
using System;
using System.Collections.Generic;
using Mirle.Def;
using System.Text;
using System.Threading.Tasks;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using System.Linq;

namespace Mirle.DB.Fun
{
    public class clsProc
    {
        private clsCmd_Mst Cmd_Mst = new clsCmd_Mst();
        private clsLocMst LocMst = new clsLocMst();
        private clsLocDtl LocDtl = new clsLocDtl();
        private clsTrnLog TrnLog = new clsTrnLog();
        private clsTool tool = new clsTool();
        private clsMiddleCmd MiddleCmd = new clsMiddleCmd();
        private WebAPI.V2BYMA30.clsHost api = new WebAPI.V2BYMA30.clsHost();
        public bool FunDoubleStorage_DoubleProc(CmdMstInfo[] cmds, MiddleCmd[] middleCmds, string[] NewLocs, WebApiConfig apiConfig, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                
                clsEnum.Shelf_LocationSide side_Before = clsEnum.Shelf_LocationSide.Fail;
                for (int i = 0; i < middleCmds.Length; i++)
                {
                    side_Before = tool.GetSide(middleCmds[i].Destination);
                    if (side_Before == clsEnum.Shelf_LocationSide.Fail) continue;
                    else break;
                }

                if(side_Before == clsEnum.Shelf_LocationSide.Fail)
                {
                    sRemark = "Error: 確認原儲位區域失敗";
                    for (int i = 0; i < cmds.Length; i++)
                    {
                        if (sRemark != cmds[i].Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmds[i].Cmd_Sno, sRemark, db);
                        }
                    }

                    return false;
                }

                clsEnum.Shelf_LocationSide side_New = tool.GetSide(NewLocs[0]);
                if (side_New == clsEnum.Shelf_LocationSide.Fail)
                {
                    sRemark = $"Error: 確認新儲位區域失敗 => <{Parameter.clsCmd_Mst.Column.New_Loc}>{NewLocs[0]}";
                    for (int i = 0; i < cmds.Length; i++)
                    {
                        if (sRemark != cmds[i].Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmds[i].Cmd_Sno, sRemark, db);
                        }
                    }

                    return false;
                }

                CarrierShelfReportInfo[] infos = new CarrierShelfReportInfo[2];
                if (side_Before == side_New)
                { //同一側
                    for (int i = 0; i < infos.Length; i++)
                    {
                        infos[i] = new CarrierShelfReportInfo
                        {
                            carrierId = cmds[i].BoxID,
                            disableLocation = clsConstValue.YesNo.Yes,
                            jobId = cmds[i].JobID,
                            shelfStatus = clsConstValue.LocSts.IN,
                            shelfId = tool.GetShelfLocation(middleCmds[i].Destination) == clsEnum.Shelf.OutSide? NewLocs[0]: NewLocs[1]
                        };
                    }
                }
                else
                {
                    for (int i = 0; i < infos.Length; i++)
                    {
                        infos[i] = new CarrierShelfReportInfo
                        {
                            carrierId = cmds[i].BoxID,
                            disableLocation = clsConstValue.YesNo.Yes,
                            jobId = cmds[i].JobID,
                            shelfStatus = clsConstValue.LocSts.IN,
                            shelfId = tool.GetShelfLocation(middleCmds[i].Destination) == clsEnum.Shelf.InSide ? NewLocs[0] : NewLocs[1]
                        };
                    }
                }

                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    for (int i = 0; i < cmds.Length; i++)
                    {
                        if (sRemark != cmds[i].Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmds[i].Cmd_Sno, sRemark, db);
                        }
                    }

                    return false;
                }

                for (int i = 0; i < infos.Length; i++)
                {
                    var cmd = cmds.Where(r => r.JobID == infos[i].jobId);
                    foreach (var c in cmd)
                    {
                        if (c.Cmd_Mode == clsConstValue.CmdMode.L2L)
                        {
                            if (!Cmd_Mst.FunUpdateNewLocForL2L(c.Cmd_Sno, infos[i].shelfId, middleCmds[i].DeviceID,
                                Location.LocationID.LeftFork.ToString(), db))
                            {
                                db.TransactionCtrl(TransactionTypes.Rollback);
                                sRemark = "Error: 二重格更新新儲位失敗";
                                if (sRemark != c.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(c.Cmd_Sno, sRemark, db);
                                }

                                return false;
                            }
                        }
                        else
                        {
                            if (!Cmd_Mst.FunUpdateLoc(c.Cmd_Sno, infos[i].shelfId, tool.funGetEquNoByLoc(infos[i].shelfId).ToString(),
                                middleCmds[i].DeviceID, Location.LocationID.LeftFork.ToString(), db))
                            {
                                db.TransactionCtrl(TransactionTypes.Rollback);
                                sRemark = "Error: 二重格更新新儲位失敗";
                                if (sRemark != c.Remark)
                                {
                                    Cmd_Mst.FunUpdateRemark(c.Cmd_Sno, sRemark, db);
                                }

                                return false;
                            }
                        }
                    }
                }

                for (int i = 0; i < middleCmds.Length; i++)
                {
                    if (!MiddleCmd.FunInsertHisMiddleCmd(middleCmds[i].CommandID, db))
                    {
                        db.TransactionCtrl(TransactionTypes.Rollback);
                        sRemark = "Error: Insert MiddleCmd_His失敗";
                        if (sRemark != cmds[i].Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmds[i].Cmd_Sno, sRemark, db);
                        }

                        return false;
                    }
                }

                for (int i = 0; i < middleCmds.Length; i++)
                {
                    if (!MiddleCmd.FunDelMiddleCmd(middleCmds[i].CommandID, db))
                    {
                        db.TransactionCtrl(TransactionTypes.Rollback);
                        sRemark = "Error: 刪除MiddleCmd失敗";
                        if (sRemark != cmds[i].Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmds[i].Cmd_Sno, sRemark, db);
                        }

                        return false;
                    }
                }

                for (int i = 0; i < infos.Length; i++)
                {
                    if (!api.GetCarrierShelfReport().FunReport(infos[i], apiConfig.IP))
                    {
                        db.TransactionCtrl(TransactionTypes.Rollback);
                        sRemark = "Error: 二重格上報WES新儲位失敗";
                        if (sRemark != cmds[i].Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmds[i].Cmd_Sno, sRemark, db);
                        }

                        return false;
                    }
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunDoubleStorage_SingleProc(CmdMstInfo cmd, MiddleCmd middleCmd, string sNewLoc, WebApiConfig apiConfig, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                if (string.IsNullOrWhiteSpace(sNewLoc))
                {
                    sRemark = "Error: 二重格找不到新儲位";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                CarrierShelfReportInfo info = new CarrierShelfReportInfo
                {
                    jobId = cmd.JobID,
                    shelfId = sNewLoc,
                    shelfStatus = clsConstValue.LocSts.IN,
                    carrierId = cmd.BoxID,
                    disableLocation = clsConstValue.YesNo.Yes
                };

                int EquNo_New = tool.funGetEquNoByLoc(sNewLoc);
                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (cmd.Cmd_Mode == clsConstValue.CmdMode.L2L)
                {
                    if (!Cmd_Mst.FunUpdateNewLocForL2L(cmd.Cmd_Sno, sNewLoc, middleCmd.DeviceID,
                        Location.LocationID.LeftFork.ToString(), db))
                    {
                        db.TransactionCtrl(TransactionTypes.Rollback);
                        sRemark = "Error: 二重格更新新儲位失敗";
                        if (sRemark != cmd.Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                        }

                        return false;
                    }
                }
                else
                {
                    if (!Cmd_Mst.FunUpdateLoc(cmd.Cmd_Sno, sNewLoc, EquNo_New.ToString(),
                        middleCmd.DeviceID, Location.LocationID.LeftFork.ToString(), db))
                    {
                        db.TransactionCtrl(TransactionTypes.Rollback);
                        sRemark = "Error: 二重格更新新儲位失敗";
                        if (sRemark != cmd.Remark)
                        {
                            Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                        }

                        return false;
                    }
                }

                if (!MiddleCmd.FunInsertHisMiddleCmd(cmd.Cmd_Sno, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: Insert MiddleCmd_His失敗";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!MiddleCmd.FunDelMiddleCmd(cmd.Cmd_Sno, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 刪除MiddleCmd失敗";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!api.GetCarrierShelfReport().FunReport(info, apiConfig.IP))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 二重格上報WES新儲位失敗";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunUpdateCmd_Mode_in(CmdMstInfo cmd, List<LocDtlInfo> locDtls,
           List<TrnLogInfo> trnLogs, List<MoldUseLogInfo> moldUseLogs, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if(sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!Cmd_Mst.FunUpdateCmdSts(cmd.Cmd_Sno, clsConstValue.CmdSts.strCmd_Finished, "過帳完成", db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    return false;
                }

                if (!LocMst.FunUpdLocSts_Mode_In(cmd, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位主檔失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!LocDtl.FunLocDtl_Mode_In(cmd, locDtls, trnLogs, moldUseLogs, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位明細失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunUpdateCmdCnl_Mode_in(CmdMstInfo cmd, List<LocDtlInfo> locDtls, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!Cmd_Mst.FunUpdateCmdSts(cmd.Cmd_Sno, clsConstValue.CmdSts.strCmd_Cancelled, "過帳取消完成", db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    return false;
                }

                if (!LocMst.FunUpdLocStsCnl_Mode_In(cmd, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位主檔失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!LocDtl.FunLocDtlCnl_Mode_In(cmd, locDtls, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位明細失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunUpdateCmd_Mode_Out(CmdMstInfo cmd, List<LocDtlInfo> locDtls,
          List<TrnLogInfo> trnLogs, List<MoldUseLogInfo> moldUseLogs, DataBase.DB db)
        {
            try
            {
                string sRemark = ""; string strLocDD = "";
                int iRet = LocMst.funCheckLocDDSts(cmd.Loc, out strLocDD, db);
                if(iRet != DBResult.Success)
                {
                    sRemark = $"Error: 找尋對照儲位失敗 => {cmd.Loc}";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!Cmd_Mst.FunUpdateCmdSts(cmd.Cmd_Sno, clsConstValue.CmdSts.strCmd_Finished, "過帳完成", db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    return false;
                }

                if (!LocMst.FunUpdLocSts_Mode_Out(cmd, strLocDD, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位主檔失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if(!LocDtl.FunLocDtl_Mode_Out(cmd, locDtls, trnLogs, moldUseLogs, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位明細失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                db.TransactionCtrl(TransactionTypes.Commit);

                string strLocDtl13 = "";
                foreach(var locdtl in locDtls)
                {
                    strLocDtl13 = locdtl.UsedStatus;
                    break;
                }

                LocMst.FunLocSts_S_LocDtl_Null(cmd, strLocDtl13, db);

                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunUpdateCmdCnl_Mode_Out(CmdMstInfo cmd, List<LocDtlInfo> locDtls, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!Cmd_Mst.FunUpdateCmdSts(cmd.Cmd_Sno, clsConstValue.CmdSts.strCmd_Cancelled, "過帳取消完成", db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    return false;
                }

                if (!LocMst.FunUpdLocStsCnl_Mode_Out(cmd, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位主檔失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!LocDtl.FunLocDtlCnl_Mode_Out(cmd, locDtls, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位明細失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunUpdateCmd_Mode_R2R(CmdMstInfo cmd, List<LocDtlInfo> locDtls,
          List<TrnLogInfo> trnLogs, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!Cmd_Mst.FunUpdateCmdSts(cmd.Cmd_Sno, clsConstValue.CmdSts.strCmd_Finished, "過帳完成", db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    return false;
                }

                if (!LocMst.FunUpdLocSts_Mode_R2R(cmd, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位主檔失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!LocDtl.FunLocDtl_Mode_R2R(cmd, locDtls, trnLogs, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位明細失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunUpdateCmdCnl_Mode_R2R(CmdMstInfo cmd, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!Cmd_Mst.FunUpdateCmdSts(cmd.Cmd_Sno, clsConstValue.CmdSts.strCmd_Cancelled, "過帳取消完成", db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    return false;
                }

                if (!LocMst.FunUpdLocStsCnl_Mode_R2R(cmd, db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    sRemark = "Error: 更新儲位主檔失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunUpdateCmd_Mode_S2S(CmdMstInfo cmd, List<TrnLogInfo> tTrn_Log, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!Cmd_Mst.FunUpdateCmdSts(cmd.Cmd_Sno, clsConstValue.CmdSts.strCmd_Finished, "過帳完成", db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    return false;
                }

                foreach (var trnlog in tTrn_Log)
                {
                    if (!TrnLog.FunInsTrnLog(trnlog, db))
                    {
                        db.TransactionCtrl(TransactionTypes.Rollback);
                        return false;
                    }
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunUpdateCmdCnl_Mode_S2S(CmdMstInfo cmd, List<TrnLogInfo> tTrn_Log, DataBase.DB db)
        {
            try
            {
                string sRemark = "";
                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                {
                    sRemark = "Error: Begin失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        Cmd_Mst.FunUpdateRemark(cmd.Cmd_Sno, sRemark, db);
                    }

                    return false;
                }

                if (!Cmd_Mst.FunUpdateCmdSts(cmd.Cmd_Sno, clsConstValue.CmdSts.strCmd_Cancelled, "過帳取消完成", db))
                {
                    db.TransactionCtrl(TransactionTypes.Rollback);
                    return false;
                }

                foreach (var trnlog in tTrn_Log)
                {
                    if (!TrnLog.FunInsTrnLog(trnlog, db))
                    {
                        db.TransactionCtrl(TransactionTypes.Rollback);
                        return false;
                    }
                }

                db.TransactionCtrl(TransactionTypes.Commit);
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionCtrl(TransactionTypes.Rollback);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }
    }
}
