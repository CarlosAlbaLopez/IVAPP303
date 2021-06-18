using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using FirebaseDatabase.Model;

namespace IVAPP303
{
    public class FirebaseHelper
    {
        FirebaseClient firebase = new FirebaseClient("https://ivapp303.firebaseio.com/"); //Firebase Database URL 
        FirebaseAuth auth;

        public async Task<List<Report>> GetAllReports()
        {
            return (await firebase.Child("report").Child(auth.CurrentUser.Uid).OnceAsync<Report>()).Select(item => new Report
            {
                ID = item.Object.ID,
                Title = item.Object.Title,
                Comments = item.Object.Comments,
                Amount = item.Object.Amount,
                Date = item.Object.Date
            }).ToList();
        }

        public async Task<Report> GetReport(string id)
        {
            var allReports = await GetAllReports();
            await firebase.Child("reports").Child(auth.CurrentUser.Uid).OnceAsync<Report>();
            return allReports.Where(a => a.ID == id).FirstOrDefault();
        }

        public async Task<List<Document>> GetAllDocuments()
        {
            return (await firebase.Child("documents").Child(auth.CurrentUser.Uid).OnceAsync<Document>()).Select(item => new Document
            {
                CIF = item.Object.CIF,
                Comentarios = item.Object.Comentarios,
                Fecha = item.Object.Fecha,
                ID_Liquidacion = item.Object.ID_Liquidacion,
                IdDocumento = item.Object.IdDocumento,
                Nombre_Fiscal = item.Object.Nombre_Fiscal,
                Tipo_Gasto = item.Object.Tipo_Gasto,
                Total = item.Object.Total,
                filename = item.Object.filename
            }).ToList();
        }

        public async Task<List<Document>> GetDocumentsInReport(string idReport)
        {
            var allDocuments = await GetAllDocuments();
            await firebase.Child("documents").Child(auth.CurrentUser.Uid).OnceAsync<Document>();
            return allDocuments.Where(a => a.ID_Liquidacion == idReport).ToList();
        }
    }
}