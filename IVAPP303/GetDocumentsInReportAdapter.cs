using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using FirebaseDatabase.Model;
using System.Collections.Generic;
using IVAPP303;

namespace IVAPP303
{
    public class GetDocumentsInReportAdapter : BaseAdapter
    {
        Activity activity2;
        Activity activityLiq;
        List<Document> lstdocumentsinreport;
        LayoutInflater inflater2;

        public GetDocumentsInReportAdapter(Activity activity2, List<Document> lstdocumentsinreport)
        {
            this.activity2 = activity2;
            this.lstdocumentsinreport = lstdocumentsinreport;
        }

        public override int Count
        {
            get { return lstdocumentsinreport.Count; }
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            inflater2 = (LayoutInflater)activity2.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View row = inflater2.Inflate(Resource.Layout.ViewDocInReport, null);
            var txtNombreFiscal = row.FindViewById<TextView>(Resource.Id.doc_nombreFiscal);
            var txtTipoGasto = row.FindViewById<TextView>(Resource.Id.doc_tipoGasto);
            var txtFecha = row.FindViewById<TextView>(Resource.Id.doc_date);
            var txtImporte = row.FindViewById<TextView>(Resource.Id.doc_importe);
            if (lstdocumentsinreport.Count > 0)
            {
                txtNombreFiscal.Text = lstdocumentsinreport[position].Nombre_Fiscal;
                txtTipoGasto.Text = lstdocumentsinreport[position].Tipo_Gasto;
                txtFecha.Text = lstdocumentsinreport[position].Fecha;
                txtImporte.Text = lstdocumentsinreport[position].Total + "€";
            }
            return row;
        }
    }
}