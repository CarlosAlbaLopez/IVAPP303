using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace IVAPP303
{
    public class Report
    {
        public string Title { get; set; }
        public string Comments { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public string ID { get; set; }

    }
}