using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace merge
{
    class FileManager
    {
        private IList<DataFile> DataFiles { get; set; }
        private string RootFolder { get; set; }

        private static string FileName = string.Format("urban{0}.dat", DateTime.Now.ToString("yyyyMMdd"));

        public FileManager(string root)
        {
            var flist = GetFiles(root);
            DeleteIfExists(flist); // delete if a merge file exists in the current folder
            this.RootFolder = root;
            DataFiles = this.GetDataFiles(root);
            var count = DataFiles.Count();
        }

        private void DeleteIfExists(IList<string> flist)
        {
            
            foreach (var file in flist)
            {
                if (Path.GetFileName(file) ==  FileManager.FileName)
                {
                    Console.WriteLine("deleting {0}", file);
                    File.Delete(file);
                }
            }
            
        }

        private void PrintInvalids(IList<DataElement> invalidlist)
        {
            foreach (var e in invalidlist)
            {
                if (!e.IsValid)
                {
                    Console.WriteLine("unique id {0} is invalid or empty", e.UniqueID);
                }
            }
        }

        private IList<DataElement> GetValidElements(string file)
        {
            List<DataElement> validlist = new List<DataElement>();
            string filename = Path.GetFileName(file);
            string datastr = File.ReadAllText(file);

            if (!string.IsNullOrEmpty(datastr))
            {
                var elist = DataElement.CreateDataElements(datastr, filename);
                // validlist can include blank data
                validlist = elist.Where(x => x.IsValid).ToList(); // IsBlank check removed
                var invalislist = elist.Where(item => !validlist.Contains(item)).ToList();
                return validlist;
            }

            return validlist;            
        }

        private IList<DataElement> GetAllElements(IList<string> flist)
        {
            List<DataElement> allelements = new List<DataElement>();

            foreach (var file in flist)
            {
                allelements.AddRange(GetValidElements(file));
            }

            return allelements;
        }
       
        private IList<DataFile> GetDataFiles(string root)
        {
            List<DataFile> dfilelist = new List<DataFile>();

            var flist = GetFiles(root);
            var allelements = GetAllElements(flist);
            
            if (allelements.Count > 0)
            {
                var result = from cx in allelements group cx by cx.UniqueID into 
                             cxGroup orderby cxGroup.Key select cxGroup;

                foreach (var cxGroup in result)
                {
                    Console.WriteLine(string.Format("unique id {0}", cxGroup.Key));
                    List<DataElement> dlist = new List<DataElement>();

                    foreach (var cx in cxGroup)
                        dlist.Add(cx);

                    if (dlist.Count > 0)
                    {
                        var d = new DataFile(dlist) { UniqueID = cxGroup.Key };
                        
                        dfilelist.Add(d);
                    }
                }
            }

            return dfilelist;
        }

        public void Save()
        {
            StringBuilder sb = new StringBuilder();

            if (DataFiles != null && DataFiles.Count > 0)
            {
                foreach (var d in DataFiles)
                    sb.AppendLine(d.ToString());

                Console.WriteLine("saving merged files...");
                string path = Path.Combine(this.RootFolder, FileManager.FileName);
                string content = sb.ToString().TrimEnd().TrimStart();
                File.WriteAllText(path, content);
                Console.WriteLine(FileManager.FileName);
            }           
        }

        public IList<string> GetFiles(string directory)
        {
            return Directory.GetFiles(directory, "*.dat", SearchOption.AllDirectories).ToList();
        }
    }
}
