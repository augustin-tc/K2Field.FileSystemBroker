using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.FileSystemBroker
{
    class FolderHelper
    {
        private System.Data.DataTable dtResults;

        public FolderHelper(System.Data.DataTable dtResults)
        {
            
            this.dtResults = dtResults;
        }

        public System.Data.DataTable CreateFolder(string fullPath)
        {
            string resultat = "true";
            try
            {
                var directoy = System.IO.Directory.CreateDirectory(fullPath);
            }
            catch (Exception ex)
            {
                resultat = ex.Message;
            }

            DataRow dr = dtResults.NewRow();
            dr["Result"] = resultat;
            dtResults.Rows.Add(dr);
            return dtResults;
        }

        public DataTable DeleteFolder(string fullPath)
        {
            string resultat = "true";

            try
            {
                System.IO.Directory.Delete(fullPath, true);
            }
            catch (Exception ex)
            {
                resultat = ex.Message;
            }

            DataRow dr = dtResults.NewRow();
            dr["Result"] = resultat;
            dtResults.Rows.Add(dr);
            return dtResults;          
        }

        public DataTable ListFolders(string fullPath)
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(fullPath).GetDirectories())
            {
                DataRow dr = dtResults.NewRow();

                dr["Name"] = dir.Name;
                dr["FullPath"] = dir.FullName;
                dr["CreationDate"] = dir.CreationTime;
                dr["LastModified"] = dir.LastWriteTime;
                dtResults.Rows.Add(dr);
            }
            return dtResults;
        }

        public DataTable ListFiles(string fullPath)
        {
            foreach (FileInfo file in new DirectoryInfo(fullPath).GetFiles())
            {
                DataRow dr = dtResults.NewRow();


                dr["Name"] = file.Name;
                dr["FullPath"] = file.FullName;
                dr["CreationDate"] = file.CreationTime;
                dr["LastModified"] = file.LastWriteTime;
                //enregistrement des résultats
                dtResults.Rows.Add(dr);
            }
            return dtResults;
        }
    }
}
