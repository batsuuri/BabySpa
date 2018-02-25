using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using static BabySpa.App;

namespace BabySpa
{
    public class AppConfig
    {
        public static void Init()
        {
            InitAppConfig();
            InitDic();
            //InitDefaultMenu();
        }

        private static void InitAppConfig()
        {

            //App.tourList = new SortedList<string, Tour>();
            //App.HomeTours = new SortedList<string, Tour>();
            //App.dataBasic = new SortedList<string, DataBasic>();
        }
        public static void InitDic()
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            Dictionary dic;
            TimeTable tt;
            SysConfig config;
            sysMsg = new System.Collections.Hashtable();
            string sqlData = @"SELECT *  FROM [dbo].[time_table] order by branch_id, day_no
                            SELECT * FROM sys_config ORDER BY config_key;";


            Result res = DataSetExecute(sqlData, 1);

            try
            {
                if (res.Succeed)
                {
                    if (App.timeTable != null && App.timeTable.Count > 0)
                    {
                        App.timeTable.Clear();
                    }
                    ds = (DataSet)res.Data;
                    dt = ds.Tables[0];


                    foreach (DataRow row in dt.Rows)
                    {
                        
                            tt = new TimeTable
                            {
                                branch_id = Func.ToStr(row["branch_id"]),
                                day_no = Func.ToStr(row["day_no"]),
                                start_time = Func.ToStr(row["start_hour"]),
                                end_time = Func.ToStr(row["end_hour"]),
                                is_work = Func.ToStr(row["is_work"])
                            };
                        timeTable.Add(tt.branch_id+"_"+tt.day_no, tt);
                    }

                    
                    App.sysConfig = new System.Collections.Hashtable();
                    dt = ds.Tables[1];
                    foreach (DataRow row in dt.Rows)
                    {
                        config = new SysConfig
                        {
                            config_key = Func.ToStr(row["config_Key"]),
                            config_value = Func.ToStr(row["config_Value"]),
                            config_type = Func.ToStr(row["config_type"]),
                            config_value2 = Func.ToStr(row["config_value2"])
                        };
                        sysConfig.Add(config.config_key, config);
                    }

                    dt.Dispose();
                    dt = null;
                    ds.Dispose();
                    ds = null;
                    dic = null;
                    config = null;
                    tt = null;
                }
                else
                {
                    Main.ErrorLog("AppConfig-DicInit", res.Desc);
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("AppConfig-DicInit", ex);
            }
        }
    }
}