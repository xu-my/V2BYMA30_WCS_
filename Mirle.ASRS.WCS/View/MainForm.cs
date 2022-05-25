﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Threading.Tasks;
using Mirle.Grid.U2NMMA30;
using System.Collections.Generic;
using Mirle.Def;
using Mirle.Def.U2NMMA30;
using Mirle.Structure.Info;
using Mirle.DB.Object;
using Mirle.DataBase;
using Mirle.WebAPI.Event.U2NMMA30;
using Unity;
using Mirle.Logger;
using Mirle.WebAPI.U2NMMA30.View;
using Mirle.ASRS.Close.Program;
using System.Threading;
using Mirle.Structure;
using Mirle.WebAPI.U2NMMA30.ReportInfo;
using Mirle.DB.Object.Table;

namespace Mirle.ASRS.WCS.View
{
    public partial class MainForm : Form
    {
        private DB.ClearCmd.Proc.clsHost clearCmd;
        private WebApiHost _webApiHost;
        private UnityContainer _unityContainer;
        private static System.Timers.Timer timRead = new System.Timers.Timer();

        public MainForm()
        {
            InitializeComponent();

            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = false; timRead.Interval = 500;
        }

        #region Event
        private void MainForm_Load(object sender, EventArgs e)
        {
            ChkAppIsAlreadyRunning();
            this.Text = this.Text + "  v " + ProductVersion;
            clInitSys.FunLoadIniSys();
            FunInit();
            GridInit();

            clsWriLog.Log.FunWriTraceLog_CV("WCS程式已開啟");
            timRead.Enabled = true;
            timer1.Enabled = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmCloseProgram objCloseProgram;
            try
            {
                e.Cancel = true;

                objCloseProgram = new frmCloseProgram();

                if (objCloseProgram.ShowDialog() == DialogResult.OK)
                {
                    chkOnline.Checked = false;
                    SpinWait.SpinUntil(() => false, 1000);
                    clsWriLog.Log.FunWriTraceLog_CV("WCS程式已關閉！");
                    throw new Exception();
                }
            }
            catch
            {
                Environment.Exit(0);
            }
            finally
            {
                objCloseProgram = null;
            }
        }

        private MainTestForm mainTest;
        private void button1_Click(object sender, EventArgs e)
        {
            if (mainTest == null)
            {
                mainTest = new MainTestForm(clInitSys.WmsApi_Config);
                mainTest.TopMost = true;
                mainTest.FormClosed += new FormClosedEventHandler(funMainTest_FormClosed);
                mainTest.Show();
            }
            else
            {
                mainTest.BringToFront();
            }
        }

