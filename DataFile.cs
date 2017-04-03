using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace merge
{
    class DataFile
    {
        public long UniqueID { get; set; }

        private IList<DataElement> _sortedElements;
        public IList<DataElement> SortedElements
        {
            set
            {
                if (null == _sortedElements) { _sortedElements = value; }
            }
            get { return _sortedElements; }
        }

        public List<DataElement> DataElements { get; set; }
        
        public DataFile(List<DataElement> delements)
        {
            this.DataElements = delements;
            if (this.DataElements != null && this.DataElements.Count >= 1)
            {
                this.UniqueID = this.DataElements.First().UniqueID;
                this.SortedElements = GetSortedDataElements();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in SortedElements)
                sb.Append(e.DataString);
            return sb.ToString().TrimEnd().TrimStart();
        }

        public static void WriteDataElements(IList<DataElement> list)
        {
            int elementCount = 0;

            foreach (var e in list)
            {
                elementCount++;
                string filename = e.IsBlank ? elementCount.ToString() + " (blank)" + ".dat" : elementCount.ToString() + ".dat";
                string filepath = Path.Combine(Environment.CurrentDirectory, filename);
                File.WriteAllText(filepath, e.DataString);
            }
        }
        
        public IList<DataElement> GetSortedDataElements()
        {            
            List<DataElement> unsorted = DataElements.Where(x => x.IsValid).ToList();
            List<DataElement> sorted = new List<DataElement>();
     
            // max household id
            var householdmaxid = unsorted.Max(x => x.HouseHoldID);

            // min household id
            var i = unsorted.Min(x => x.HouseHoldID);

            for (; i <= householdmaxid; i++)
            {
                var female = unsorted.FirstOrDefault(x => x.HouseHoldID == i && x.DataType == _dataType.Female);
                var male = unsorted.FirstOrDefault(x => x.HouseHoldID == i && x.DataType == _dataType.Male);

                if (female != null && male != null)
                {
                    sorted.Add(female);
                    sorted.Add(male);
                }

                if (female == null)
                    Console.WriteLine("no female data found for household {0} in unique id {1}", i, female.UniqueID);
                
                else if (male == null)
                    Console.WriteLine("no male data found for household {0} in unique id {1}", i, male.UniqueID);
            }

            return sorted.ToList();
        }
    }
}
