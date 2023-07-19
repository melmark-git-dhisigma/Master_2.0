using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Data;
using System.Web.UI.WebControls;

[Serializable]
public class clsSession : ISerializable
{
    public clsSession()
    {

    }
    protected clsSession(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException("info");

        ar = (ArrayList)info.GetValue("ar", typeof(object));
        arBinder = (ArrayList)info.GetValue("arBinder", typeof(object));
        arName = (Hashtable)info.GetValue("arName", typeof(object));
        mArryPostedFile = (HttpPostedFile[])info.GetValue("mArryPostedFile", typeof(object));
        mIsLogin = info.GetBoolean("mIsLogin");
        mSessionID = info.GetString("mSessionID");
        mUserName = info.GetString("mUserName");
        mGender = info.GetString("mGender");
        mGenders = info.GetString("mGenders");
        mLoginId = info.GetInt32("mLoginId");
        mYearId = info.GetInt32("mYearId");
        mSchoolId = info.GetInt32("mSchoolId");
        mStudentId = info.GetInt32("mStudentId");
        mAdmStudentId = info.GetInt32("mAdmStudentId");
        mLoginTime = info.GetString("mLoginTime");
        mRoleId = info.GetInt32("mRoleId");
        mRoleName = info.GetString("mRoleName");
        mSchoolName = info.GetString("mSchoolName");
        mStudentName = info.GetString("mStudentName");
        mPhoto = info.GetString("mPhoto");
        mDob = info.GetString("mDob");
        mGrade = info.GetString("mGrade");
        mIEPId = info.GetInt32("mIEPId");
        mIEPStatus = info.GetInt32("mIEPStatus");
        mMenuId = info.GetInt32("mMenuId");
        mRedirect = info.GetString("mRedirect");
        mClassid = info.GetInt32("mClassid");
        mAdminView = info.GetInt32("mAdminView");

        mRes_Ind = info.GetString("mRes_Ind");
        dtStudList = (DataTable)info.GetValue("dtStudList", typeof(object));
        days4weeks = (SelectedDatesCollection)info.GetValue("days4weeks", typeof(object));
        crrntdate = info.GetString("crrntdate");

    }
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {

        info.AddValue("mIsLogin", mIsLogin);
        info.AddValue("ar", ar);
        info.AddValue("arBinder", arBinder);
        info.AddValue("arName", arName);
        info.AddValue("mSessionID", mSessionID);
        info.AddValue("mArryPostedFile", mArryPostedFile);
        info.AddValue("mUserName", mUserName);
        info.AddValue("mGender", mGender);
        info.AddValue("mGenders", mGenders);
        info.AddValue("mLoginId", mLoginId);
        info.AddValue("mYearId", mYearId);
        info.AddValue("mSchoolId", mSchoolId);
        info.AddValue("mStudentId", mStudentId);
        info.AddValue("mAdmStudentId", mAdmStudentId);
        info.AddValue("mLoginTime", mLoginTime);
        info.AddValue("mRoleId", mRoleId);
        info.AddValue("mRoleName", mRoleName);
        info.AddValue("mSchoolName", mSchoolName);
        info.AddValue("mStudentName", mStudentName);
        info.AddValue("mPhoto", mPhoto);
        info.AddValue("mDob", mDob);
        info.AddValue("mGrade", mGrade);
        info.AddValue("mIEPId", mIEPId);
        info.AddValue("mIEPStatus", mIEPStatus);
        info.AddValue("mMenuId", mMenuId);
        info.AddValue("mRedirect", mRedirect);
        info.AddValue("mClassid", mClassid);
        info.AddValue("mAdminView", mAdminView);
        info.AddValue("mRes_Ind", mRes_Ind);


        info.AddValue("dtStudList", dtStudList);
        info.AddValue("days4weeks", days4weeks);
        info.AddValue("crrntdate", crrntdate);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException("info");

        GetObjectData(info, context);
    }

    private string mRes_Ind = "";

    public string Res_Ind
    {
        get { return mRes_Ind; }
        set { mRes_Ind = value; }
    }

    private bool mIsLogin = false;
    public bool Is = true;

    private ArrayList ar = null;


    public ArrayList perPage
    {
        get
        {
            return ar;
        }
        set
        {
            ar = value;
        }
    }

    private ArrayList arBinder = null;


    public ArrayList perPageBinder
    {
        get
        {
            return arBinder;
        }
        set
        {
            arBinder = value;
        }
    }


    private Hashtable arName = null;


    public Hashtable perPageName
    {
        get
        {
            return arName;
        }
        set
        {
            arName = value;
        }
    }


