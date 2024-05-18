using System.Collections.Generic;
using Newtonsoft.Json;

namespace ERPSystem.DataModel
{
    public class PagingData<T>
    {
        public PagingData()
        {
            Meta = new Meta();
        }
        public List<T> Data { get; set; }
        public Meta Meta { get; set; }
    }

    public class Meta
    {
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }

    public class PagingData<T1, T2>
    {
        public PagingData()
        {
            Meta = new Meta();
        }
        public List<T1> Data { get; set; }
        public List<T1> Data1 { get; set; }
        public List<T2> Header { get; set; }
        public Meta Meta { get; set; }
    }

    public class PagingDataNewObject<T>
    {
        public PagingDataNewObject()
        {
            Meta = new Meta();
        }
        public List<T> Data { get; set; }
        public Meta Meta { get; set; }
        public object DataInit { get; set; }
    }

    public class HeaderData
    {
        public string PageName { get; set; }

        public int HeaderId { get; set; }
        public string HeaderName { get; set; }
        public string HeaderVariable { get; set; }
        public bool IsCategory { get; set; }

        //[JsonIgnore]
        public int HeaderOrder { get; set; }

        /// <summary>
        /// This variable is for visibility on page.
        /// </summary>
        public bool IsVisible { get; set; } = true;
    }
}
