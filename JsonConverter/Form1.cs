using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsonConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String query = "select * from carts_table";  //id,note,status

            SqlCommand cmd = new SqlCommand(query);
            using (SqlConnection con = new SqlConnection("server=LAPTOP-JC1L9RTG\\SQL2014;database=DEEMA;User id = sa;password=sql2014"))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;

                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        int i = 0;
                        //DataTable dtAllInf = new DataTable();
                        DataTable dtAllPart = new DataTable();
                        foreach (DataRow dr in dt.Rows)
                        {
                            String json = dr[1].ToString();
                            //dataGridView1.DataSource = data;
                            //    DataSet myDataSet = JsonConvert.DeserializeObject<DataSet>(json);
                            DataTable dataTable = GetJSONToDataTableUsingMethod(json);
                            //DataTable dataTableInf = GetJSONToDataTableUsingMethodInf(dataTable.Rows[0][1].ToString().Replace(" id: 1", ""));
                            //dataTable.Columns.Remove("id");
                            //dataTable.Columns.Remove("note");
                            //dataTable.Columns.Remove("Status");
                            DataTable particulars = dataTable;

                            //if (dtAllInf.Rows.Count>0)
                            //{
                            //    foreach(DataRow dr1 in dataTableInf.Rows)
                            //    {
                            //        dtAllInf.ImportRow(dr1);
                            //        dataGridView2.DataSource = dtAllInf;
                            //        dataGridView2.Refresh();
                            //        dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.Rows.Count - 1;
                            //    }
                            //} else
                            //{
                            //    dtAllInf = dataTableInf;
                            //    dataGridView2.DataSource = dtAllInf;
                            //}

                            if (dtAllPart.Rows.Count > 0)
                            {
                                foreach (DataRow dr1 in particulars.Rows)
                                {
                                    dtAllPart.ImportRow(dr1);
                                    dataGridView1.DataSource = dtAllPart;
                                    dataGridView1.Refresh();
                                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
                                }
                            }
                            else
                            {
                                dtAllPart = particulars;
                                dataGridView1.DataSource = dtAllPart;
                            }
                        }

                        //dataGridView2.DataSource = dtAllInf;
                        dataGridView1.DataSource = dtAllPart;

                    }
                }
            }
        }

        public static DataTable GetJSONToDataTableUsingMethodInf(string JSONData)
        {
            DataTable dtUsingMethodReturn = new DataTable();
            string[] jsonStringArray = Regex.Split(JSONData.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string strJSONarr in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(strJSONarr.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx).Replace("\"", "").Trim();
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add("Action");
                            ColumnsName.Add("DateTime");
                            ColumnsName.Add("Location");
                            ColumnsName.Add("Id");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dtUsingMethodReturn.Columns.Add(AddColumnName);
            }
            foreach (string strJSONarr in jsonStringArray)
            {
                string[] RowData = Regex.Split(strJSONarr.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dtUsingMethodReturn.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        string[] drRow = Regex.Split(rowData, " ");
                        foreach (string rowData1 in drRow)
                        {
                            if (!rowData1.Contains(":"))
                            {
                                nr["Action"] = rowData1;
                            }
                            else
                            {
                                int idx1 = rowData1.IndexOf(":");
                                string RowColumns1 = rowData1.Substring(0, idx1).Replace("\"", "").Trim();
                                string RowDataString1 = rowData1.Substring(idx1 + 1).Replace("\"", "").Trim();
                                nr[RowColumns1] = RowDataString1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dtUsingMethodReturn.Rows.Add(nr);
            }
            return dtUsingMethodReturn;
        }

        public static DataTable GetJSONToDataTableUsingMethod(string JSONData)
        {
            DataTable dtUsingMethodReturn = new DataTable();
            string[] jsonStringArray = Regex.Split(JSONData.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string strJSONarr in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(strJSONarr.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx).Replace("\"", "").Trim();
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData + ex.ToString()));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dtUsingMethodReturn.Columns.Add(AddColumnName);
            }
            foreach (string strJSONarr in jsonStringArray)
            {
                string[] RowData = Regex.Split(strJSONarr.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dtUsingMethodReturn.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx).Replace("\"", "").Trim();
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "").Trim();
                        nr[RowColumns] = RowDataString;
                        if (RowColumns == "minimumRate")
                        {
                            dtUsingMethodReturn.Rows.Add(nr);
                            nr = dtUsingMethodReturn.NewRow();
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                //dtUsingMethodReturn.Rows.Add(nr);
                //nr = dtUsingMethodReturn.NewRow();
            }
            return dtUsingMethodReturn;
        }
    }
}