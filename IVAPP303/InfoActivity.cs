using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace IVAPP303
{
    [Activity(Label = "InfoActivity", Theme = "@style/AppTheme")]
    class InfoActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Info);

        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(LvDocActivity));

            Finish();

        }

    }
}