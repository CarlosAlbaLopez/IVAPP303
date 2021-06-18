using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FirebaseDatabase.Model
{
    public class Document
    {
        public string CIF { get; set; }

        public string Nombre_Fiscal { get; set; }

        public string Tipo_Gasto { get; set; }

        public string Fecha { get; set; }

        public string Total { get; set; }

        public string Comentarios { get; set; }

        public string ID_Liquidacion { get; set; }

        public string filename { get; set; }

        public string IdDocumento { get; set; }
    }
}