        private void funMainTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mainTest != null)
                mainTest = null;
        }

        private void chkOnline_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOnline.Checked)
                clsWriLog.Log.FunWriTraceLog_CV("WCS OnLine.");
            else
                clsWriLog.Log.FunWriTraceLog_CV("WCS OffLine.");
        }

        private void chkCycleRun_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCycleRun.Checked)
                clsWriLog.Log.FunWriTraceLog_CV("周邊CycleRun啟動！");
            else
                clsWriLog.Log.FunWriTraceLog_CV("周邊CycleRun關閉！");
        }

        private void chkIgnoreTkt_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIgnoreTkt.Checked)
                clsWriLog.Log.FunWriTraceLog_CV("啟動IgnoreTicket功能！");
            else
                clsWriLog.Log.FunWriTraceLog_CV("關閉IgnoreTicket功能！");
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl + L
            if (e.KeyCode == Keys.L && e.Modifiers == Keys.Control)
            {
                Def.clsTool.FunVisbleChange(ref button1);
                Def.clsTool.FunVisbleChange(ref btnTeachMaintain);
                Def.clsTool.FunVisbleChange(ref chkCycleRun);
                Def.clsTool.FunVisbleChange(ref chkIgnoreTkt);
            }
        }
        #endregion Event
        #region Timer
        private void timRead_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            timRead.Enabled = false;
            try
            {
                SubShowCmdtoGrid(ref Grid1);
                if(clsDB_Proc.DBConn)
                {

                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                timRead.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            try
            {
                lblDBConn_WCS.BackColor = clsDB_Proc.DBConn ? Color.Blue : Color.Red;
                lblTimer.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                timer1.Enabled = true;
            }
        } 
        #endregion Timer

        private void FunInit()
        {
            var archive = new AutoArchive();
            archive.Start();

            _unityContainer = new UnityContainer();
            _unityContainer.RegisterInstance(new WCSController());
            _webApiHost = new WebApiHost(new Startup(_unityContainer), clInitSys.WcsApi_Config.IP);
            clearCmd = new DB.ClearCmd.Proc.clsHost();
        }

        #region Grid顯示
        private void GridInit()
        {
            Gird.clInitSys.GridSysInit(ref Grid1);
            ColumnDef.CMD_MST.GridSetLocRange(ref Grid1);
        }

        delegate void degShowCmdtoGrid(ref DataGridView oGrid);
        private void SubShowCmdtoGrid(ref DataGridView oGrid)
        {
            degShowCmdtoGrid obj;
            DataTable dtTmp = new DataTable();
            try
            {
                if (InvokeRequired)
                {
                    obj = new degShowCmdtoGrid(SubShowCmdtoGrid);
                    Invoke(obj, oGrid);
                }
                else
                {
                    int iRet = clsDB_Proc.GetDB_Object().GetCmd_Mst().FunGetCmdMst_Grid(ref dtTmp);
                    if (iRet == DBResult.Success)
                    {
                        if (oGrid.Columns.Count == 0)
                            return;

                        int intSelectRowIndex = (oGrid.SelectedRows.Count == 0 ? -1 : oGrid.SelectedRows[0].Index);
                        oGrid.SuspendLayout();
                        if (oGrid.Rows.Count > dtTmp.Rows.Count)
                        {
                            for (int intRow = oGrid.Rows.Count; intRow > dtTmp.Rows.Count; intRow--)
                                oGrid.Rows.Remove(oGrid.Rows[intRow - 1]);
                        }
                        else if (oGrid.Rows.Count < dtTmp.Rows.Count)
                        {
                            for (int intRow = oGrid.Rows.Count; intRow < dtTmp.Rows.Count; intRow++)
                            {
                                oGrid.Rows.Add();
                                oGrid.Rows[intRow].HeaderCell.Value = (intRow + 1).ToString();
                            }
                        }
                        else
                        {
                            for (int intRow = 0; intRow < oGrid.Rows.Count; intRow++)
                                oGrid.Rows[intRow].HeaderCell.Value = (intRow + 1).ToString();
                        }

                        string strField = string.Empty;
                        string strSortField = string.Empty;
                        SortOrder sortOrder = SortOrder.Ascending;
                        object sync1 = new object();
                        object sync2 = new object();

                        if (oGrid.SortedColumn != null)
                        {
                            strSortField = oGrid.SortedColumn.Name;
                            sortOrder = oGrid.SortOrder;
                            dtTmp.DefaultView.Sort = strSortField + (sortOrder == SortOrder.Ascending ? " ASC" : " DESC");
                            dtTmp = dtTmp.DefaultView.ToTable();
                        }

                        for (int intRow = 0; intRow < oGrid.Rows.Count; intRow++)
                        {
                            for (int intCol = 0; intCol < dtTmp.Columns.Count; intCol++)
                            {
                                //dataGridView.Columns[intCol].HeaderCell.SortGlyphDirection = SortOrder.None;
                                strField = oGrid.Columns[intCol].Name;
                                if (oGrid.Columns.Contains(strField))
                                {
                                    if (Convert.ToString(oGrid.Rows[intRow].Cells[intCol].Value) != Convert.ToString(dtTmp.Rows[intRow][strField]))
                                        oGrid.Rows[intRow].Cells[intCol].Value = Convert.ToString(dtTmp.Rows[intRow][strField]);
                                }
                            }
                        }

                        if (intSelectRowIndex >= 0)
                        {
                            if (oGrid.Rows.Count > intSelectRowIndex)
                                oGrid.Rows[intSelectRowIndex].Selected = true;
                            else
                                oGrid.Rows[oGrid.Rows.Count - 1].Selected = true;
                        }
                        else
                            oGrid.ClearSelection();

                        oGrid.ResumeLayout();
                    }
                    else
                    {
                        oGrid.Rows.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                dtTmp = null;
            }
        }
        #endregion Grid顯示

        /// <summary>
        /// 檢查程式是否重複開啟
        /// </summary>
        private void ChkAppIsAlreadyRunning()
        {
            try
            {
                string aFormName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
                string aProcName = System.IO.Path.GetFileNameWithoutExtension(aFormName);
                if (System.Diagnostics.Process.GetProcessesByName(aProcName).Length > 1)
                {
                    MessageBox.Show("程式已開啟", "Communication System", MessageBoxButtons.OK);
                    //Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                Environment.Exit(0);
            }
        }

        private void ChangeSubForm(Form subForm)
        {
            try
            {
                var children = spcMainView.Panel1.Controls;
                foreach (Control c in children)
                {
                    if (c is Form)
                    {
                        var thisChild = c as Form;
                        //thisChild.Hide();
                        spcMainView.Panel1.Controls.Remove(thisChild);
                        thisChild.Width = 0;
                    }
                }

                if (subForm != null)
                {
                    subForm.TopLevel = false;
                    subForm.Dock = DockStyle.Fill;//適應窗體大小
                    subForm.FormBorderStyle = FormBorderStyle.None;//隱藏右上角的按鈕
                    subForm.Parent = spcMainView.Panel1;
                    spcMainView.Panel1.Controls.Add(subForm);
                    subForm.Show();
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }
    }
}