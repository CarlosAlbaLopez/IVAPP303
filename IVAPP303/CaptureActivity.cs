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
    public class CaptureActivity : AppCompatActivity, IOnProgressListener, IOnSuccessListener, IOnFailureListener
    {
        ProgressDialog progress;
        static Android.Net.Uri uriSavedImage;
        private Button btnUpload, btnDate, btnCategory;
        private EditText input_total, input_comentarios, input_proveedor;
        private TextView date_display, tvTotal, tvProveedor, tvCategory;
        FirebaseAuth auth;
        FirebaseStorage storage;
        StorageReference storageRef;
        string Timestamp = System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_");
        private ImageButton imgView;
        private const int PICK_IMAGE_REQUEST = 71;
        private const string FirebaseURL = "https://ivapp303.firebaseio.com/"; //Firebase Auth & DB URL

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.Capture);

            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            FirebaseUser user = auth.CurrentUser;

            //iniciar firebase common (incluye storage)
            FirebaseApp.InitializeApp(this);

            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl("gs://ivapp303.appspot.com/");

            uriSavedImage = Android.Net.Uri.FromFile(new Java.IO.File("/sdcard/" + Timestamp + auth.CurrentUser.Uid + ".png"));

            //View   
            btnUpload = FindViewById<Button>(Resource.Id.btnUpload);
            btnDate = FindViewById<Button>(Resource.Id.btnDate);
            btnCategory = FindViewById<Button>(Resource.Id.btnCategory);
            date_display = FindViewById<TextView>(Resource.Id.date_display1);
            tvTotal = FindViewById<TextView>(Resource.Id.tvTotal);
            tvProveedor = FindViewById<TextView>(Resource.Id.tvProveedor);
            tvCategory = FindViewById<TextView>(Resource.Id.tvCategory);
            input_total = FindViewById<EditText>(Resource.Id.list2_total);
            input_proveedor = FindViewById<EditText>(Resource.Id.list2_Proveedor);
            input_comentarios = FindViewById<EditText>(Resource.Id.list2_comentarios);
            imgView = FindViewById<ImageButton>(Resource.Id.imgView);

            btnUpload.Enabled = true;

            Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 3);
            //eventos
            btnUpload.Click += delegate
            {
                btnUpload.Enabled = false;
                progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
                progress.SetMessage("Subiendo imagen...");
                progress.SetCancelable(false);
                progress.Show();
                UploadImage();
                CreateDocumento();
            };

            btnDate.Click += delegate
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time) {
                    btnDate.Text = time.ToShortDateString();
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };

            imgView.Click += (e, o) =>
            {

            };

            btnCategory.Click += (s, arg) => {
                PopupMenu menu = new PopupMenu(this, btnCategory);
                menu.Inflate(Resource.Menu.popup_category);

                menu.MenuItemClick += (s1, arg1) => {
                    btnCategory.Text = arg1.Item.TitleFormatted.ToString();
                };

                menu.DismissEvent += (s2, arg2) => {
                    Console.WriteLine("menu dismissed");
                };

                menu.Show();
            };

            ChooseImage();
        }

        private void UploadImage()
        {
            string filename = Timestamp + auth.CurrentUser.Uid;
            var images = storageRef.Child("images/" + filename);
            images.PutFile(uriSavedImage)
                    .AddOnProgressListener(this)
                    .AddOnSuccessListener(this)
                    .AddOnFailureListener(this);
        }

        private void CreateDocumento()
        {
            Document document = new Document();
            document.CIF = "";
            document.Nombre_Fiscal = input_proveedor.Text;
            document.Tipo_Gasto = btnCategory.Text;
            document.Fecha = btnDate.Text;
            document.Total = input_total.Text;
            document.Comentarios = input_comentarios.Text;
            document.ID_Liquidacion = "";
            document.filename = Timestamp + auth.CurrentUser.Uid;
            document.IdDocumento = Guid.NewGuid().ToString();
            var firebase = new FirebaseClient(FirebaseURL);
            //Add Item  
            //PUTASYNC en vez de POSTASYNC hace que el child previo sea el nombre del nodo en vez de generar uno al azar
            var item = firebase.Child("documents").Child(auth.CurrentUser.Uid).Child(document.IdDocumento).PutAsync<Document>(document);
        }

        private void ChooseImage()
        {
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            Intent intent = new Intent(MediaStore.ActionImageCapture);
            intent.PutExtra(MediaStore.ExtraOutput, uriSavedImage);

            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                try
                {
                    Android.Graphics.Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, uriSavedImage);
                    imgView.SetImageBitmap(bitmap);
                }
                catch (Exception)
                {
                    StartActivity(typeof(LvDocActivity));

                    Finish();
                }
            }
            else
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 3);
            }
        }

        public void OnProgress(Java.Lang.Object snapshot)
        {
            //var taskSnapShot = (UploadTask.TaskSnapshot)snapshot;
            //double progress = (100.0 * taskSnapShot.BytesTransferred / taskSnapShot.TotalByteCount);
            //progressDialog.SetMessage("La cosa está al " + progress + " %");
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            //progressDialog.Dismiss();
            //Toast.MakeText(this, "Exito", ToastLength.Short).Show();

            //var myactivity = new LvDocActivity();
            //await myactivity.LoadData();
            StartActivity(typeof(LvDocActivity));

            Finish();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            //progressDialog.Dismiss();
            Toast.MakeText(this, "" + e.Message, ToastLength.Short).Show();
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(LvDocActivity));

            Finish();
        }
    }

    // Create a class DatePickerFragment  
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.  
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();
        // Initialize this value to prevent NullReferenceExceptions.  
        Action<DateTime> _dateSelectedHandler = delegate { };
        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog = new DatePickerDialog(Activity, this, currently.Year, currently.Month, currently.Day);
            return dialog;
        }
        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!  
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(TAG, selectedDate.ToShortDateString());
            _dateSelectedHandler(selectedDate);
        }
    }
}
