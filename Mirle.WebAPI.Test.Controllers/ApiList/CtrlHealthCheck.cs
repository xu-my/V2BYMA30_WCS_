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

namespace Mirle.WebAPI.Test.Controllers.ApiList
{
    public partial class CtrlHealthCheck : Form
    {
        private WebApiConfig Apiconfig = new WebApiConfig();
        public static WebApiConfig _BoxApi_Config = new WebApiConfig();
        public static WebApiConfig _E04Api_Config = new WebApiConfig();
        public static WebApiConfig _E05Api_Config = new WebApiConfig();
        private V2BYMA30.clsHost api = new V2BYMA30.clsHost();
        public CtrlHealthCheck(WebApiConfig E04Api_config,WebApiConfig E05Api_config, WebApiConfig BoxApi_config)
        {
            _BoxApi_Config = BoxApi_config;
            _E04Api_Config = E04Api_config;
            _E05Api_Config = E05Api_config;
            InitializeComponent();
        }

        private void button_HealthCheck_Click(object sender, EventArgs e)
        {
            bool ctrltype = true;
            switch (comboBox1.SelectedItem)
            {
                case "LIFT4C":
                    Apiconfig = _E04Api_Config;
                    break;
                case "LIFT5C":
                    Apiconfig = _E05Api_Config;
                    break;
                case "B800C":
                    Apiconfig = _BoxApi_Config;
                    break;

                default:
                    ctrltype = false;
                    MessageBox.Show($"未選擇對象controller", "Buffer Roll Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            if(ctrltype)
            {
                HealthCheckInfo info = new HealthCheckInfo
                {
                    jobId = textBox_jobId.Text
                };
                if (!api.GetHealthCheck().FunReport(info, Apiconfig.IP))
                {
                    MessageBox.Show($"失敗, jobId:{info.jobId}.", "Health Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"成功, jobId:{info.jobId}.", "Health Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
