using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Android;
using Android.Support.V4.Content;
using Android.Content.PM;
using System;
using System.Threading.Tasks;
using FirebaseDatabase.Model;
using Android.Content;

namespace IVAPP303
{
    [Activity(Label = "DetailLiqActivity", Theme = "@style/AppTheme")]
    public class DetailLiqActivity : AppCompatActivity
    {
        private TextView detail_Title, detail_Comments, detail_Date, detail_Amount, detail_ID;
        private List<Report> list_reports = new List<Report>();
        public LvLiqAdapter adapter;
        public GetDocumentsInReportAdapter adapter2;
        private const string FirebaseURL = "https://ivapp303.firebaseio.com/"; //Firebase Database URL 
        FirebaseAuth auth;
        FirebaseClient firebase = new FirebaseClient("https://ivapp303.firebaseio.com/"); //Firebase Database URL
        private List<FirebaseDatabase.Model.Document> list_documentos_in_report = new List<FirebaseDatabase.Model.Document>();

        private ListView list_documents;
        private ProgressBar circular_progressLV;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.DetailLiq);

            //Add Toolbar  
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tbDetailLiq);
            toolbar.Title = "IVA303";
            SetSupportActionBar(toolbar);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //View   
            detail_Title = FindViewById<TextView>(Resource.Id.detail_Title);
            detail_Comments = FindViewById<TextView>(Resource.Id.detail_Comments);
            detail_Date = FindViewById<TextView>(Resource.Id.detail_Date);
            detail_Amount = FindViewById<TextView>(Resource.Id.detail_Amount);
            detail_ID = FindViewById<TextView>(Resource.Id.detail_ID);
            circular_progressLV = FindViewById<ProgressBar>(Resource.Id.circularProgressLV);
            list_documents = FindViewById<ListView>(Resource.Id.list_documents);

            string liqTitle = Intent.GetStringExtra("titleLiq");
            string liqComments = Intent.GetStringExtra("commentsLiq");
            string liqDate = Intent.GetStringExtra("dateLiq");
            string liqAmount = Intent.GetStringExtra("amountLiq");
            string liqId = Intent.GetStringExtra("idLiq");

            detail_Title.Text = liqTitle;
            detail_Comments.Text = liqComments;
            detail_Date.Text = "Fecha: " + liqDate;
            detail_Amount.Text = "Importe: " + liqAmount + "€";
            detail_ID.Text = liqId;

            _ = LoadData();
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
                DownloadReport();
            }
            else
            if (id == Resource.Id.menu_delete)
            {
                AskForConfirmation();
            }
            else
            if (id == Resource.Id.menu_add)
            {
                AddTickets();
            }
            return base.OnOptionsItemSelected(item);
        }

        private void DownloadReport()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                iTextSharp.text.Document pdfDocument = new iTextSharp.text.Document();

                var filename = "/sdcard/" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_") + auth.CurrentUser.Uid + ".pdf";

                PdfWriter.GetInstance(pdfDocument, new FileStream(filename, FileMode.Create));

                pdfDocument.Open();

                Chunk c = new Chunk("Expense report \n", FontFactory.GetFont("Roboto", 12));
                Chunk c1 = new Chunk("______________________________________________________________________________", FontFactory.GetFont("Roboto", 12));

                Paragraph p = new Paragraph();
                p.Add(c);
                p.Add(c1);

                Chunk c2 = new Chunk("Liquidación:          " + detail_Title.Text + "\n", FontFactory.GetFont("Roboto", 14));
                Chunk c3 = new Chunk("Nombre:               " + "\n", FontFactory.GetFont("Roboto", 14));
                Chunk c4 = new Chunk("Usuario:              " + auth.CurrentUser.Email + "\n", FontFactory.GetFont("Roboto", 14));
                Chunk c5 = new Chunk("Fecha:                " + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "\n", FontFactory.GetFont("Roboto", 14));
                Chunk c6 = new Chunk("Operación:            " + "\n" + "\n" + "\n", FontFactory.GetFont("Roboto", 14));

                Paragraph p1 = new Paragraph();
                p1.Add(c2);
                p1.Add(c3);
                p1.Add(c4);
                p1.Add(c5);
                p1.Add(c6);

                PdfPTable header = new PdfPTable(6);
                float[] risas = { 50f, 85f, 150f, 120f, 150f, 85f };
                header.SetWidthPercentage(risas, pdfDocument.PageSize);
                header.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
                header.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                header.DefaultCell.BorderColor = Color.BLACK;
                header.DefaultCell.BackgroundColor = Color.WHITE;
                header.DefaultCell.Border = Rectangle.BOX;
                header.DefaultCell.BorderWidth = 1;
                header.DefaultCell.Padding = 3;
                header.AddCell(new Phrase("ID"));
                header.AddCell(new Phrase("FECHA"));
                header.AddCell(new Phrase("PROVEEDOR"));
                header.AddCell(new Phrase("CATEGORÍA"));
                header.AddCell(new Phrase("COMENTARIOS"));
                header.AddCell(new Phrase("TOTAL"));
                
                PdfPTable table = new PdfPTable(6);
                table.SetWidthPercentage(risas, pdfDocument.PageSize);
                for (int i = 0; i < list_documentos_in_report.Count; i++)
                {
                    string tipogasto = list_documentos_in_report[i].Tipo_Gasto;
                    string totalgasto = list_documentos_in_report[i].Total;
                    string fechagasto = list_documentos_in_report[i].Fecha;
                    string nombreFgasto = list_documentos_in_report[i].Nombre_Fiscal;
                    string comentgasto = list_documentos_in_report[i].Comentarios;

                    table.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
                    table.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.BorderColor = Color.BLACK;
                    table.DefaultCell.BackgroundColor = Color.WHITE;
                    table.DefaultCell.Border = Rectangle.BOX;
                    table.DefaultCell.BorderWidth = 1;
                    table.DefaultCell.Padding = 3;
                    table.AddCell(new Phrase("#" + (i + 1)));
                    table.AddCell(new Phrase(fechagasto));
                    table.AddCell(new Phrase(nombreFgasto));
                    table.AddCell(new Phrase(tipogasto));
                    table.AddCell(new Phrase(comentgasto));
                    table.AddCell(new Phrase(totalgasto + "€"));
                }
                pdfDocument.Add(p);
                pdfDocument.Add(p1);
                pdfDocument.Add(header);
                pdfDocument.Add(table);                
                pdfDocument.Close();


                var file = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString(), filename);
                var uri = Android.Net.Uri.Parse(file);

                Intent intent = new Intent(Intent.ActionView);
                intent.SetFlags(ActivityFlags.ClearTop);
                intent.SetDataAndType(uri, "application/pdf");
                try
                {
                    StartActivity(intent);
                }
                catch (ActivityNotFoundException)
                {
                    Toast.MakeText(Application.Context, "Install a pdf viewer.", ToastLength.Long).Show();
                }
            }
            else
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 3);
            };
        }

        private void AddTickets()
        {
            StartActivity(typeof(AddTicketsToReport));
        }

        public void DeleteReport()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            firebase.Child("reports").Child(auth.CurrentUser.Uid).Child(Intent.GetStringExtra("idLiq")).DeleteAsync();
        }

        private void AskForConfirmation()
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("Mensaje de confirmación");
            builder.SetMessage("¿Desea eliminar esta liquidación?");
            builder.SetPositiveButton("SI", delegate
            {
                DeleteReport();
            });
            builder.SetNegativeButton("NO", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(PanelActivity));

            Finish();
        }

        public async Task LoadData()
        {
            circular_progressLV.Visibility = ViewStates.Visible;
            list_documents.Visibility = ViewStates.Invisible;
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("documents")
                .Child(auth.CurrentUser.Uid)
                .OnceAsync<FirebaseDatabase.Model.Document>();
            list_documentos_in_report.Clear();
            adapter2 = null;
            foreach (var item in items)
            {
                FirebaseDatabase.Model.Document document = new FirebaseDatabase.Model.Document();
                document.CIF = item.Object.CIF;
                document.Nombre_Fiscal = item.Object.Nombre_Fiscal;
                document.Fecha = item.Object.Fecha;
                document.Total = item.Object.Total;
                document.Comentarios = item.Object.Comentarios;
                document.filename = item.Object.filename;
                document.Tipo_Gasto = item.Object.Tipo_Gasto;
                document.ID_Liquidacion = item.Object.ID_Liquidacion;
                document.IdDocumento = item.Object.IdDocumento;
                if (document.ID_Liquidacion == Intent.GetStringExtra("idLiq"))
                {
                    list_documentos_in_report.Add(document);
                }
            }
            adapter2 = new GetDocumentsInReportAdapter(this, list_documentos_in_report);
            adapter2.NotifyDataSetChanged();
            list_documents.Adapter = adapter2;
            circular_progressLV.Visibility = ViewStates.Invisible;
            list_documents.Visibility = ViewStates.Visible;
        }
    }
}
