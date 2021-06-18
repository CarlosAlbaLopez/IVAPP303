using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using FirebaseDatabase.Model;
using System.Collections.Generic;
using IVAPP303;

namespace IVAPP303
{
    public class LvDocAdapter : BaseAdapter
    {
        Activity activity;
        List<Document> lstdocuments;
        LayoutInflater inflater;

        public LvDocAdapter(Activity activity, List<Document> lstdocuments)
        {
            this.activity = activity;
            this.lstdocuments = lstdocuments;
        }
        public override int Count
        {
            get { return lstdocuments.Count; }
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
            inflater = (LayoutInflater)activity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View row = inflater.Inflate(Resource.Layout.ViewRow, null);
            var txtNombreFiscal = row.FindViewById<TextView>(Resource.Id.list_nombreFiscal);
            var txtTipoGasto = row.FindViewById<TextView>(Resource.Id.list_tipoGasto);
            var txtFecha = row.FindViewById<TextView>(Resource.Id.list_date);
            var txtImporte = row.FindViewById<TextView>(Resource.Id.list_importe);
            if (lstdocuments.Count > 0)
            {
                txtNombreFiscal.Text = lstdocuments[position].Nombre_Fiscal;
                txtTipoGasto.Text = lstdocuments[position].Tipo_Gasto;
                txtFecha.Text = lstdocuments[position].Fecha;
                txtImporte.Text = lstdocuments[position].Total + "€";
            }
            return row;
        }
    }
}