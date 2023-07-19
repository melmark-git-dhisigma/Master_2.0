using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;




public class clsImpersonate
{
    bool IsImpersonate = false;

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public extern static bool CloseHandle(IntPtr handle);




    public clsImpersonate()
    {
    }
    public void splitLoginValues(string Logins, out string Domain, out string Username, out string Password)
    {
        string[] Values = Logins.Split(',');
        Domain = Values[0].ToString();
        Username = Values[1].ToString();
        Password = Values[2].ToString();
    }

    public bool IsUserImpersonate(string Values,string Domain)
    {
        try
        {
            SafeTokenHandle safeTokenHandle;
            string DomainName = "", UserNm = "", Passwd = "";
            splitLoginValues(Values, out DomainName, out UserNm, out Passwd);


            const int LOGON32_PROVIDER_DEFAULT = 0;
            //This parameter causes LogonUser to create a primary token. 
            const int LOGON32_LOGON_INTERACTIVE = 2;

            // Call LogonUser to obtain a handle to an access token. 
            bool returnValue = LogonUser(UserNm, Domain, Passwd,
                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                out safeTokenHandle);


            if (returnValue == true)
            {
                using (safeTokenHandle)
                {
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {
                            return true;
                        }
                    }
                }

            }
            else
            {
                return false;           
            }


        }
        catch
        {
        }


        return IsImpersonate;

    }




    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }


}