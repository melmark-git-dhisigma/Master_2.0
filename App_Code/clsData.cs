using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for clsData
/// </summary>
public class clsData
{
    public clsData()
    {

    }

    private static SqlConnection mCon = null;
    private string mConectionString = "";
    SqlCommand cmd = null;
    SqlDataAdapter DAdap = null;
    public SqlTransaction Trans = null;
    public static bool blnTrans = true;

    public void Reset()
    {
        cmd = null;
        DAdap = null;
    }

    public byte[] FetchPhoto(string SQL)
    {
        byte[] content;
        using (cmd = new SqlCommand())
        {
            if (blnTrans) cmd.Transaction = Trans;
            cmd.CommandText = SQL;
            cmd.Connection = mCon;
            content = (byte[])cmd.ExecuteScalar();
        }
        return content;
    }

    public void ExecuteSp()
    {
        using (cmd = new SqlCommand())
        {
            if (blnTrans) cmd.Transaction = Trans;
            cmd.Connection = mCon;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SessionScore_Calculation";
            Open();
            object content = cmd.ExecuteScalar();
        }
    }

    public object ExecuteIOAPercCalculation(int NormalTable, int IOATable)
    {
        using (cmd = new SqlCommand())
        {
            if (blnTrans) cmd.Transaction = Trans;
            cmd.Connection = mCon;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@NormalSessHdr", NormalTable);
            cmd.Parameters.AddWithValue("@IOASessHdr", IOATable);
            cmd.CommandText = "IOAPercentage_Calculation";
            Open();
            object content = cmd.ExecuteScalar();
            return content;
        }
    }

