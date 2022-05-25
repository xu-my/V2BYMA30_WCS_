﻿namespace Mirle.ASRS.WCS.View
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tlpMainSts = new System.Windows.Forms.TableLayoutPanel();
            this.lblTimer = new System.Windows.Forms.Label();
            this.picMirle = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDBConn_WMS = new System.Windows.Forms.Label();
            this.chkOnline = new System.Windows.Forms.CheckBox();
            this.lblDBConn_WCS = new System.Windows.Forms.Label();
            this.spcView = new System.Windows.Forms.SplitContainer();
            this.spcMainView = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkIgnoreTkt = new System.Windows.Forms.CheckBox();
            this.btnCrandSpeedMaintain = new System.Windows.Forms.Button();
            this.btnCmdMaintain = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnStockerModeMaintain = new System.Windows.Forms.Button();
            this.btnTeachMaintain = new System.Windows.Forms.Button();
            this.btnTransferToTeachLoc = new System.Windows.Forms.Button();
            this.chkCycleRun = new System.Windows.Forms.CheckBox();
            this.btnOEEParameterMaintain = new System.Windows.Forms.Button();
            this.Grid1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tlpMainSts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMirle)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcView)).BeginInit();
            this.spcView.Panel1.SuspendLayout();
            this.spcView.Panel2.SuspendLayout();
            this.spcView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcMainView)).BeginInit();
            this.spcMainView.Panel2.SuspendLayout();
            this.spcMainView.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tlpMainSts);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.spcView);
            this.splitContainer1.Size = new System.Drawing.Size(1561, 748);
            this.splitContainer1.SplitterDistance = 99;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // tlpMainSts
            // 
            this.tlpMainSts.ColumnCount = 4;
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tlpMainSts.Controls.Add(this.lblTimer, 0, 0);
            this.tlpMainSts.Controls.Add(this.picMirle, 0, 0);
            this.tlpMainSts.Controls.Add(this.tableLayoutPanel2, 3, 0);
            this.tlpMainSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainSts.Location = new System.Drawing.Point(0, 0);
            this.tlpMainSts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tlpMainSts.Name = "tlpMainSts";
            this.tlpMainSts.RowCount = 1;
            this.tlpMainSts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainSts.Size = new System.Drawing.Size(1561, 99);
            this.tlpMainSts.TabIndex = 0;
            // 
            // lblTimer
            // 
            this.lblTimer.BackColor = System.Drawing.SystemColors.Control;
            this.lblTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTimer.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.ForeColor = System.Drawing.Color.Black;
            this.lblTimer.Location = new System.Drawing.Point(222, 0);
            this.lblTimer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(210, 99);
            this.lblTimer.TabIndex = 268;
            this.lblTimer.Text = "yyyy/MM/dd hh:mm:ss";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picMirle
            // 
            this.picMirle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picMirle.Image = ((System.Drawing.Image)(resources.GetObject("picMirle.Image")));
            this.picMirle.Location = new System.Drawing.Point(4, 3);
            this.picMirle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.picMirle.Name = "picMirle";
            this.picMirle.Size = new System.Drawing.Size(210, 93);
            this.picMirle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMirle.TabIndex = 267;
            this.picMirle.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.lblDBConn_WMS, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkOnline, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblDBConn_WCS, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1345, 3);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(212, 93);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lblDBConn_WMS
            // 
            this.lblDBConn_WMS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDBConn_WMS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDBConn_WMS.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDBConn_WMS.Location = new System.Drawing.Point(4, 30);
            this.lblDBConn_WMS.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBConn_WMS.Name = "lblDBConn_WMS";
            this.lblDBConn_WMS.Size = new System.Drawing.Size(204, 30);
            this.lblDBConn_WMS.TabIndex = 3;
            this.lblDBConn_WMS.Text = "WMS DB Sts";
            this.lblDBConn_WMS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkOnline
            // 
            this.chkOnline.AutoSize = true;
            this.chkOnline.Checked = true;
            this.chkOnline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkOnline.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkOnline.Location = new System.Drawing.Point(4, 63);
            this.chkOnline.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkOnline.Name = "chkOnline";
            this.chkOnline.Size = new System.Drawing.Size(204, 27);
            this.chkOnline.TabIndex = 2;
            this.chkOnline.Text = "OnLine";
            this.chkOnline.UseVisualStyleBackColor = true;
            this.chkOnline.Visible = false;
            this.chkOnline.CheckedChanged += new System.EventHandler(this.chkOnline_CheckedChanged);
            // 
            // lblDBConn_WCS
            // 
            this.lblDBConn_WCS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDBConn_WCS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDBConn_WCS.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDBConn_WCS.Location = new System.Drawing.Point(4, 0);
            this.lblDBConn_WCS.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBConn_WCS.Name = "lblDBConn_WCS";
            this.lblDBConn_WCS.Size = new System.Drawing.Size(204, 30);
            this.lblDBConn_WCS.TabIndex = 1;
            this.lblDBConn_WCS.Text = "WCS DB Sts";
            this.lblDBConn_WCS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // spcView
            // 
            this.spcView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcView.Location = new System.Drawing.Point(0, 0);
            this.spcView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.spcView.Name = "spcView";
            this.spcView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcView.Panel1
            // 
            this.spcView.Panel1.Controls.Add(this.spcMainView);
            // 
            // spcView.Panel2
            // 
            this.spcView.Panel2.Controls.Add(this.Grid1);
            this.spcView.Size = new System.Drawing.Size(1561, 644);
            this.spcView.SplitterDistance = 472;
            this.spcView.SplitterWidth = 5;
            this.spcView.TabIndex = 0;
            // 
            // spcMainView
            // 
            this.spcMainView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spcMainView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMainView.Location = new System.Drawing.Point(0, 0);
            this.spcMainView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.spcMainView.Name = "spcMainView";
            // 
            // spcMainView.Panel2
            // 
            this.spcMainView.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.spcMainView.Size = new System.Drawing.Size(1561, 472);
            this.spcMainView.SplitterDistance = 1397;
            this.spcMainView.SplitterWidth = 5;
            this.spcMainView.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chkIgnoreTkt, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.btnCrandSpeedMaintain, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnCmdMaintain, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnStockerModeMaintain, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnTeachMaintain, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnTransferToTeachLoc, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.chkCycleRun, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.btnOEEParameterMaintain, 0, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(157, 470);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // chkIgnoreTkt
            // 
            this.chkIgnoreTkt.AutoSize = true;
            this.chkIgnoreTkt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkIgnoreTkt.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkIgnoreTkt.Location = new System.Drawing.Point(4, 397);
            this.chkIgnoreTkt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkIgnoreTkt.Name = "chkIgnoreTkt";
            this.chkIgnoreTkt.Size = new System.Drawing.Size(149, 31);
            this.chkIgnoreTkt.TabIndex = 10;
            this.chkIgnoreTkt.Text = "Ignore Ticket";
            this.chkIgnoreTkt.UseVisualStyleBackColor = true;
            this.chkIgnoreTkt.Visible = false;
            this.chkIgnoreTkt.CheckedChanged += new System.EventHandler(this.chkIgnoreTkt_CheckedChanged);
            // 
            // btnCrandSpeedMaintain
            // 
            this.btnCrandSpeedMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCrandSpeedMaintain.Location = new System.Drawing.Point(4, 253);
            this.btnCrandSpeedMaintain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCrandSpeedMaintain.Name = "btnCrandSpeedMaintain";
            this.btnCrandSpeedMaintain.Size = new System.Drawing.Size(149, 48);
            this.btnCrandSpeedMaintain.TabIndex = 6;
            this.btnCrandSpeedMaintain.Text = "Stocker Speed Maintain";
            this.btnCrandSpeedMaintain.UseVisualStyleBackColor = true;
            // 
            // btnCmdMaintain
            // 
            this.btnCmdMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCmdMaintain.Location = new System.Drawing.Point(4, 91);
            this.btnCmdMaintain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCmdMaintain.Name = "btnCmdMaintain";
            this.btnCmdMaintain.Size = new System.Drawing.Size(149, 48);
            this.btnCmdMaintain.TabIndex = 4;
            this.btnCmdMaintain.Text = "Command Maintain";
            this.btnCmdMaintain.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(4, 57);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(149, 28);
            this.button1.TabIndex = 3;
            this.button1.Text = "Send API Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnStockerModeMaintain
            // 
            this.btnStockerModeMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStockerModeMaintain.Location = new System.Drawing.Point(4, 3);
            this.btnStockerModeMaintain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnStockerModeMaintain.Name = "btnStockerModeMaintain";
            this.btnStockerModeMaintain.Size = new System.Drawing.Size(149, 48);
            this.btnStockerModeMaintain.TabIndex = 0;
            this.btnStockerModeMaintain.Text = "Stocker Mode Maintain";
            this.btnStockerModeMaintain.UseVisualStyleBackColor = true;
            // 
            // btnTeachMaintain
            // 
            this.btnTeachMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnTeachMaintain.Location = new System.Drawing.Point(4, 145);
            this.btnTeachMaintain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnTeachMaintain.Name = "btnTeachMaintain";
            this.btnTeachMaintain.Size = new System.Drawing.Size(149, 48);
            this.btnTeachMaintain.TabIndex = 5;
            this.btnTeachMaintain.Text = "TeachLoc Maintain";
            this.btnTeachMaintain.UseVisualStyleBackColor = true;
            this.btnTeachMaintain.Visible = false;
            // 
            // btnTransferToTeachLoc
            // 
            this.btnTransferToTeachLoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnTransferToTeachLoc.Location = new System.Drawing.Point(3, 199);
            this.btnTransferToTeachLoc.Name = "btnTransferToTeachLoc";
            this.btnTransferToTeachLoc.Size = new System.Drawing.Size(151, 48);
            this.btnTransferToTeachLoc.TabIndex = 8;
            this.btnTransferToTeachLoc.Text = "Transfer To TeachLoc";
            this.btnTransferToTeachLoc.UseVisualStyleBackColor = true;
            // 
            // chkCycleRun
            // 
            this.chkCycleRun.AutoSize = true;
            this.chkCycleRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkCycleRun.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkCycleRun.Location = new System.Drawing.Point(4, 352);
            this.chkCycleRun.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkCycleRun.Name = "chkCycleRun";
            this.chkCycleRun.Size = new System.Drawing.Size(149, 39);
            this.chkCycleRun.TabIndex = 7;
            this.chkCycleRun.Text = "Cycle Run";
            this.chkCycleRun.UseVisualStyleBackColor = true;
            this.chkCycleRun.Visible = false;
            this.chkCycleRun.CheckedChanged += new System.EventHandler(this.chkCycleRun_CheckedChanged);
            // 
            // btnOEEParameterMaintain
            // 
            this.btnOEEParameterMaintain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOEEParameterMaintain.Location = new System.Drawing.Point(4, 308);
            this.btnOEEParameterMaintain.Margin = new System.Windows.Forms.Padding(4);
            this.btnOEEParameterMaintain.Name = "btnOEEParameterMaintain";
            this.btnOEEParameterMaintain.Size = new System.Drawing.Size(149, 37);
            this.btnOEEParameterMaintain.TabIndex = 9;
            this.btnOEEParameterMaintain.Text = "OEE Parameter";
            this.btnOEEParameterMaintain.UseVisualStyleBackColor = true;
            // 
            // Grid1
            // 
            this.Grid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid1.Location = new System.Drawing.Point(0, 0);
            this.Grid1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Grid1.Name = "Grid1";
            this.Grid1.RowHeadersWidth = 62;
            this.Grid1.RowTemplate.Height = 24;
            this.Grid1.Size = new System.Drawing.Size(1561, 167);
            this.Grid1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1561, 748);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tlpMainSts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMirle)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.spcView.Panel1.ResumeLayout(false);
            this.spcView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcView)).EndInit();
            this.spcView.ResumeLayout(false);
            this.spcMainView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMainView)).EndInit();
            this.spcMainView.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblDBConn_WCS;
        private System.Windows.Forms.SplitContainer spcView;
        private System.Windows.Forms.DataGridView Grid1;
        private System.Windows.Forms.CheckBox chkOnline;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer spcMainView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnStockerModeMaintain;
        private System.Windows.Forms.TableLayoutPanel tlpMainSts;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox picMirle;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnCmdMaintain;
        private System.Windows.Forms.Button btnTeachMaintain;
        private System.Windows.Forms.Button btnCrandSpeedMaintain;
        private System.Windows.Forms.CheckBox chkCycleRun;
        private System.Windows.Forms.Button btnTransferToTeachLoc;
        private System.Windows.Forms.Button btnOEEParameterMaintain;
        private System.Windows.Forms.CheckBox chkIgnoreTkt;
        private System.Windows.Forms.Label lblDBConn_WMS;
    }
}
