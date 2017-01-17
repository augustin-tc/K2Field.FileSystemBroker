using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.FileSystemBroker
{
   public class ACLHelper
    {
        DataTable dtResults;

        public ACLHelper(DataTable dt)
        {
            dtResults = dt;
        }

        //public GetFolderAcl(string fullPath)
        //{
        //    DirectoryInfo dirInfo = new DirectoryInfo(fullPath);

        //    foreach( in dirInfo.GetAccessControl().GetAccessRules())
        //}


        public DataTable SetFolderAcl(string fullPath, string login, string permission)
        {

            string res = "true";
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(fullPath);


                System.Security.AccessControl.FileSystemRights rights = System.Security.AccessControl.FileSystemRights.Read;
                if (permission.ToLower() == "modify")
                    rights = System.Security.AccessControl.FileSystemRights.Write;
                else if (permission.ToLower() == "fullcontrol")
                    rights = System.Security.AccessControl.FileSystemRights.FullControl;


                System.Security.AccessControl.FileSystemAccessRule rule = new System.Security.AccessControl.FileSystemAccessRule(login, 
                    rights,
                    InheritanceFlags.ContainerInherit,
                    PropagationFlags.None, 
                    AccessControlType.Allow);
                
                DirectorySecurity dirSec = dirInfo.GetAccessControl();
                
                dirSec.SetAccessRule(rule);
                dirInfo.SetAccessControl(dirSec);
            }
            catch (Exception e)
            {
                res = e.Message;

            }

            DataRow row = dtResults.NewRow();
            row["Result"] = res;

            dtResults.Rows.Add(row);
            return dtResults;
        }

       //public DataTable GetFolderAcl(string fullPath)
       // {
       //     string res = "true";
       //     try
       //     {
       //         DirectoryInfo dirInfo = new DirectoryInfo(fullPath);



       //         DirectorySecurity dirSec = dirInfo.GetAccessControl();

       //         var acls = dirSec.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
       //         dirSec.get

       //         foreach(AuthorizationRule rule in   acls)
       //         {
       //             rule.
       //         }


       //         dirSec.SetAccessRule(rule);
       //         dirInfo.SetAccessControl(dirSec);
       //     }
       //     catch (Exception e)
       //     {
       //         res = e.Message;

       //     }

       //     DataRow row = dtResults.NewRow();
       //     row["Result"] = res;

       //     dtResults.Rows.Add(row);
       //     return dtResults;
       // }

    }
}
