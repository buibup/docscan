using System;
using System.Collections.Generic;

using System.Web;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;


namespace DocScanEPR
{
    public class clstDataManager
    {

        public DataTable GetDataSQL(string StrWhere)
        {
            DataTable dt = new DataTable();

            using (SqlCommand Command = new SqlCommand())
            {
                try
                {
                    SqlConnection connection = new SqlConnection(Constants.SVH21);
                    Command.CommandText = StrWhere;
                    Command.CommandType = CommandType.Text;
                    Command.Connection = connection;
                    Command.CommandTimeout = 3000;
                    if (connection.State == ConnectionState.Closed)
                    {
                        Command.Connection.Open();
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(Command);

                    adapter.Fill(dt);
                    Command.Connection.Close();

                }
                catch (System.Exception e)
                {
                    Command.Connection.Close();
                }



            }
            return dt;

        }




        public DataTable GetData(string StrWhere)
        {
            DataTable dt = new DataTable();

            using (OdbcCommand Command = new OdbcCommand())
            {
                try
                {
                    OdbcConnection connection = new OdbcConnection(Constants.OCN_MEDSD);
                    Command.CommandText = StrWhere;
                    Command.CommandType = CommandType.Text;
                    Command.Connection = connection;
                    Command.CommandTimeout = 3000;
                    if (connection.State == ConnectionState.Closed)
                    {
                        Command.Connection.Open();
                    }

                    OdbcDataAdapter adapter = new OdbcDataAdapter(Command);

                    adapter.Fill(dt);
                    Command.Connection.Close();

                }
                catch (System.Exception e)
                {
                    Command.Connection.Close();
                }



            }
            return dt;

        }

        public DataTable CallStoredProcedure(string commandtext)
        {
            OdbcConnection con = new OdbcConnection(Constants.OCN_MEDSD);
            OdbcCommand cmd = new OdbcCommand();
            DataTable dt = new DataTable();
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = commandtext;
                cmd.CommandTimeout = 3000;
                dt.Load(cmd.ExecuteReader());
                cmd.Connection.Close();
            }
            catch (Exception e)
            {
                //cmd.Connection.Close();
                //cmd.Dispose();
                // System.Windows.Forms.MessageBox.Show(e.Message.ToString());
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Dispose();
            }
            return dt;
        }

    }
}