    public void ExecutePhoto(byte[] content, int Id, bool val)
    {
        using (cmd = new SqlCommand())
        {
            if (blnTrans) cmd.Transaction = Trans;
            try
            {
                cmd.Connection = mCon;
                cmd.CommandType = CommandType.StoredProcedure;

                if (val == true)
                {
                    cmd.CommandText = "saveImage";
                }
                else
                {
                    cmd.CommandText = "updateImage";
                }
                cmd.Parameters.Add("@SMS_AdmReg_ID", SqlDbType.Int);
                cmd.Parameters.Add("@Photo", SqlDbType.Image);
                cmd.Parameters["@SMS_AdmReg_ID"].Value = Id;
                cmd.Parameters["@Photo"].Value = content;
                cmd.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Cannot insert duplicate"))
                    throw new Exception("Duplicate");
                ClsErrorLog errlog = new ClsErrorLog();
                errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + ex.ToString());
            }
        }
    }

    public void Dispose()
    {
        mCon = null;
        mConectionString = "";
        cmd = null;
        DAdap = null;
        Trans = null;
        blnTrans = false;
    }

    public string ConnectionString
    {
        get
        {
            return mConectionString = ConfigurationManager.ConnectionStrings["dbConnectionString"].ToString();
        }
    }
    //Get One Value From Table........
    public object FetchValue(string SQL)
    {
        object x = null;
        try
        {
            Open();
            cmd = null;
            using (cmd = new SqlCommand())
            {
                cmd.CommandText = SQL;
                cmd.Connection = mCon;

                x = cmd.ExecuteScalar();
            }
            Close();
        }
        catch (Exception exp)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog(exp.ToString());
        }

        return x;
    }
    public object FetchValueTrans(string SQL, SqlTransaction Transs, SqlConnection Con)
    {
        object x = null;
        try
        {

            cmd = null;
            using (cmd = new SqlCommand())
            {
                cmd.Transaction = Transs;
                cmd.CommandText = SQL;
                cmd.Connection = Con;

                x = cmd.ExecuteScalar();
            }

        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return x;
    }

    public DataTable ReturnDataTableWithTransaction(string Query, SqlTransaction Trans, bool sql)
    {
        DataTable Dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand(Query, mCon);
            cmd.Transaction = Trans;
            DAdap = new SqlDataAdapter(cmd);
            DAdap.Fill(Dt);
            cmd = null;

        }
        catch (Exception Ex)
        {
            Dt = null;
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return Dt;
    }
    public object GetCurrentAutoIncID()
    {
        object x = null;
        try
        {
            Open();
            using (SqlCommand cmd = new SqlCommand("SELECT SCOPE_IDENTITY()", mCon))
            {
                if (blnTrans) cmd.Transaction = Trans;
                x = cmd.ExecuteScalar();
            }
            Close();
        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return x;
    }

    public bool IFExists(string SQL)
    {
        bool returnvalue = false;
        try
        {
            Open();
            using (cmd = new SqlCommand(SQL, mCon))
            {
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd != null) if (rd.Read()) returnvalue = true;
                    rd.Close();
                }
            }
            Close();
        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return returnvalue;

    }



    public bool IFExistsWithTranss(string SQL, SqlTransaction Trans, SqlConnection mCon)
    {
        bool returnvalue = false;
        try
        {

            using (cmd = new SqlCommand(SQL, mCon))
            {
                cmd.Transaction = Trans;
                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read()) returnvalue = true;
                        rd.Close();
                    }
                }
                catch (SqlException ex)
                {
                    RollBackTransation(Trans);
                    //Close();
                    ClsErrorLog errlog = new ClsErrorLog();
                    errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + ex.ToString());
                }

                //  Close();

            }
        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }

        return returnvalue;


    }

    public int Save(ref DataTable dtAdd)
    {
        int returnValue = 0;
        SqlCommand cmd = null;
        DataTable Dt = new DataTable();

        try
        {
            using (cmd = new SqlCommand("SELECT * FROM " + dtAdd.TableName + " WHERE 1=2", mCon))
            {
                if (blnTrans) cmd.Transaction = Trans;
                using (DAdap = new SqlDataAdapter(cmd))
                {
                    DAdap.Fill(Dt);
                    DAdap.FillSchema(Dt, SchemaType.Source);

                    DataRow Dr;

                    foreach (DataRow D in dtAdd.Rows)
                    {
                        Dr = Dt.NewRow();
                        Dr.ItemArray = D.ItemArray;
                        Dt.Rows.Add(Dr);
                    }

                    Dt.GetChanges();

                    SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(DAdap);
                    DAdap.InsertCommand = cmdBuilder.GetInsertCommand();
                    returnValue = DAdap.Update(Dt);
                }
            }
        }
        catch (SqlException Ex)
        {

            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());

        }
        finally
        {
        }

        return returnValue;

    }




    public int Update(ref DataTable dtUpd, DataColumn[] primaryKey, string WhereCondition)
    {
        int returnValue = 0;
        DataTable dt = new DataTable();
        using (cmd = new SqlCommand("SELECT * FROM " + dtUpd.TableName + " WHERE " + WhereCondition, mCon))
        {
            if (blnTrans) cmd.Transaction = Trans;
            using (DAdap = new SqlDataAdapter(cmd))
            {
                string pkey = "";
                foreach (DataColumn C in primaryKey)
                {
                    pkey += C.ColumnName + " ";
                }


                DAdap.Fill(dt);
                DAdap.FillSchema(dt, SchemaType.Source);
                DataRow Dr;

                foreach (DataRow D in dtUpd.Rows)
                {
                    Dr = dt.NewRow();
                    foreach (DataColumn C in dtUpd.Columns)
                    {
                        if (pkey.Contains(C.ColumnName)) continue;
                        Dr[C.ColumnName] = D[C.ColumnName];
                    }
                    dt.Rows.Add(Dr);
                }

                dt.GetChanges();
                dt.PrimaryKey = primaryKey;

                using (SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(DAdap))
                {
                    DAdap.UpdateCommand = cmdBuilder.GetUpdateCommand();
                }

                returnValue = DAdap.Update(dt);
            }
        }
        return returnValue;
    }


    public SqlConnection Open()
    {
        try
        {
            if (mCon == null) mCon = new SqlConnection(ConnectionString);
            if (mCon.State != ConnectionState.Open)
            {
                mCon.Close();
                mCon.Open();
            }
        }
        catch (SqlException eX)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + eX.ToString());
        }
        return mCon;
    }

    public void Open(bool BeginTrans)
    {
        try
        {
            if (mCon == null) mCon = new SqlConnection(ConnectionString);
            if (mCon.State != ConnectionState.Open)
            {
                mCon.Close();
                mCon.Open();
                blnTrans = BeginTrans;
                if (BeginTrans) Trans = mCon.BeginTransaction();
            }
        }
        catch (SqlException eX)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + eX.ToString());
        }
    }

    public void Close()
    {
        try
        {
            if (mCon.State == ConnectionState.Open)
            {
                mCon.Close();
                cmd = null;
                DAdap = null;

            }
            //blnTrans = false;
        }
        catch (SqlException eX)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + eX.ToString());
        }
    }
    public void CommitTransation()
    {
        if (blnTrans) Trans.Commit();
        blnTrans = false;
        Close();
    }
    public void CommitTransation(SqlTransaction Transs)
    {
        if (Transs != null)
        {
            Transs.Commit();
            blnTrans = false;
            Close();
        }
    }
    public void RollBackTransation()
    {
        Trans.Rollback();
        blnTrans = false;
        Close();
    }
    public void RollBackTransation(SqlTransaction Transs)
    {
        Transs.Rollback();
        blnTrans = false;
        Close();
    }


    //Use this For Insertin ,Update and Deleting.......

    public int ExecuteWithScopeandConnection(string sql, SqlConnection con, SqlTransaction Transs)
    {
        int retval = 0;

        try
        {
            sql = sql + "\nSELECT SCOPE_IDENTITY()";
            using (cmd = new SqlCommand(sql, con))
            {
                cmd.Transaction = Transs;
                try
                {
                    cmd.Connection = con;
                    retval = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (SqlException ex)
                {
                    RollBackTransation(Transs);
                    //Close();
                    ClsErrorLog errlog = new ClsErrorLog();
                    errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + ex.ToString());
                }

                //  Close();

            }
        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return retval;
    }

    public int ExecuteWithScope(string sql)
    {
        int retval = 0;

        try
        {
            Open();
            sql = sql + "\nSELECT SCOPE_IDENTITY()";
            using (cmd = new SqlCommand(sql, mCon))
            {              
                try
                {
                    cmd.Connection = mCon;
                    retval = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (SqlException ex)
                {
                    RollBackTransation(Trans);
                    Close();
                    ClsErrorLog errlog = new ClsErrorLog();
                    errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + ex.ToString());
                }

                Close();

            }
        }
        catch (Exception Ex)
        {
            retval = -1;
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return retval;
    }
    public int ExecuteWithTrans(string sql, SqlConnection con, SqlTransaction Transs)
    {
        int retval = 0;
        using (cmd = new SqlCommand(sql, con))
        {
            if (blnTrans) cmd.Transaction = Transs;
            try
            {
                Open();
                cmd.Connection = con;
                retval = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                RollBackTransation(Transs);
                Close();
                ClsErrorLog errlog = new ClsErrorLog();
                errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + ex.ToString());
                return retval;
               
            }
            return retval;
        }
    }
    public int Execute(string sql)
    {
        int retval = 0;
        using (cmd = new SqlCommand(sql, mCon))
        {
            // if (blnTrans) cmd.Transaction = Trans;
            try
            {
                Open();
                cmd.Connection = mCon;
                retval = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Close();
                ClsErrorLog errlog = new ClsErrorLog();
                errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + ex.ToString());
                return retval;
                
            }
            Close();
            return retval;
        }
    }

    public DataTable ReturnDataTable(string TableName)
    {
        DataTable Dt;
        using (cmd = new SqlCommand("SELECT * FROM " + TableName, mCon))
        {
            if (blnTrans) cmd.Transaction = Trans;
            using (DAdap = new SqlDataAdapter(cmd))
            {
                Dt = new DataTable();
                DAdap.Fill(Dt);
            }
        }
        return Dt;
    }

    public SqlDataReader ReturnDataReader(string Query, bool sql)
    {
        Open();
        SqlDataReader dr = null;
        if (cmd != null) cmd.Dispose();
        cmd = new SqlCommand(Query, mCon);

        if (dr != null) if (dr.Read()) dr.Close();
        if (blnTrans) cmd.Transaction = Trans;
        dr = cmd.ExecuteReader();
        cmd = null;

        return dr;
    }

    public DataTable ReturnDataTable(string Query, bool sql)
    {
        DataTable Dt = new DataTable();
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            Da.Fill(Dt);
            cmd = null;
            Da = null;
            Close();
        }
        catch (Exception Ex)
        {
            Dt = null;
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return Dt;
    }
    public DataTable ReturnDataTable(string Query, SqlTransaction Trans, bool sql)
    {
        DataTable Dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand(Query, mCon);
            cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            Da.Fill(Dt);
            cmd = null;
            Da = null;
        }
        catch (Exception Ex)
        {
            Dt = null;
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return Dt;
    }
    public DataTable ReturnDataTable(string Query, bool sql, bool GetSchema)
    {
        DataTable Dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);

            Da.Fill(Dt);
            if (GetSchema)
            {
                try
                {
                    Da.FillSchema(Dt, SchemaType.Source);
                }
                catch (Exception e)
                {
                    if (e.Message == "")
                    {
                        ClsErrorLog errlog = new ClsErrorLog();
                        errlog.WriteToLog(e.ToString());
                    }
                    Dt = null;

                }
            }
            cmd = null;
            Da = null;
        }
        catch (Exception Ex)
        {
            Dt = null; ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return Dt;
    }

    public DataTable ReturnDataTableForDropDown(string Query, bool sql)
    {
        DataTable Dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);

            Da.Fill(Dt);
            //Da.FillSchema(Dt, SchemaType.Source);
            cmd = null;
            Da = null;
            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    dr[0] = "(Select)";
                }

                else if (Dt.Columns.Count == 2)
                {
                    dr[0] = "";
                    dr[1] = "(Select)";
                }
                Dt.Rows.InsertAt(dr, 0);
            }
        }
        catch (Exception Ex)
        {
            Dt = null;
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        return Dt;
    }

    public void ReturnDataTableForRadioList(string Query, RadioButtonList rdo)
    {
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            //Da.FillSchema(Dt, SchemaType.Source);
            cmd = null;
            Da = null;



            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    rdo.DataSource = Dt;
                    rdo.DataTextField = "Name";
                    rdo.DataBind();
                }

                else if (Dt.Columns.Count == 2)
                {
                    rdo.DataSource = Dt;
                    rdo.DataValueField = "Id";
                    rdo.DataTextField = "Name";
                    rdo.DataBind();
                }

            }

        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        Close();

    }

    public void ReturnDropDown_LessonPlan(string Query, DropDownList Lst)
    {
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            //Da.FillSchema(Dt, SchemaType.Source);
            cmd = null;
            Da = null;
            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();

                if (Dt.Columns.Count == 2)
                {
                    dr[0] = 0;
                    dr[1] = "----------All Lesson Plan----------";
                }
                Dt.Rows.InsertAt(dr, 0);
            }



            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    Lst.DataSource = Dt;
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }

                else if (Dt.Columns.Count == 2)
                {
                    Lst.DataSource = Dt;
                    Lst.DataValueField = "Id";
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }
                Dt.Rows.InsertAt(dr, 0);
            }
        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        Close();

    }

    public void ReturnDropDown(string Query, DropDownList Lst)
    {
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            //Da.FillSchema(Dt, SchemaType.Source);
            cmd = null;
            Da = null;
            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    dr[0] = "---------------Select--------------";
                }

                else if (Dt.Columns.Count == 2)
                {
                    dr[0] = 0;
                    dr[1] = "---------------Select--------------";
                }
                Dt.Rows.InsertAt(dr, 0);
            }



            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    Lst.DataSource = Dt;
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }

                else if (Dt.Columns.Count == 2)
                {
                    Lst.DataSource = Dt;
                    Lst.DataValueField = "Id";
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }
                Dt.Rows.InsertAt(dr, 0);
            }
        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        Close();

    }


    public void ReturnDropDownForStep(string Query, DropDownList Lst)
    {
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            //Da.FillSchema(Dt, SchemaType.Source);
            cmd = null;
            Da = null;
            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    dr[0] = "----------------------------------ALL----------------------";
                }

                else if (Dt.Columns.Count == 2)
                {
                    dr[0] = 0;
                    dr[1] = "----------------------------------ALL----------------------";
                }
                Dt.Rows.InsertAt(dr, 0);
            }



            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    Lst.DataSource = Dt;
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }

                else if (Dt.Columns.Count == 2)
                {
                    Lst.DataSource = Dt;
                    Lst.DataValueField = "Id";
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }
                Dt.Rows.InsertAt(dr, 0);
            }
        }
        catch { }
        Close();

    }


    public void ReturnDropDownForCriteria(string Query, DropDownList Lst)
    {
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            //Da.FillSchema(Dt, SchemaType.Source);
            cmd = null;
            Da = null;
            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    dr[0] = "---------------Select Column--------------";
                }

                else if (Dt.Columns.Count == 2)
                {
                    dr[0] = 0;
                    dr[1] = "---------------Select Column--------------";
                }
                Dt.Rows.InsertAt(dr, 0);
            }

            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    Lst.DataSource = Dt;
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }

                else if (Dt.Columns.Count == 2)
                {
                    Lst.DataSource = Dt;
                    Lst.DataValueField = "Id";
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }
                Dt.Rows.InsertAt(dr, 0);
            }
        }
        catch { }
        Close();

    }



    public void ReturnDropDownForMeasureCriteria(string Query, DropDownList Lst)
    {
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            //Da.FillSchema(Dt, SchemaType.Source);
            cmd = null;
            Da = null;
            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    dr[0] = "---------------Select Measure--------------";
                }

                else if (Dt.Columns.Count == 2)
                {
                    dr[0] = 0;
                    dr[1] = "---------------Select Measure--------------";
                }
                Dt.Rows.InsertAt(dr, 0);
            }





            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    Lst.DataSource = Dt;
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }

                else if (Dt.Columns.Count == 2)
                {
                    Lst.DataSource = Dt;
                    Lst.DataValueField = "Id";
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }
                Dt.Rows.InsertAt(dr, 0);
            }
        }
        catch { }
        Close();

    }

    public void ReturnCheckBoxList(string Query, CheckBoxList Lst)
    {
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            cmd = null;
            Da = null;


            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    Lst.DataSource = Dt;
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }

                else if (Dt.Columns.Count == 2)
                {
                    Lst.DataSource = Dt;
                    Lst.DataValueField = "Id";
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }
                Dt.Rows.InsertAt(dr, 0);
            }
        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }

        Close();

    }


    public void ReturnListBox(string Query, ListBox Lst)
    {
        try
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query, mCon);
            if (blnTrans) cmd.Transaction = Trans;
            SqlDataAdapter Da = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            cmd = null;
            Da = null;


            if (Dt.Rows.Count > 0)
            {
                DataRow dr = Dt.NewRow();
                if (Dt.Columns.Count == 1)
                {
                    Lst.DataSource = Dt;
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }

                else if (Dt.Columns.Count == 2)
                {
                    Lst.DataSource = Dt;
                    Lst.DataValueField = "Id";
                    Lst.DataTextField = "Name";
                    Lst.DataBind();
                }
                Dt.Rows.InsertAt(dr, 0);
            }

        }
        catch (Exception Ex)
        {
            ClsErrorLog errlog = new ClsErrorLog();
            errlog.WriteToLog("Page Name: " + clsGeneral.getPageName() + "\n" + Ex.ToString());
        }
        Close();

    }


}