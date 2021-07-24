using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RainWorx.FrameWorx.MVC.App_Start
{
    public class DataAccessLayer
    {
        #region Public
        public DataAccessLayer()
        {

            strConnectionString = Convert.ToString(System.Configuration.ConfigurationManager.ConnectionStrings["db_connection"]);          
        }
       

        #endregion
        #region Private
        private string strConnectionString = "";      
        private SqlConnection objSqlConnection = new SqlConnection();
        private bool ConnectionOpen()
        {
            try
            {
                if (objSqlConnection.State != ConnectionState.Open)
                {
                    objSqlConnection.ConnectionString = strConnectionString;
                    objSqlConnection.Open();
                }               

                return true;
            }
            catch (Exception ex)
            {         
                return false;
            }
        }
        private bool ConnectionClose()
        {
            try
            {
                if (objSqlConnection.State != ConnectionState.Closed)
                {
                    objSqlConnection.Close();
                }
              
                return true;
            }
            catch (Exception ex)
            {               
                return false;
            }
        }
        public int ExecuteSpNonQuery(string SPName)
        {
            try
            {
                if (ConnectionOpen())
                {
                    SqlCommand objSqlCommand = new SqlCommand(SPName, objSqlConnection);
                    objSqlCommand.CommandType = CommandType.StoredProcedure;
                    objSqlCommand.CommandTimeout = 300;
                    int iReturn = objSqlCommand.ExecuteNonQuery();
                    ConnectionClose();
                    return iReturn;
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public int ExecuteSpNonQuery(string SPName, params SqlParameter[] SPParameters)
        {
            try
            {
                if (ConnectionOpen())
                {
                    SqlCommand objSqlCommand = new SqlCommand(SPName, objSqlConnection);
                    objSqlCommand.CommandType = CommandType.StoredProcedure;
                    objSqlCommand.CommandTimeout = 300;
                    for (int i = 0; i < SPParameters.Length; i++)
                    {
                        objSqlCommand.Parameters.Add(SPParameters[i]);
                    }
                    int iReturn = objSqlCommand.ExecuteNonQuery();
                    ConnectionClose();
                    return iReturn;
                }
                return -1;
            }
            catch (Exception ex)
            {             
                return -1;
            }
        }
        public DataTable ExecuteSpDataTable(string SPName)
        {
            try
            {
                if (ConnectionOpen())
                {
                    SqlCommand objSqlCommand = new SqlCommand(SPName, objSqlConnection);
                    objSqlCommand.CommandType = CommandType.StoredProcedure;
                    objSqlCommand.CommandTimeout = 300;
                    SqlDataAdapter objSqlDataAdapter = new SqlDataAdapter();
                    DataTable objDataTable = new DataTable();
                    objSqlDataAdapter.SelectCommand = objSqlCommand;
                    objSqlDataAdapter.Fill(objDataTable);
                    ConnectionClose();
                    return objDataTable;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable ExecuteSpDataTable(string SPName, params SqlParameter[] SPParameters)
        {
            try
            {
                if (ConnectionOpen())
                {
                    SqlCommand objSqlCommand = new SqlCommand(SPName, objSqlConnection);
                    objSqlCommand.CommandType = CommandType.StoredProcedure;
                    objSqlCommand.CommandTimeout = 300;
                    for (int i = 0; i < SPParameters.Length; i++)
                    {
                        objSqlCommand.Parameters.Add(SPParameters[i]);
                    }
                    SqlDataAdapter objSqlDataAdapter = new SqlDataAdapter();
                    DataTable objDataTable = new DataTable();
                    objSqlDataAdapter.SelectCommand = objSqlCommand;
                    objSqlDataAdapter.Fill(objDataTable);
                    ConnectionClose();
                    return objDataTable;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       
        public string GetDatabaseNameFromConnStr()
        {
            try
            {
                string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["db_connection"].ToString();
                if (strConnString.Trim() != "")
                {
                    strConnString = strConnString.Substring(strConnString.IndexOf("Initial Catalog=") + 16);
                    if (strConnString.Trim() != "")
                    {
                        strConnString = strConnString.Substring(0, strConnString.IndexOf(";"));
                        return strConnString;
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
              
                return "";
            }
        }
        public void ExecuteSQL(string SPName)
        {
            try
            {
                if (ConnectionOpen())
                {
                    SqlCommand objSqlCommand = new SqlCommand(SPName, objSqlConnection);
                    objSqlCommand.CommandType = CommandType.Text;
                    objSqlCommand.ExecuteNonQuery();
                    ConnectionClose();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public DataTable GetDataTable(string SPName)
        {
            try
            {
                if (ConnectionOpen())
                {
                    SqlCommand objSqlCommand = new SqlCommand(SPName, objSqlConnection);
                    objSqlCommand.CommandType = CommandType.Text;
                    SqlDataAdapter objSqlDataAdapter = new SqlDataAdapter();
                    DataTable objDataTable = new DataTable();
                    objSqlDataAdapter.SelectCommand = objSqlCommand;
                    objSqlDataAdapter.Fill(objDataTable);
                    ConnectionClose();
                    return objDataTable;
                }
                return null;
            }
            catch (Exception ex)
            {
              
                return null;
            }
        }



        public DataSet GetDataSet(string SPName)
        {
            try
            {
                if (ConnectionOpen())
                {
                    SqlCommand objSqlCommand = new SqlCommand(SPName, objSqlConnection);
                    objSqlCommand.CommandType = CommandType.Text;
                    SqlDataAdapter objSqlDataAdapter = new SqlDataAdapter();
                    DataSet objDataTable = new DataSet();
                    objSqlDataAdapter.SelectCommand = objSqlCommand;
                    objSqlDataAdapter.Fill(objDataTable);
                    ConnectionClose();
                    return objDataTable;
                }
                return null;
            }
            catch (Exception ex)
            {               
                return null;
            }
        }
              
        #endregion
      
    }
}