using SourceCode.SmartObjects.Services.ServiceSDK;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace K2Field.FileSystemBroker
{
    class ServiceBroker : ServiceAssemblyBase
    {
        //Configuration instance de service
        public override string GetConfigSection()
        {

            return base.GetConfigSection();
        }

        //configuration service objects (properties - méthodes)
        public override string DescribeSchema()
        {
            #region svo folder
            //svo foler 
            ServiceObject svo = new ServiceObject("Folder");
            svo.MetaData.DisplayName = "Folder";
            svo.Active = true;

            //properties
            svo.Properties.Add(CreateProperty("Name", SoType.Memo));
            svo.Properties.Add(CreateProperty("FullPath", SoType.Memo));
            svo.Properties.Add(CreateProperty("CreationDate", SoType.DateTime));
            svo.Properties.Add(CreateProperty("LastModified", SoType.DateTime));
            svo.Properties.Add(CreateProperty("Result", SoType.Text));
            

            //méthodes
            svo.Methods.Add(CreateMethod("Create", MethodType.Create));
            svo.Methods.Add(CreateMethod("Delete", MethodType.Delete));
            svo.Methods.Add(CreateMethod("ListFolders", MethodType.List));
            svo.Methods.Add(CreateMethod("ListFiles", MethodType.List));
            
            //méthodes properties
            svo.Methods["Create"].InputProperties.Add("FullPath");
            svo.Methods["Create"].ReturnProperties.Add(svo.Properties["Result"]);

            svo.Methods["Delete"].InputProperties.Add("FullPath");
            svo.Methods["Delete"].ReturnProperties.Add(svo.Properties["Result"]);

            svo.Methods["ListFolders"].InputProperties.Add("FullPath");
            svo.Methods["ListFolders"].ReturnProperties.Add(svo.Properties["Name"]);
            svo.Methods["ListFolders"].ReturnProperties.Add(svo.Properties["FullPath"]);
            svo.Methods["ListFolders"].ReturnProperties.Add(svo.Properties["CreationDate"]);
            svo.Methods["ListFolders"].ReturnProperties.Add(svo.Properties["LastModified"]);
            
            svo.Methods["ListFiles"].InputProperties.Add("FullPath");
            svo.Methods["ListFiles"].ReturnProperties.Add(svo.Properties["Name"]);
            svo.Methods["ListFiles"].ReturnProperties.Add(svo.Properties["FullPath"]);
            svo.Methods["ListFiles"].ReturnProperties.Add(svo.Properties["CreationDate"]);
            svo.Methods["ListFiles"].ReturnProperties.Add(svo.Properties["LastModified"]);

            this.Service.ServiceObjects.Add(svo);
            #endregion

            //svo file
            #region svo file
            ServiceObject svoFile = new ServiceObject("File");
            svoFile.MetaData.DisplayName = "File";
            svoFile.Active = true;

            //properties
            svoFile.Properties.Add(CreateProperty("Name", SoType.Memo));
            svoFile.Properties.Add(CreateProperty("FullPath", SoType.Memo));
            svoFile.Properties.Add(CreateProperty("CreationDate", SoType.DateTime));
            svoFile.Properties.Add(CreateProperty("LastModified", SoType.DateTime));
            svoFile.Properties.Add(CreateProperty("File", SoType.File));
            svoFile.Properties.Add(CreateProperty("CreatedBy", SoType.Text));
            svoFile.Properties.Add(CreateProperty("CreationDate", SoType.DateTime));
            svoFile.Properties.Add(CreateProperty("LastAccessDate", SoType.DateTime));
            svoFile.Properties.Add(CreateProperty("LastWriteDate", SoType.DateTime));
            svoFile.Properties.Add(CreateProperty("Length", SoType.Number));
            svoFile.Properties.Add(CreateProperty("Result", SoType.Text));

            //méthodes
            svoFile.Methods.Add(CreateMethod("Create", MethodType.Create));
            svoFile.Methods.Add(CreateMethod("Update", MethodType.Update));
            svoFile.Methods.Add(CreateMethod("Delete", MethodType.Delete));
            svoFile.Methods.Add(CreateMethod("Read", MethodType.Read));


            //Méthode Properties
            svoFile.Methods["Create"].InputProperties.Add("FullPath");
            svoFile.Methods["Create"].InputProperties.Add("File");
            
            svoFile.Methods["Update"].InputProperties.Add("FullPath");
            svoFile.Methods["Update"].InputProperties.Add("File");

            svoFile.Methods["Delete"].InputProperties.Add("FullPath");
           
            svoFile.Methods["Read"].InputProperties.Add("FullPath");
            svoFile.Methods["Read"].ReturnProperties.Add(svoFile.Properties["File"]);
            svoFile.Methods["Read"].ReturnProperties.Add(svoFile.Properties["CreatedBy"]);
            svoFile.Methods["Read"].ReturnProperties.Add(svoFile.Properties["CreationDate"]);
            svoFile.Methods["Read"].ReturnProperties.Add(svoFile.Properties["LastAccessDate"]);
            svoFile.Methods["Read"].ReturnProperties.Add(svoFile.Properties["LastWriteDate"]);
            svoFile.Methods["Read"].ReturnProperties.Add(svoFile.Properties["Length"]);

            this.Service.ServiceObjects.Add(svoFile);
            #endregion 

            //SVO ACL
            #region svo acl
            ServiceObject svoAcl = new ServiceObject("ACL");
            svoAcl.MetaData.DisplayName = "ACL";
            svoAcl.Active = true;

            //properties
            svoAcl.Properties.Add(CreateProperty("FullPath", SoType.Memo));
            svoAcl.Properties.Add(CreateProperty("Login", SoType.Memo));
            svoAcl.Properties.Add(CreateProperty("Permission", SoType.Text));
            svoAcl.Properties.Add(CreateProperty("Result", SoType.Memo));
            

            //méthodes
            svoAcl.Methods.Add(CreateMethod("AddACL", MethodType.Update));

            //Méthode Properties
            svoAcl.Methods["AddACL"].InputProperties.Add("FullPath");
            svoAcl.Methods["AddACL"].InputProperties.Add("Login");
            svoAcl.Methods["AddACL"].InputProperties.Add("Permission");
            svoAcl.Methods["AddACL"].ReturnProperties.Add(svo.Properties["Result"]);

         
            this.Service.ServiceObjects.Add(svoAcl);

            #endregion 

            return base.DescribeSchema();
        }

        //exécution méthode
        public override void Execute()
        {
            //récupération service appellé 
            ServiceObject calledSvo = this.Service.ServiceObjects[0];
            //récupération méthode 
            Method method = calledSvo.Methods[0];

            //création générique table de retour
            DataTable dtResults = new DataTable();
            for (int i = 0; i < method.ReturnProperties.Count; i++)
            {
                dtResults.Columns.Add(method.ReturnProperties[i]);
            }


            #region Folder
            if (calledSvo.Name == "Folder")
            {
                FolderHelper helper = new FolderHelper(dtResults);
                string fullPath = calledSvo.Properties["FullPath"].Value.ToString();
                //méthode scalaire
                if (method.Name == "Create")
                {
                    dtResults = helper.CreateFolder(fullPath);
                }
                //méthode scalaire
                else if (method.Name == "Delete")
                {
                    dtResults = helper.DeleteFolder(fullPath);
                }
                else if (method.Name == "ListFolders")
                {
                    dtResults = helper.ListFolders(fullPath);
                }
                else if (method.Name == "ListFiles")
                {
                    dtResults = helper.ListFiles(fullPath);

                }
            }
            #endregion

            #region file
            else if (calledSvo.Name == "File")
            {
                calledSvo.Properties.InitResultTable();

                FileHelper fileHelper = new FileHelper(dtResults);

                if (method.Name == "Create")
                {
                    //récupération property
                    string fullPath = (string)calledSvo.Properties["FullPath"].Value;
                    FileProperty fProp = (FileProperty)calledSvo.Properties["File"];

                    fileHelper.CreateFile(fullPath, fProp.Content);
                }
                else if (method.Name == "Update")
                {
                    //récupération property
                    string filePath = (string)calledSvo.Properties["FullPath"].Value;
                    FileProperty fProp = (FileProperty)calledSvo.Properties["File"];
                    fileHelper.UpdateFile(filePath, fProp.Content);

                    calledSvo.Properties.BindPropertiesToResultTable();
                }
                //méthode scalaire
                else if (method.Name == "Delete")
                {
                    //récupération property
                    string filePath = (string)calledSvo.Properties["FullPath"].Value;
                    fileHelper.DeleteFile(filePath);
                }
                else if (method.Name == "Read")
                {
                    string path = (string)calledSvo.Properties["FullPath"].Value;

                    dtResults = fileHelper.ReadFile(path, dtResults);
                }
            }
            #endregion

            #region ACL
            if (calledSvo.Name == "ACL")
            {
                ACLHelper helper = new ACLHelper(dtResults);

                string fullPath = calledSvo.Properties["FullPath"].Value.ToString();
                string login = calledSvo.Properties["Login"].Value.ToString();
                string permission = calledSvo.Properties["Permission"].Value.ToString();
                if (method.Name =="AddACL")
                {
                    dtResults = helper.SetFolderAcl(fullPath, login, permission);
                }
            }

            #endregion 
            
                calledSvo.Properties.InitResultTable();
                foreach (DataRow dr in dtResults.Rows)
                {
                    foreach (DataColumn column in dtResults.Columns)
                    {
                        if (dr != null)
                            calledSvo.Properties[column.ColumnName].Value = dr[column].ToString();
                    }
                    calledSvo.Properties.BindPropertiesToResultTable();
                }
           
        }

        Property CreateProperty(string Name, SoType soType)
        {
            Property prop = new Property(Name);
            prop.MetaData.DisplayName = Name;
            prop.SoType = soType;
            return prop;
        }

        Method CreateMethod(string Name, MethodType type)
        {
            Method method = new Method(Name);
            method.MetaData.DisplayName = Name;
            method.Type = type;
            return method;
        }
            


        public override void Extend()
        {
            throw new NotImplementedException();
        }

    }
}
