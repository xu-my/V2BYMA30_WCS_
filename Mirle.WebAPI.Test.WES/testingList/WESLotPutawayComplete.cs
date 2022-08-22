﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Mirle.Def;

namespace Mirle.WebAPI.Test.WES.testingList
{
    public partial class WESLotPutawayComplete : Form
    {
        private WebApiConfig wesApiconfig = new WebApiConfig();
        private WebAPI.V2BYMA30.clsHost api = new WebAPI.V2BYMA30.clsHost();
        public WESLotPutawayComplete(WebApiConfig WESAPIconfig)
        {
            InitializeComponent();
            wesApiconfig = WESAPIconfig;
        }

        private void label_lotId_Click(object sender, EventArgs e)
        {

        }

        private void textBox_lotId_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_LotPutawayComplete_Click(object sender, EventArgs e)
        {
            LotPutawayCompleteInfo info = new LotPutawayCompleteInfo
            {
                jobId = textBox_jobId.Text,
                lotId = textBox_lotId.Text,
                shelfId = textBox_shelfId.Text,
                isComplete = textBox_isComplete.Text
            };
            if (!api.GetLotPutawayComplete().FunReport(info, wesApiconfig.IP))
            {
                MessageBox.Show($"失敗, jobId:{info.jobId}.", "Lot PutAway Complete", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show($"成功, jobId:{info.jobId}.", "Lot PutAway Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
