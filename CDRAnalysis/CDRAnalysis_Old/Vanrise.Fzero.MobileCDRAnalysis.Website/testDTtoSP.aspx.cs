using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class testDTtoSP : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (! IsPostBack)
        {
            //To represent the table parameter in C#, we need to either 
            //have a set of entities which are IEnumreable 
            //or a data reader or a Data table.
            //In this example we create a data table with same name as the type we have in the DB 
            DataTable dataTable = new DataTable("SampleDataType");
            //we create column names as per the type in DB 
            dataTable.Columns.Add("SampleString", typeof(string));
            dataTable.Columns.Add("SampleInt", typeof(Int32));
            //and fill in some values 
            dataTable.Rows.Add("99", 99);
            dataTable.Rows.Add("98", null);
            dataTable.Rows.Add("97", 99);



            SqlConnection conn = new SqlConnection("Data Source=192.168.110.185;Initial Catalog=CDRAnalysisMobile_WF;User ID=development;Password=dev!123;");
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SampleProcedure";
            cmd.CommandType=CommandType.StoredProcedure;
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@Sample";
            parameter.SqlDbType = System.Data.SqlDbType.Structured;
            parameter.Value = dataTable;
            parameter.TypeName = "SampleDataType";
            cmd.Parameters.Add(parameter);
            
            cmd.ExecuteNonQuery(); 
            conn.Close();

        }
    }
}