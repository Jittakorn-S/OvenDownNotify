using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace OvenDownNotify
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnection conn;
            string connetionString = Properties.Settings.Default.ConnectionDB;
            conn = new SqlConnection(connetionString);
            conn.Open();
            List<GetDataLot> ListLot = new List<GetDataLot>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT LotNo, MCNo FROM (" +
                    "SELECT [LotNo]" +
                    ",[MCNo]" +
                    ",[LotStartTime]" +
                    ",[OPNo]" +
                    ",TIME AS TimeSTD" +
                    ",DATEDIFF(minute, LotStartTime, getdate()) AS TimeRun" +
                    ",IIF(DATEDIFF(minute, LotStartTime, getdate()) > TIME, 1, 0) AS IsOver" +
                    " FROM [DBx].[dbo].[SPOvenData]" +
                    " INNER JOIN APCSProDB.trans.lots ON [SPOvenData].LotNo = lots.lot_no" +
                    " INNER JOIN APCSProDB.method.packages ON lots.act_package_id = packages.id" +
                    " INNER JOIN [DBx].[dbo].[OvenRecipe] ON packages.name = OvenRecipe.Package" +
                    " AND [OvenRecipe].Process = 'SP'" +
                    " LEFT JOIN [APCSProDB].[trans].[lot_process_records] ON [lots].id = [lot_process_records].lot_id" +
                    " AND record_class = 2" +
                    " AND [lot_process_records].job_id IN (197 ,291 ,199 ,293)" +
                    " WHERE [SPOvenData].LotEndTime IS NULL" +
                    " AND lots.wip_state = 20" +
                    " AND record_class IS NULL ) AS GetOverLot" +
                    " WHERE IsOver = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GetDataLot DataLot = new GetDataLot();

                        if (!(reader["LotNo"] is DBNull)) DataLot.LotNo = reader["LotNo"].ToString();
                        if (!(reader["MCNo"] is DBNull)) DataLot.MCNo = reader["MCNo"].ToString();
                        ListLot.Add(DataLot);
                    }
                    cmd.Dispose();
                    conn.Close();
                }
            }
            string ResultLot = "";
            foreach (GetDataLot DataLot in ListLot)
            {
                ResultLot += "Lot no. " + DataLot.LotNo + " ที่เครื่อง " + DataLot.MCNo + "\n";
            }
            if (ResultLot != "")
            {
                LineNotify("กรุณา End Lot ตามรายละเอียดดังนี้ : \n" + ResultLot);
                Application.Exit();
            }
            else
            {
                Application.Exit();
            }
        }
        private void LineNotify(string msg)
        {
            string token = ConfigurationManager.AppSettings.Get("Token");
            try
            {
                //Initial LINE API
                var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
                var postData = string.Format("message={0}", msg);
                var data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Bearer " + token);
                using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //
            }
            catch (Exception Error)
            {
                //Create Log File
                string LogFilePath = ConfigurationManager.AppSettings.Get("LogFile");
                using (FileStream FileLog = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write))
                using (StreamWriter WriteFile = new StreamWriter(FileLog))
                {
                    WriteFile.WriteLine(DateTime.Now);
                    WriteFile.WriteLine(Error.Message);
                    WriteFile.WriteLine("{0} Exception caught.", Error);
                    WriteFile.WriteLine("-----------------------------------------------------------------");
                }
            }
        }
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
