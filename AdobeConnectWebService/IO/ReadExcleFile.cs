using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;


namespace WorkExcle
{
    public class ReadExcleFile 
    {
        private string _Connectionstring;
       // private OleDbConnection _Conn;

        public ReadExcleFile(string fullPathToExcel)
        {
            _Connectionstring = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=yes'", fullPathToExcel);
           /// OpenConnection();
        }
        //private void OpenConnection()
        //{
        //    _Conn = new OleDbConnection(_Connectionstring);
        //    _Conn.Open();


        //}

        private DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (OleDbConnection _conn = new OleDbConnection(_Connectionstring))
            {
                _conn.Open();
                using OleDbCommand cmd = new OleDbCommand(sql, _conn);
                using OleDbDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
          
            return dt;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPathToExcel">ie C:\Temp\YourExcel.xls</param>
        /// <param name="SheetName">ie Sheet1 </param>
        /// <returns>DataTable</returns>
        public DataTable GetAllCoulumnsFromSheet(string SheetName)
        {
            SheetName = $"[{SheetName}$]";
            DataTable dt = GetDataTable($"Select * from {SheetName}");
            dt.TableName = SheetName;
            return dt;

        }

        /// <summary>
        /// Important: Sheet name Must be End Of With '$'
        /// </summary>
        /// <param name="fullPathToExcel">e C:\Temp\YourExcel.xls</param>
        /// <param name="Query">ie Select id,name form Sheet1$ </param>
        /// <returns></returns>
        public DataTable GetAllCoulumnsFromQuey(string Query)
        {
            DataTable dt = GetDataTable(Query);
            return dt;

        }



        public List<string> GetAllSheetName()
        {
            List<string> sheets = new List<string>();
            DataTable dt;
            using (OleDbConnection _conn = new OleDbConnection(_Connectionstring))
            {
                 dt = _conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            }
            foreach (DataRow dataRow in dt.Rows)
            {
                if (dataRow[2].ToString().EndsWith("$'"))
                {
                    string s = dataRow["TABLE_NAME"].ToString();
                    sheets.Add(s.StartsWith("'") ? s.Substring(1, s.Length - 3) : s.Substring(0, s.Length - 1));
                }
            }
            return sheets;

        }

     
    }
}
