using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace merge
{
    public enum _dataType { Male, Female, Unknown };

    class DataElement
    {
        public string DataString { get; set; }

        public string Filename { get; set; }

        public _dataType DataType { get; set; }
        public long ClusterID { get; set; }
        public int HouseHoldID { get; set; }
        public long UniqueID { get; set; }
        public bool IsBlank { get; set; }
        public bool IsValid { get; set; }

        private string StartRegex {
            get {
                return DataType == _dataType.Female ? FemaleStartRegex : MaleStartRegex;
            }
        }

        private static string FemaleStartRegex = @"[J][\s][\d]|[J][\d]";
        public static string MaleStartRegex = @"[/\^/][\s][\d]|[/\^/][\d]";

        private static string FemaleDataLine = @"[\d]*[A-Z]*[\s][\d][\s][\d]+[\s][\d]";
        public static string MaleDataLine = @"[a-z][\s][\d][\s][\d]+[\s][\d]";

        public DataElement(string datastr)
        {
            if (!string.IsNullOrEmpty(datastr))
            {
                DataString = datastr;
                DataType = GetDataType(datastr);
                HouseHoldID = GetHouseholdId_(datastr);
                IsBlank = IsDataEmpty(datastr);
                UniqueID = GetUniqueID(datastr);
            }

            IsValid = Validate();
        }

        private bool Validate()
        {
            return UniqueID > 0;
        }

        private bool IsDataEmpty(string datastr)
        {
            int blankcount = 0;

            using (StringReader reader1 = new StringReader(datastr))
            {
                string line;
                while ((line = reader1.ReadLine()) != null)
                {
                    string regex = DataType == _dataType.Female ? FemaleDataLine : MaleDataLine;
                    if (Regex.Match(line, regex).Success)
                    {
                        var tokens = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length < 5)
                        {
                            blankcount++;
                            if (blankcount >= 3)
                                return true;
                        }
                        else
                        {
                            blankcount = 0;
                        }
                    }
                }
            }

            return false;
        }

        private string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        private long GetUniqueID(string datastr)
        {
            long uniqueId = -1;

            using (StringReader reader1 = new StringReader(datastr))
            {
                string line;
                if ((line = reader1.ReadLine()) != null)
                {
                    string regex = DataType == _dataType.Female ? FemaleStartRegex : MaleStartRegex;

                    if (Regex.Match(line, regex).Success)
                    {
                        int idlen = 14;
                        if (line.Length >= idlen)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 1; i < idlen; i++)
                                sb.Append(line[i].ToString());

                            string value = GetNumbers(sb.ToString());
                            long.TryParse(value, out uniqueId);
                        }
                    }
                }
            }

            return uniqueId;
        }
        
        private int GetHouseholdId_(string datastr)
        {
            int householdId = -1;
            var tokens = new List<string>();
            string line;

            using (StringReader reader1 = new StringReader(datastr))
            {
                if ((line = reader1.ReadLine()) != null)
                {
                    if (Regex.Match(line, StartRegex).Success)
                    {
                        char h1 = line[12];
                        char h2 = line[13];
                        string hhid = (h1.ToString() + h2.ToString()).Trim();
                        int.TryParse(GetNumbers(hhid), out householdId);
                    }
                }
            }

            if (householdId <= 0)
            {
                //var count = tokens.Count();
                //var toks = Regex.Split(line, StartRegex);
            }

            return householdId;
        }
        
        private _dataType GetDataType(string datastr)
        {
            using (StringReader reader1 = new StringReader(datastr))
            {
                string line;
                while ((line = reader1.ReadLine()) != null)
                {
                    if (Regex.Match(line, FemaleStartRegex).Success)
                        return _dataType.Female;

                    if (Regex.Match(line, MaleStartRegex).Success)
                        return _dataType.Male;
                }
            }

            return _dataType.Unknown;
        }

        public static IList<DataElement> CreateDataElements(string datastr, string filename)
        {
            IList<DataElement> elist = ReadDataElements(datastr);
            elist.AsParallel().ForAll(x=>x.Filename = filename);
            return elist;
        }

        public static IList<DataElement> ReadDataElements(string datastr)
        {
            List<DataElement> elist = new List<DataElement>();

            IList<DataElement> flist = ReadFemaleDataElements(datastr);
            IList<DataElement> mlist = ReadMaleDataElements(datastr);

            elist.AddRange(flist);
            elist.AddRange(mlist);

            return elist;
        }

        public static IList<DataElement> ReadFemaleDataElements(string datastr)
        {
            List<DataElement> elist = new List<DataElement>();
            StringBuilder appendLines = new StringBuilder();

            using (StringReader reader2 = new StringReader(datastr))
            {
                string line;
                while ((line = reader2.ReadLine()) != null)
                {
                    if (Regex.Match(line, FemaleStartRegex).Success)
                    {
                        appendLines.AppendLine(line);
                        while ((line = reader2.ReadLine()) != null && !Regex.Match(line, MaleStartRegex).Success)
                        {
                            appendLines.AppendLine(line);
                        }

                        elist.Add(new DataElement(appendLines.ToString()));
                        appendLines = new StringBuilder();
                    }
                }

            }

            return elist;
        }

        public static IList<DataElement> ReadMaleDataElements(string datastr)
        {
            List<DataElement> elist = new List<DataElement>();
            StringBuilder appendLines = new StringBuilder();

            using (StringReader reader2 = new StringReader(datastr))
            {
                string line;
                while ((line = reader2.ReadLine()) != null)
                {
                    if (Regex.Match(line, MaleStartRegex).Success)
                    {
                        appendLines.AppendLine(line);
                        while ((line = reader2.ReadLine()) != null && !Regex.Match(line, FemaleStartRegex).Success)
                        {
                            appendLines.AppendLine(line);
                        }

                        elist.Add(new DataElement(appendLines.ToString()));
                        appendLines = new StringBuilder();
                    }
                }

            }

            return elist;
        }

    }
}