    public bool IsLogin
    {
        get
        {
            return mIsLogin;
        }
        set
        {
            mIsLogin = value;
        }
    }

    private string mSessionID = "";

    public string SessionID
    {
        get
        {
            return mSessionID;
        }
        set
        {
            mSessionID = value;
        }
    }

    public HttpPostedFile[] mArryPostedFile;
    public HttpPostedFile[] ArryPostedFile
    {
        get
        {
            return mArryPostedFile;
        }
        set
        {
            mArryPostedFile = value;
        }
    }

    private string mUserName = "";

    public string UserName
    {
        get
        {
            return mUserName;
        }
        set
        {
            mUserName = value;
        }
    }

    private string mGender = "";

    public string Gender
    {
        get
        {
            return mGender;
        }
        set
        {
            mGender = value;
        }
    }


    private string mGenders = "";

    public string Genders
    {
        get
        {
            return mGenders;
        }
        set
        {
            mGenders = value;
        }
    }



    private int mLoginId = 0;

    public int LoginId
    {
        get
        {
            return mLoginId;
        }
        set
        {
            mLoginId = value;
        }
    }

    private int mYearId = 0;


    private int mGridPagingSize = 25;

    public int GridPagingSize
    {
        get { return mGridPagingSize; }

    }



    public int YearId
    {
        get
        {
            return mYearId;
        }
        set
        {
            mYearId = value;
        }
    }

    private int mSchoolId = 0;

    public int SchoolId
    {
        get
        {
            return mSchoolId;
        }
        set
        {
            mSchoolId = value;
        }
    }

    private int mStudentId = 0;

    public int StudentId
    {
        get
        {
            return mStudentId;
        }
        set
        {
            mStudentId = value;
        }
    }
    private int mAdmStudentId = 0;

    public int AdmStudentId
    {
        get
        {
            return mAdmStudentId;
        }
        set
        {
            mAdmStudentId = value;
        }
    }


    private string mLoginTime = "";

    public string LoginTime
    {
        get
        {
            return mLoginTime;
        }
        set
        {
            mLoginTime = value;
        }
    }

    private int mRoleId = 0;

    public int RoleId
    {
        get
        {
            return mRoleId;
        }
        set
        {
            mRoleId = value;
        }
    }


    private string mRoleName = "";

    public string RoleName
    {
        get
        {
            return mRoleName;
        }
        set
        {
            mRoleName = value;
        }
    }



    private string mSchoolName = "";

    public string SchoolName
    {
        get
        {
            return mSchoolName;
        }
        set
        {
            mSchoolName = value;
        }
    }


    private string mStudentName = "";

    public string StudentName
    {
        get
        {
            return mStudentName;
        }
        set
        {
            mStudentName = value;
        }
    }


    private string mPhoto = "";

    public string Photo
    {
        get
        {
            return mPhoto;
        }
        set
        {
            mPhoto = value;
        }
    }




    private string mDob = "";

    public string Dob
    {
        get
        {
            return mDob;
        }
        set
        {
            mDob = value;
        }
    }


    private string mGrade = "";

    public string Grade
    {
        get
        {
            return mGrade;
        }
        set
        {
            mGrade = value;
        }


    }


    private int mIEPId = 0;

    public int IEPId
    {
        get
        {
            return mIEPId;
        }
        set
        {
            mIEPId = value;
        }
    }

    private int mIEPStatus = 0;

    public int IEPStatus
    {
        get
        {
            return mIEPStatus;
        }
        set
        {
            mIEPStatus = value;
        }
    }

    private int mMenuId = 0;

    public int CurrentMenuId
    {
        get
        {
            return mMenuId;
        }
        set
        {
            mMenuId = value;
        }
    }


    private string mRedirect = "";

    public string Redirect
    {
        get
        {
            return mRedirect;
        }
        set
        {
            mRedirect = value;
        }
    }
    private int mClassid = 0;
    public int Classid
    {
        get
        {
            return mClassid;
        }
        set
        {
            mClassid = value;
        }
    }


    private int mAdminView = 0;
    public int AdminView
    {
        get
        {
            return mAdminView;
        }
        set
        {
            mAdminView = value;
        }
    }



    private DataTable dtStudList;
    public DataTable dt_StudList
    {
        get { return dtStudList; }
        set { dtStudList = value; }
    }

    private SelectedDatesCollection days4weeks;
    public SelectedDatesCollection days_4weeks
    {
        get { return days4weeks; }
        set { days4weeks = value; }
    }
    private string crrntdate = "";
    public string CrrntDate
    {
        get { return crrntdate; }
        set { crrntdate = value; }
    }


}