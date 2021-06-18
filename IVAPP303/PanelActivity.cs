using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System.Collections.Generic;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Firebase.Auth;
using Android.Content;
using System.Threading.Tasks;
using FirebaseDatabase.Model;
using System.Linq;

namespace IVAPP303
{
    [Activity(Label = "PanelActivity", Theme = "@style/AppTheme")]
    public class PanelActivity : AppCompatActivity
    {
        private Report selectedLiq;
        Button btnCreate;
        FirebaseAuth auth;
        private ListView list_info3;
        private ProgressBar circular_progress;
        private List<Report> list_reports = new List<Report>();
        public LvLiqAdapter adapter;
        private const string FirebaseURL = "https://ivapp303.firebaseio.com/";
        public string oldLiqComments;
        public string oldLiqDate;
        public string oldLiqTitle;
        public string oldLiqId;
        public string oldLiqAmount;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewLiq);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            string docCIF = Intent.GetStringExtra("CIFdoc");
            string docNombreF = Intent.GetStringExtra("nombreFiscalDoc");
            string docTotal = Intent.GetStringExtra("totalDoc");
            string docFecha = Intent.GetStringExtra("fechaDoc");
            string docComent = Intent.GetStringExtra("comentarioDoc");
            string docTipoG = Intent.GetStringExtra("tipoGastoDoc");
            string docIdLiq = Intent.GetStringExtra("idLiqDoc");
            string docFilename = Intent.GetStringExtra("filenameDoc");
            string docId = Intent.GetStringExtra("idDocDoc");
            string pantallaprevia = Intent.GetStringExtra("pantallaprevia");

            //View 
            btnCreate = FindViewById<Button>(Resource.Id.btn_create);

            if (pantallaprevia == "panel")
            {
                btnCreate.Visibility = ViewStates.Invisible;
            }
            else
            {

            }

            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            list_info3 = FindViewById<ListView>(Resource.Id.list_info3);
            list_info3.ItemClick += async (s, e) =>
            {
                if (pantallaprevia == "panel")
                {
                    Report report = list_reports[e.Position];
                    selectedLiq = report;
                    string liqComments = selectedLiq.Comments;
                    string liqDate = selectedLiq.Date;
                    string liqTitle = selectedLiq.Title;
                    await UpdateDoc(docCIF, docNombreF, docTotal, docFecha, docComent, docTipoG, docFilename, docId, selectedLiq, liqComments, liqDate, liqTitle);
                    if (oldLiqId == "")
                    {

                    }
                    else
                    {
                        await UpdateOldLiq(oldLiqComments, oldLiqDate, oldLiqTitle, oldLiqId);
                    }
                    StartActivity(typeof(PanelActivity));
                    Finish();
                }
                else
                {
                    Report report = list_reports[e.Position];
                    selectedLiq = report;

                    //CREA UN INTENT Y LE PASA TODOS LOS DATOS DEL REPORT SELECCIONADO A LA ACTIVIDAD DETAILLIQ
                    var openDetailLiqIntent = new Intent(this, typeof(DetailLiqActivity));
                    openDetailLiqIntent.PutExtra("titleLiq", selectedLiq.Title);
                    openDetailLiqIntent.PutExtra("commentsLiq", selectedLiq.Comments);
                    openDetailLiqIntent.PutExtra("dateLiq", selectedLiq.Date);
                    openDetailLiqIntent.PutExtra("amountLiq", selectedLiq.Amount);
                    openDetailLiqIntent.PutExtra("idLiq", selectedLiq.ID);

                    StartActivity(openDetailLiqIntent);
                    Finish();
                }
            };

