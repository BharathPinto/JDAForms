using System;
using System.Collections.Generic;
using System.Text;

namespace JDAForms.Model
{
    public class DataModel
    {
        public DataModel(DateTime startdate, DateTime enddate, string upto, string reason)
        {
            this.startdate = startdate;
            this.enddate = enddate;
            this.upto = upto;
            this.reason = reason;
        }

        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string upto { get; set; }
        public string reason { get; set; }
        public bool status { get; set; }
    }
}
