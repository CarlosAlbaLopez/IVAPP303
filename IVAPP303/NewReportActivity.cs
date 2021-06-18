using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;
using Firebase;
using System;
using Firebase.Auth;
using Firebase.Storage;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using FirebaseDatabase.Model;
using Android.Util;

namespace IVAPP303
{
    [Activity(Label = "FirebaseDatabase", Theme = "@style/AppTheme")]
    public class NewReportActivity : AppCompatActivity, IOnProgressListener, IOnSuccessListener, IOnFailureListener
    {
        ProgressDialog progressR;
        private Button btnSave;
        EditText title, comments;
        FirebaseAuth auth;
        FirebaseStorage storage;
        StorageReference storageRef;
        private const string FirebaseURL = "https://ivapp303.firebaseio.com/"; //Firebase Auth & DB URL

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.NewReport);

            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            FirebaseUser user = auth.CurrentUser;


            //iniciar firebase common (incluye storage)
            FirebaseApp.InitializeApp(this);

            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl("gs://ivapp303.appspot.com/");

            //View   
            btnSave = FindViewById<Button>(Resource.Id.btn_save);
            title = FindViewById<EditText>(Resource.Id.input_title);
            comments = FindViewById<EditText>(Resource.Id.input_comments);

            //eventos
            btnSave.Click += delegate
            {
                btnSave.Enabled = false;
                progressR = new ProgressDialog(this);
                progressR.Indeterminate = true;
                progressR.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
                progressR.SetMessage("Creando liquidación...");
                progressR.SetCancelable(false);
                progressR.Show();
                CreateReport();
                StartActivity(typeof(PanelActivity));
                Finish();
            };
        }

        private void CreateReport()
        {
            Report report = new Report();
            report.Title = title.Text;
            report.Comments = comments.Text;
            report.Amount = "0";
            report.Date = "";
            report.ID = Guid.NewGuid().ToString();
            var firebase = new FirebaseClient(FirebaseURL);
            //Add Item  

            //PUTASYNC en vez de POSTASYNC hace que el child previo sea el nombre del nodo en vez de generar uno al azar
            var item = firebase.Child("reports").Child(auth.CurrentUser.Uid).Child(report.ID).PutAsync<Report>(report);
        }



        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            //{
            //    try
            //    {
            //        Android.Graphics.Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, uriSavedImage);
            //        imgView.SetImageBitmap(bitmap);
            //    }
            //    catch (Exception)
            //    {
            //        StartActivity(typeof(LvDocActivity));

            //        Finish();
            //    }
            //}
            //else
            //{
            //    Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 3);
            //}
        }

        public void OnProgress(Java.Lang.Object snapshot)
        {
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            StartActivity(typeof(LvDocActivity));

            Finish();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, "" + e.Message, ToastLength.Short).Show();
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(LvDocActivity));

            Finish();
        }
    }
}
