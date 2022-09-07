﻿using System;
using Mirle.Def;
using System.Windows.Forms;
using Mirle.Structure;
using Mirle.Structure.Info;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Mirle.DB.Object
{
    public class clsAPI
    {
        private static WebAPI.V2BYMA30.clsHost api;
        private static WebApiConfig wesApiconfig;
        private static WebApiConfig agvcApiconfig;
        private static WebApiConfig towerApiconfig;
        public static void Initial(WebApiConfig WesApiConfig, WebApiConfig AgvcApiConfig, WebApiConfig TowerApiConfig)
        {
            api = new WebAPI.V2BYMA30.clsHost();
            wesApiconfig = WesApiConfig;
            agvcApiconfig = AgvcApiConfig;
            towerApiconfig = TowerApiConfig;
        }

        public static WebAPI.V2BYMA30.clsHost GetAPI() => api;
        public static WebApiConfig GetWesApiConfig() => wesApiconfig;
        public static WebApiConfig GetAgvcApiConfig() => agvcApiconfig;
        public static WebApiConfig GetTowerApiConfig() => towerApiconfig;
    }
}