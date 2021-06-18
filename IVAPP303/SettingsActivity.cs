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
using Android.Gms.Tasks;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Firebase.Auth;
using static Android.Views.View;

namespace IVAPP303
{
    [Activity(Label = "Settings", Theme = "@style/AppTheme")]
    class SettingsActivity : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        TextView txtWelcome;
        Button btnLogout;
        RelativeLayout activity_settings;
        FirebaseAuth auth;
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.dashboard_btn_logout)
                LogoutUser();
        }
        private void LogoutUser()
        {
            auth.SignOut();
            if (auth.CurrentUser == null)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
        }
        private void ChangePassword(string newPassword)
        {
            FirebaseUser user = auth.CurrentUser;
            user.UpdatePassword(newPassword)
            .AddOnCompleteListener(this);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            //View  
            txtWelcome = FindViewById<TextView>(Resource.Id.dashboard_welcome);
            btnLogout = FindViewById<Button>(Resource.Id.dashboard_btn_logout);
            activity_settings = FindViewById<RelativeLayout>(Resource.Id.activity_settings);
            btnLogout.SetOnClickListener(this);
            //Check Session  
            if (auth != null)
                txtWelcome.Text = "Sesión activa: " + auth.CurrentUser.Email;
        }
        public void OnComplete(Task task)
        {
        }


        public override void OnBackPressed()
        {
            StartActivity(typeof(LvDocActivity));

            Finish();
        }
    }
}
