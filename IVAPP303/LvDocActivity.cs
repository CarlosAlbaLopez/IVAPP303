using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Firebase.Auth;
using Android.Content;
using FirebaseDatabase.Model;

namespace IVAPP303
{
    [Activity(Label = "FirebaseDatabase", Theme = "@style/AppTheme")]
    public class LvDocActivity : AppCompatActivity
    {
        private Document selectedDoc;
        Button btnCapture;
        FirebaseAuth auth;
        public string nombredelReport;
        private ListView list_data;
        private ProgressBar circular_progress;
        private List<Document> list_documentos = new List<Document>();
        public LvDocAdapter adapter;
        private const string FirebaseURL = "https://ivapp303.firebaseio.com/";
        private List<Report> list_reports_para_nombre = new List<Report>();

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.ViewDoc);

            //Add Toolbar  
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "IVA303";
            SetSupportActionBar(toolbar);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //View   
            btnCapture = FindViewById<Button>(Resource.Id.btn_capture);
            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            list_data = FindViewById<ListView>(Resource.Id.list_data);
            list_data.ItemClick += async (s, e) =>
            {
                Document document = list_documentos[e.Position];
                selectedDoc = document;
                await LoadNombreReport();

                //CREA UN INTENT Y LE PASA TODOS LOS DATOS DEL DOCUMENTO SELECCIONADO A LA ACTIVIDAD DETAILDOC
                var openDetailIntent = new Intent(this, typeof(DetailDocActivity));
                openDetailIntent.PutExtra("CIFdoc", selectedDoc.CIF);
                openDetailIntent.PutExtra("nombreFiscalDoc", selectedDoc.Nombre_Fiscal);
                openDetailIntent.PutExtra("totalDoc", selectedDoc.Total);
                openDetailIntent.PutExtra("fechaDoc", selectedDoc.Fecha);
                openDetailIntent.PutExtra("comentarioDoc", selectedDoc.Comentarios);
                openDetailIntent.PutExtra("tipoGastoDoc", selectedDoc.Tipo_Gasto);
                openDetailIntent.PutExtra("idLiqDoc", selectedDoc.ID_Liquidacion);
                openDetailIntent.PutExtra("filenameDoc", selectedDoc.filename);
                openDetailIntent.PutExtra("idDocDoc", selectedDoc.IdDocumento);
                openDetailIntent.PutExtra("NombreReport", nombredelReport);

                StartActivity(openDetailIntent);
                Finish();
            };

            btnCapture.Click += delegate
            {
                StartActivity(typeof(CaptureActivity));
                    
                Finish();
            };

            await LoadData();
        }

        public async Task LoadData()
        {
            circular_progress.Visibility = ViewStates.Visible;
            list_data.Visibility = ViewStates.Invisible;
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("documents")
                .Child(auth.CurrentUser.Uid)
                .OnceAsync<Document>();
            list_documentos.Clear();
            adapter = null;
            foreach (var item in items)
            {
                Document document = new Document();
                document.CIF = item.Object.CIF;
                document.Nombre_Fiscal = item.Object.Nombre_Fiscal;
                document.Fecha = item.Object.Fecha;
                document.Total = item.Object.Total;
                document.Comentarios = item.Object.Comentarios;
                document.filename = item.Object.filename;
                document.Tipo_Gasto = item.Object.Tipo_Gasto;
                document.ID_Liquidacion = item.Object.ID_Liquidacion;
                document.IdDocumento = item.Object.IdDocumento;
                list_documentos.Add(document);
            }
            adapter = new LvDocAdapter(this, list_documentos);
            adapter.NotifyDataSetChanged();
            list_data.Adapter = adapter;
            circular_progress.Visibility = ViewStates.Invisible;
            list_data.Visibility = ViewStates.Visible;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_info)
            {
                InfoActi();
            }
            else
            if (id == Resource.Id.menu_settings)
            {
                SettingsActi();
            }
            else
            if (id == Resource.Id.menu_panel) 
            {

                PanelActi();
            }
            return base.OnOptionsItemSelected(item);
        }

        private void InfoActi()
        {
            StartActivity(typeof(InfoActivity));

            Finish();
        }

        private void SettingsActi()
        {
            StartActivity(typeof(SettingsActivity));

            Finish();
        }

        private void PanelActi()
        {
            var openAddToReportIntent = new Intent(this, typeof(PanelActivity));
            openAddToReportIntent.PutExtra("pantallaprevia", "main");
            StartActivity(openAddToReportIntent);
            Finish();
        }

        public async Task LoadNombreReport()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("reports")
                .Child(auth.CurrentUser.Uid)
                .OnceAsync<Report>();

            foreach (var item in items)
            {
                Report report = new Report();
                report.ID = item.Object.ID;
                report.Title = item.Object.Title;

                if (report.ID == selectedDoc.ID_Liquidacion)
                {
                    nombredelReport = report.Title;
                }
            }
        }
    }
}