using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using FirebaseDatabase.Model;
using System.Collections.Generic;
using IVAPP303;


namespace IVAPP303
{
    public class LvLiqAdapter : BaseAdapter
    {
        Activity activityL;
        List<Report> lstreports;
        LayoutInflater inflater;

        public LvLiqAdapter(Activity activityL, List<Report> lstreports)
        {
            this.activityL = activityL;
            this.lstreports = lstreports;
        }

        public override int Count
        {
            get { return lstreports.Count; }
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
            inflater = (LayoutInflater)activityL.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View row = inflater.Inflate(Resource.Layout.ViewRowLiq, null);
            var txtTitle = row.FindViewById<TextView>(Resource.Id.list_Title);
            var txtAmount = row.FindViewById<TextView>(Resource.Id.list_Amount);
            if (lstreports.Count > 0)
            {
                txtTitle.Text = lstreports[position].Title;
                txtAmount.Text = lstreports[position].Amount + "€";
            }
            return row;
        }
    }
}