using System;
using System.Windows.Forms;
using Plugin.RestClient;

namespace RJsimpson
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            WrongPassLabel.Visible = false;
            Login();
            Password.Text = "Password";

            Password.GotFocus += RemoveText;
            Password.LostFocus += AddText;
           // AnyAppBroadcaster anyAppBroadcaster = new AnyAppBroadcaster(this);
           // anyAppBroadcaster.Show();
            this.Hide();

        }
        public void RemoveText(object sender, EventArgs e)
        {
            Password.Text = "";
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password.Text))
                Password.Text = "Password";
        }

        #region Make Form Movable

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CAPTION = 0x2;

        #endregion

        #region Close and minimize Functions

        private void Fermer_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private string pass;
        #endregion
        public async void  Login()
        {
            RestClient<string> rest = new RestClient<string>();
            var passwordString = await rest.GetAsync();
            pass = passwordString;
            
        }
        private void Password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                pictureBox1_Click(null, null);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


            if(Password.Text != pass)
            {
                WrongPassLabel.Visible = true;
                Password.Text = "";
            }
            else
            {
                AnyAppBroadcaster anyAppBroadcaster = new AnyAppBroadcaster(this);
                anyAppBroadcaster.Show();
                this.Hide();
            }
        }
    }
}
