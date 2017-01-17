using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.IO;
using System.Diagnostics;

namespace K2Field.FileSystemBroker.UnitTests
{
    [TestClass]
    public class FileTest
    {
        DataTable dt;

        public FileTest()
        {
            dt = new DataTable();
            dt.Columns.Add("Result");

        }

        [TestMethod]
        public void CreateFileTest()
        {
            FileHelper fileHelper = new FileHelper(dt);
            string testFilePath = @"c:\temp\test.txt";
            string b64file = Convert.ToBase64String(File.ReadAllBytes(testFilePath));

            fileHelper.CreateFile(@"c:\unitest\test.txt", b64file);
        }

        [TestMethod]
        public void ReadFileTest()
        {
            string props = "File;CreatedBy;CreationDate;LastAccesDate;LastWriteDate;Length";
          
            FileHelper fileHelper = new FileHelper(dt);
            string testFilePath = @"c:\temp\test.txt";

            DataTable dtresult = new DataTable();

            foreach(string strPropName in props.Split(';'))
            {
               dtresult.Columns.Add(strPropName);
            }

            dtresult =  fileHelper.ReadFile(testFilePath, dtresult);
            foreach(DataRow row in dtresult.Rows)
            {
                foreach(DataColumn dc in dtresult.Columns)
                {
                    Trace.Write(row[dc.ColumnName].ToString());
                }
            }

        }

    }
}
