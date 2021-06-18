using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using FirebaseDatabase.Model;

namespace IVAPP303
{
    [Activity(Label = "AddTicketsToReport")]
    public class AddTicketsToReport : Activity
    {
        private const string FirebaseURL = "https://ivapp303.firebaseio.com/"; //Firebase Auth & DB URL
        private ListView list_info1;
        private ProgressBar circular_progress;
        private List<Report> list_reports = new List<Report>();
        FirebaseAuth auth;
        public LvLiqAdapter adapter;

        protected override  void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddTicketsToReport);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            string IDDocumento = Intent.GetStringExtra("idDocDoc");

            list_info1 = FindViewById<ListView>(Resource.Id.list_info1);
            list_info1.ItemClick += (s, e) =>
            {
  
            };
            _ = LoadDatos();
        }

        public async Task LoadDatos()
        {
            circular_progress.Visibility = ViewStates.Visible;
            list_info1.Visibility = ViewStates.Invisible;
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("reports")
                .Child(auth.CurrentUser.Uid)
                .OnceAsync<Report>();
            list_reports.Clear();
            adapter = null;
            foreach (var item in items)
            {
                Report report = new Report();
                report.Title = item.Object.Title;
                report.Comments = item.Object.Comments;
                report.Date = item.Object.Date;
                report.Amount = item.Object.Amount;
                report.ID = item.Object.ID;
                list_reports.Add(report);
            }
            adapter = new LvLiqAdapter(this, list_reports);
            adapter.NotifyDataSetChanged();
            list_info1.Adapter = adapter;
            circular_progress.Visibility = ViewStates.Invisible;
            list_info1.Visibility = ViewStates.Visible;
        }
    }
}