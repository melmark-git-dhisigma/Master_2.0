using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.DirectoryServices;


public class clsAciveDirectory
{
    clsImpersonate objImpersonate = null;
    string Domain = "";

    bool IsImpersonate = false;
    bool IsActiveDirectoryLogined = false;
    static bool IsActiveDirectory = false;


    public clsAciveDirectory()
    {

    }


    public bool IsActiveDirectoryLogin(ArrayList LstServers, string DomainName, string Username, string Password)
    {

        try
        {
            Domain = "LDAP://" + DomainName;

            objImpersonate = new clsImpersonate();

            IsImpersonate = objImpersonate.IsUserImpersonate(LstServers[0].ToString(), DomainName) ? true : objImpersonate.IsUserImpersonate(LstServers[1].ToString(), DomainName) ? true : objImpersonate.IsUserImpersonate(LstServers[2].ToString(), DomainName) ? true : false;

            if (IsImpersonate == true)
            {
                DirectoryEntry entry = new DirectoryEntry(Domain, Username, Password);
                object nativeObject = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "samaccountname=" + Username + "";
                search.PropertiesToLoad.Add("cn");
                SearchResult sr = search.FindOne();
                if (sr != null)
                {
                    IsActiveDirectoryLogined = true;
                }
            }
            else
            {

            }
        }
        catch
        {
            return IsActiveDirectoryLogined;
        }

        return IsActiveDirectoryLogined;
    }



    public static bool IsActiveDirectoryExit(ArrayList LstServers, string DomainName, string Username)
    {
        IsActiveDirectory = false;
        string UserNm = "";
        string Passwd = "";
        string DomName = "";
        clsImpersonate objImp = new clsImpersonate();
        DirectoryEntry entry = null;
        object nativeObject = null;


        try
        {
            for (int i = 0; i < 3; i++)
            {
                objImp.splitLoginValues(LstServers[i].ToString(), out DomName, out UserNm, out Passwd);
                entry = new DirectoryEntry(DomainName, UserNm, Passwd);
                nativeObject = entry.NativeObject;
                if (nativeObject != null) break;
            }

            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = "samaccountname=" + Username + "";
            search.PropertiesToLoad.Add("cn");
            SearchResult sr = search.FindOne();
            if (sr != null)
            {
                IsActiveDirectory = true;
            }

        }
        catch
        {
            return IsActiveDirectory;
        }

        return IsActiveDirectory;
    }
}