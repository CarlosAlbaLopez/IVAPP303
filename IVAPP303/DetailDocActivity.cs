using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace IVAPP303
{
    [Activity(Label = "DetailDocActivity", Theme = "@style/AppTheme")]
    public partial class DetailDocActivity : AppCompatActivity
    {
        private const string FirebaseURL = "https://ivapp303.firebaseio.com/"; //Firebase Database URL 
        private EditText detail_total, detail_nombreFiscal, detail_CIF, detail_comentarios;
        private ImageButton imgDoc;
        private Button btnDate, btnAddToReport, btnCategory, btnSave;
        FirebaseAuth auth;
        
        public GetDocumentsInReportAdapter adapter2;
        //private List<Report> list_reports_para_nombre = new List<Report>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.DetailDoc);

            //Add Toolbar  
            Android.Support.V7.Widget.Toolbar toolbar2 = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tbDetailDoc);
            toolbar2.Title = "IVA303";
            SetSupportActionBar(toolbar2);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //View   
            imgDoc = FindViewById<ImageButton>(Resource.Id.imgView);
            btnDate = FindViewById<Button>(Resource.Id.btnDate);
            detail_total = FindViewById<EditText>(Resource.Id.list2_total);
            btnAddToReport = FindViewById<Button>(Resource.Id.btnAddToReport);
            detail_nombreFiscal = FindViewById<EditText>(Resource.Id.list2_Proveedor);
            detail_CIF = FindViewById<EditText>(Resource.Id.list2_CIF);
            btnCategory = FindViewById<Button>(Resource.Id.btnCategory);
            detail_comentarios = FindViewById<EditText>(Resource.Id.list2_comentarios);
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            
            string docCIF = Intent.GetStringExtra("CIFdoc");
            string docNombreF = Intent.GetStringExtra("nombreFiscalDoc");
            string docTotal = Intent.GetStringExtra("totalDoc");
            string docFecha = Intent.GetStringExtra("fechaDoc");
            string docComent = Intent.GetStringExtra("comentarioDoc");
            string docTipoG = Intent.GetStringExtra("tipoGastoDoc");
            string docIdLiq = Intent.GetStringExtra("idLiqDoc");
            string docFilename = Intent.GetStringExtra("filenameDoc");
            string docId = Intent.GetStringExtra("idDocDoc");
            string nombrereport = Intent.GetStringExtra("NombreReport");

            btnDate.Text = docFecha;
            detail_total.Text = docTotal + "€";
            if (docIdLiq == "")
            {
                btnAddToReport.Text = "Seleccionar";
            }
            else
            {
                btnAddToReport.Text = nombrereport;
            }
            detail_nombreFiscal.Text = docNombreF;
            detail_CIF.Text = docCIF;
            btnCategory.Text = docTipoG;
            detail_comentarios.Text = docComent;

            //EVENTOS
            btnSave.Click += delegate
            {

            };

            btnAddToReport.Click += delegate
            {
                Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
                builder.SetTitle("Añadir gasto a liquidación");
                builder.SetMessage("¿Desea añadir el gasto a una nueva liquidación o a una ya existente?");
                builder.SetPositiveButton("NUEVA LIQUIDACION", delegate
                {
                    var openAddToNewReportIntent = new Intent(this, typeof(NewReportActivity));
                    openAddToNewReportIntent.PutExtra("CIFdoc", Intent.GetStringExtra("CIFdoc"));
                    openAddToNewReportIntent.PutExtra("nombreFiscalDoc", Intent.GetStringExtra("nombreFiscalDoc"));
                    openAddToNewReportIntent.PutExtra("totalDoc", Intent.GetStringExtra("totalDoc"));
                    openAddToNewReportIntent.PutExtra("fechaDoc", Intent.GetStringExtra("fechaDoc"));
                    openAddToNewReportIntent.PutExtra("comentarioDoc", Intent.GetStringExtra("comentarioDoc"));
                    openAddToNewReportIntent.PutExtra("tipoGastoDoc", Intent.GetStringExtra("tipoGastoDoc"));
                    openAddToNewReportIntent.PutExtra("idLiqDoc", Intent.GetStringExtra("idLiqDoc"));
                    openAddToNewReportIntent.PutExtra("filenameDoc", Intent.GetStringExtra("filenameDoc"));
                    openAddToNewReportIntent.PutExtra("idDocDoc", Intent.GetStringExtra("idDocDoc"));
                    StartActivity(openAddToNewReportIntent);
                    Finish();
                });
                builder.SetNegativeButton("LIQUIDACION EXISTENTE", delegate
                {
                    var openAddToReportIntent = new Intent(this, typeof(PanelActivity));
                    openAddToReportIntent.PutExtra("CIFdoc", Intent.GetStringExtra("CIFdoc"));
                    openAddToReportIntent.PutExtra("nombreFiscalDoc", Intent.GetStringExtra("nombreFiscalDoc"));
                    openAddToReportIntent.PutExtra("totalDoc", Intent.GetStringExtra("totalDoc"));
                    openAddToReportIntent.PutExtra("fechaDoc", Intent.GetStringExtra("fechaDoc"));
                    openAddToReportIntent.PutExtra("comentarioDoc", Intent.GetStringExtra("comentarioDoc"));
                    openAddToReportIntent.PutExtra("tipoGastoDoc", Intent.GetStringExtra("tipoGastoDoc"));
                    openAddToReportIntent.PutExtra("idLiqDoc", docIdLiq);
                    openAddToReportIntent.PutExtra("filenameDoc", Intent.GetStringExtra("filenameDoc"));
                    openAddToReportIntent.PutExtra("idDocDoc", Intent.GetStringExtra("idDocDoc"));
                    openAddToReportIntent.PutExtra("pantallaprevia", "panel");
                    StartActivity(openAddToReportIntent);
                    Finish();
                });
                builder.Show();
            };

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {

                try
                {
                    Android.Net.Uri uriSavedImage = Android.Net.Uri.FromFile(new Java.IO.File("/sdcard/" + docFilename + ".png"));
                    Android.Graphics.Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, uriSavedImage);
                    imgDoc.SetImageBitmap(bitmap);
                }
                catch (Exception)
                {
                    Toast.MakeText(ApplicationContext, "Imagen no encontrada", ToastLength.Long).Show();
                }

            }
            else
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 3);
            }

            //Android.Net.Uri uriSavedImage = Android.Net.Uri.FromFile(new Java.IO.File("/sdcard/" + docFilename + ".png"));
            //Android.Graphics.Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, uriSavedImage);
            //imgDoc.SetImageBitmap(bitmap);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_reports, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_download)
            {
                //to do
            }
            else
            if (id == Resource.Id.menu_delete)
            {
                AskForConfirmation();
            }
            else
            if (id == Resource.Id.menu_add)
            {
                //to do
            }
            return base.OnOptionsItemSelected(item);
        }

        private void AskForConfirmation()
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("Mensaje de confirmación");
            builder.SetMessage("¿Desea eliminar este documento?");
            builder.SetPositiveButton("SI", delegate
            {
                DeleteDoc();
            });
            builder.SetNegativeButton("NO", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }

        public void DeleteDoc()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            firebase.Child("documents").Child(auth.CurrentUser.Uid).Child(Intent.GetStringExtra("idDocDoc")).DeleteAsync();

            //por hacer: restar cantidad del Amount del report correspondiente
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(LvDocActivity));

            Finish();
        }
    }
}

