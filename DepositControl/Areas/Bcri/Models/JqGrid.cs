using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DepositControl.Areas.Bcri.Models
{
    public class JqGrid
    {
        public string editurl { get; set; }
        public string url { get; set; }
        //public IEnumerable<object> data { get; set; }
        public IEnumerable<JqGridColModel> colModel { get; set; }
        public string pager { get; set; }
        public string datatype { get; set; } = "json";
        public string sortname { get; set; } = "Id";
        public string sortorder { get; set; } = "ASC";
        public bool loadonce { get; set; } = true;
        public bool sortable { get; set; } = true;
        public bool viewrecords { get; set; } = true;
        public bool rownumbers { get; set; } = false;
        public bool shrinkToFit { get; set; } = false;
        public bool forceFit { get; set; } = false;
        public bool ignoreCase { get; set; } = true;
        public int width { get; set; } = 450;
        public int height { get; set; } = 350;
        public int rowNum { get; set; } = 20;

    }
}
