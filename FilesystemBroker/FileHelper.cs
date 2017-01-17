using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace K2Field.FileSystemBroker
{
    public class FileHelper
    {
        private System.Data.DataTable dtResults;

        public FileHelper(DataTable dtresults)
        {
            this.dtResults = dtresults;
        }

        private void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
            

        public void CreateFile(string filePath, string fileContent)
        {
            var fileInfo = new FileInfo(filePath);
            CheckDirectory(fileInfo.Directory.FullName);

            using (FileStream rdr = new FileStream(fileInfo.FullName, FileMode.Create))
            {
                var bytes = Convert.FromBase64String(fileContent);
                rdr.Write(bytes, 0, bytes.Length);
            }
        }

        public void UpdateFile(string filePath, string fileContent)
        {
            var fileInfo = new FileInfo(filePath);

            CheckDirectory(fileInfo.Directory.FullName);

            using (FileStream rdr = new FileStream(fileInfo.FullName, FileMode.OpenOrCreate))
            {
                var bytes = Convert.FromBase64String(fileContent);
                rdr.Write(bytes, 0, bytes.Length);
            }
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public DataTable ReadFile(string filePath, DataTable dtResults)
        {
            if (File.Exists(filePath))
            {
                FileInfo finfo = new FileInfo(filePath);

                FileSecurity fs =  File.GetAccessControl(filePath);
                var sid = fs.GetOwner(typeof(SecurityIdentifier));          
                IdentityReference OwnerNtAccount = sid.Translate(typeof(NTAccount));
              

                string base64file = Convert.ToBase64String(File.ReadAllBytes(filePath));
                string filePropertyvalue = string.Format("<file><name>{0}</name><content>{1}</content></file>", finfo.Name, base64file);
                DataRow drResults = dtResults.NewRow();
                drResults["File"] = filePropertyvalue;
                drResults["CreatedBy"] = OwnerNtAccount.Value;
                drResults["CreationDate"] = finfo.CreationTime;
                drResults["LastAccessDate"] = finfo.LastAccessTime;
                drResults["LastWriteDate"] = finfo.LastWriteTime;
                drResults["Length"] = finfo.Length;
                
                dtResults.Rows.Add(drResults);

            }
            return dtResults;
        }

           
    }
}
