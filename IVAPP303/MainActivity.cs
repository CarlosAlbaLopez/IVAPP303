
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Firebase;
using Firebase.Auth;
using System;
using Android.Gms.Tasks;
using Android.Support.Design.Widget;
using FirebaseDatabase;
using static Android.Views.View;

namespace IVAPP303
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity, IOnClickListener, IOnCompleteListener
    {
        Button btnLogin;
        EditText input_email, input_password;
        TextView btnSignUp, btnForgetPassword;
        RelativeLayout activity_main;
        public static FirebaseApp app;
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            InitFirebaseAuth();

            auth = FirebaseAuth.GetInstance(app);

            FirebaseUser user = auth.CurrentUser;

            base.OnCreate(savedInstanceState);

            if (user != null)
            {
                StartActivity(new Android.Content.Intent(this, typeof(LvDocActivity)));
                Finish();
            }
            else
            {
                //prompt logg to the user

                //ESPECIFICAR VISTA PRINCIPAL
                SetContentView(Resource.Layout.main);

                //CONTROLES
                btnLogin = FindViewById<Button>(Resource.Id.login_btn_login);
                input_email = FindViewById<EditText>(Resource.Id.login_email);
                input_password = FindViewById<EditText>(Resource.Id.login_password);
                btnSignUp = FindViewById<TextView>(Resource.Id.login_btn_sign_up);
                btnForgetPassword = FindViewById<TextView>(Resource.Id.login_btn_forget_password);
                activity_main = FindViewById<RelativeLayout>(Resource.Id.activity_main);

                btnSignUp.SetOnClickListener(this);
                btnLogin.SetOnClickListener(this);
                btnForgetPassword.SetOnClickListener(this);
            }
        }

        private void InitFirebaseAuth()
        {
            var options = new FirebaseOptions.Builder()
               .SetApplicationId("1:1010972272943:android:7a912125b54eff5c")
               .SetApiKey("AIzaSyAwcbHOsNzZub1uL8EUKZWqELPK9T7fKac")
               .Build();
            if (app == null)
                app = FirebaseApp.InitializeApp(this, options, "IVAPP303");
            auth = FirebaseAuth.GetInstance(app);
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.login_btn_forget_password)
            {
                StartActivity(new Android.Content.Intent(this, typeof(ForgetPassword)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_sign_up)
            {
                StartActivity(new Android.Content.Intent(this, typeof(SignUp)));
                Finish();
            }
            else
               if (v.Id == Resource.Id.login_btn_login)
            {
                LoginUser(input_email.Text, input_password.Text);
            }
        }

        private void LoginUser(string email, string password)
        {
            auth.SignInWithEmailAndPassword(email, password).AddOnCompleteListener(this);
        }
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                StartActivity(new Android.Content.Intent(this, typeof(LvDocActivity)));
                Finish();
            }
            else
            {
                Snackbar snackbar = Snackbar.Make(activity_main, "Error", Snackbar.LengthShort);
                snackbar.Show();
            }
        }
    }
}

