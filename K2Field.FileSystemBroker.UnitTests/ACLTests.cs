using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace K2Field.FileSystemBroker.UnitTests
{
    [TestClass]
    public class ACLTests
    {

        DataTable dt = new DataTable();

        public ACLTests()
        {
            dt.Columns.Add("Result");
        }

        [TestMethod]
        public void TestAddAcl()
        {
            ACLHelper helper = new ACLHelper(dt);
            dt = helper.SetFolderAcl("c:\\test", "Backup Operators", "Modify");
            
            Assert.AreEqual(dt.Rows[0]["Result"], "true");

           
        }
    }
}
