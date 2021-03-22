using System.Data;
using System.IO;

namespace MyTool.CableCount
{
    public class CableInfoManager
    {
        //单例模式
        private CableInfoManager() { }
        private static CableInfoManager instance = new CableInfoManager();
        public static CableInfoManager Instance()
        {
            return instance;
        }

        public DataTable ReadCSV(string filePath)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string strLine = "";
            string[] tableBody = null;
            string[] tableHead = null;
            int columnCount = 0;
            bool IsFirst = true;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    tableBody = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < columnCount; i++)
                    {
                        dr[i] = tableBody[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (tableBody != null && tableBody.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }
            sr.Close();
            fs.Close();
            return dt;
        }
    }
}
