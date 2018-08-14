using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BabySpa.Models
{
    public class Globalization
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string KeyField { get; set; }
        public string Lang { get; set; }
        public string TextValue { get; set; }

        public Result res { get; set;}


        public Result Insert() {
            res = new Result(true);
            SqlParameter pTextValue = new SqlParameter("@textvalue", SqlDbType.NVarChar);

            string sql = @"Insert into Data_Globalization(TableName, ColumnName,KeyField,Lang, TextValue) 
                values("+ this.TableName+","+
                this.ColumnName+","+
                this.KeyField+","+
                this.Lang+ 
                ",@textvalue)";
            
            res = Main.ExecuteNonQuery(sql, new []{pTextValue});
            return res;
        }

        public Result Update()
        {
            res = new Result(true);
            SqlParameter pTextValue = new SqlParameter("@textvalue", SqlDbType.NVarChar);

            string sql = @"update Data_Globalization set textvalue = @textvalue
                where tablename =" + this.TableName + " and " +
                    "columnName = "+this.ColumnName + " and " +
                    "KeyField = " + this.KeyField + " and " +
                    "Lang = " + this.Lang;

            res = Main.ExecuteNonQuery(sql, new[] { pTextValue });
            return res;
        }

        public Result Delete()
        {
            res = new Result(true);

            string sql = @"delete from Data_Globalization 
                where tablename =" + this.TableName + " and " +
                    "columnName = " + this.ColumnName + " and " +
                    "KeyField = " + this.KeyField + " and " +
                    "Lang = " + this.Lang;

            res = Main.ExecuteNonQuery(sql);
            return res;
        }
    }
}