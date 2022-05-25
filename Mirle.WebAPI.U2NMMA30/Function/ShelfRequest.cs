﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mirle.WebAPI.U2NMMA30.ReportInfo;
using Mirle.Def;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Mirle.WebAPI.U2NMMA30.Function
{
    public class ShelfRequest
    {
        private WebApiConfig _config = new WebApiConfig();
        public ShelfRequest(WebApiConfig Config)
        {
            _config = Config;
        }

        public bool FunReport(ShelfRequestInfo info)
        {
            try
            {
                string strJson = JsonConvert.SerializeObject(info);
                clsWriLog.Log.FunWriTraceLog_CV(strJson);
                string sLink = $"http://{_config.IP}/SHELF_REQUEST";
                clsWriLog.Log.FunWriTraceLog_CV($"URL: {sLink}");
                string re = clsTool.HttpPost(sLink, strJson);
                clsWriLog.Log.FunWriTraceLog_CV(re);

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }
    }
}