            btnCreate.Click += delegate
            {
                StartActivity(typeof(NewReportActivity));
                Finish();
            };
            _ = LoadInfo();
        }

        public async Task UpdateDoc(string docCIF, string docNombreF, string docTotal, string docFecha, string docComent, string docTipoG, string docFilename, string docId, Report selectedLiq, string liqComments, string liqDate, string liqTitle)
        {
            decimal totalReport = decimal.Parse(selectedLiq.Amount);
            decimal totalDoc = decimal.Parse(docTotal);
            decimal totalTotalDecimal = totalReport + totalDoc;
            string totalTotal = totalTotalDecimal.ToString();

            var result = await AlertAsync(this, "Añadir gasto a liquidación", "¿Desea añadir el gasto a esta liquidación?", "Yes", "No");
            if (result == true)
            {
                var firebase = new FirebaseClient(FirebaseURL);
                var toUpdateDoc = (await firebase.Child("documents").Child(auth.CurrentUser.Uid).OnceAsync<Document>()).Where(a => a.Object.IdDocumento == docId).FirstOrDefault();
                var toUpdateLiq = (await firebase.Child("reports").Child(auth.CurrentUser.Uid).OnceAsync<Report>()).Where(a => a.Object.ID == selectedLiq.ID).FirstOrDefault();
                await firebase.Child("documents").Child(auth.CurrentUser.Uid).Child(toUpdateDoc.Key).PutAsync(new Document() { CIF = docCIF, Comentarios = docComent, Fecha = docFecha, ID_Liquidacion = selectedLiq.ID, IdDocumento = docId, Nombre_Fiscal = docNombreF, Tipo_Gasto = docTipoG, Total = docTotal, filename = docFilename });
                await firebase.Child("reports").Child(auth.CurrentUser.Uid).Child(toUpdateLiq.Key).PutAsync(new Report() { Amount = totalTotal, Comments = liqComments, Date = liqDate, ID = selectedLiq.ID, Title = liqTitle });
            }
        }

        public Task<bool> AlertAsync(Context context, string title, string message, string positiveButton, string negativeButton)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (var db = new Android.Support.V7.App.AlertDialog.Builder(context))
            {
                db.SetTitle(title);
                db.SetMessage(message);
                db.SetPositiveButton(positiveButton, (sender, args) => { tcs.TrySetResult(true); });
                db.SetNegativeButton(negativeButton, (sender, args) => { tcs.TrySetResult(false); });
                db.Show();
            }
            return tcs.Task;
        }

        public async Task UpdateOldLiq(string oldLiqComments, string oldLiqDate, string oldLiqTitle, string oldLiqId)
        {
            decimal totalReport = decimal.Parse(oldLiqAmount);
            string toSustract = Intent.GetStringExtra("totalDoc");
            decimal toSustractDecimal = decimal.Parse(toSustract);
            decimal totalSustractedDecimal = totalReport - toSustractDecimal;
            string totalSustracted = totalSustractedDecimal.ToString();

            var firebase = new FirebaseClient(FirebaseURL);
            var toUpdateOldLiq = (await firebase.Child("reports").Child(auth.CurrentUser.Uid).OnceAsync<Report>()).Where(a => a.Object.ID == oldLiqId).FirstOrDefault();
            await firebase.Child("reports").Child(auth.CurrentUser.Uid).Child(toUpdateOldLiq.Key).PutAsync(new Report() { Amount = totalSustracted, Comments = oldLiqComments, Date = oldLiqDate, ID = oldLiqId, Title = oldLiqTitle });
        }

        public async Task LoadInfo()
        {
            circular_progress.Visibility = ViewStates.Visible;
            list_info3.Visibility = ViewStates.Invisible;
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

                if (report.ID == Intent.GetStringExtra("idLiqDoc"))
                {
                    ////POR ESTAR ASIGNANDO AQUÍ UNOS VALORES, LA TASK UPDATEDOC NO CHUSCA, SALTA DEL BUILDER QUE MUESTRA EL MENSAJE (Y ANTES DE DEJARTE ELEGIR SI O NO) PASA A LA SIGUIENTE PARTE DEL CÓDIGO TRAS LA TASK UPDATEDOC
                    oldLiqComments = report.Comments;
                    oldLiqDate = report.Date;
                    oldLiqId = report.ID;
                    oldLiqTitle = report.Title;
                    oldLiqAmount = report.Amount;
                }
                else
                {
                    list_reports.Add(report);
                }
            }
            adapter = new LvLiqAdapter(this, list_reports);
            adapter.NotifyDataSetChanged();
            list_info3.Adapter = adapter;
            circular_progress.Visibility = ViewStates.Invisible;
            list_info3.Visibility = ViewStates.Visible;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(LvDocActivity));

            Finish();
        }
    }
}