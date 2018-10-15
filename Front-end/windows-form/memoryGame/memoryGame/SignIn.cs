using memoryGame.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;


namespace memoryGame
{
    public partial class SignIn : Form
    {
        static HttpClient client = new HttpClient();
        public SignIn()
        {
            InitializeComponent();
        }


        private void btnSignIn_Click(object sender, EventArgs e)
        {
            //Try converting age.
            try
            {
                string userName = name.Text;
                int userAge = int.Parse(age.Text);
                //Validations 
                if (userAge < 18 || userAge > 120)
                {
                    MessageBox.Show("User age must be between 18 and 120.");
                    return;
                }
                if (userName.Length < 2 || userName.Length > 10)
                {
                    MessageBox.Show("User name must contains 2 - 10 chars.");
                    return;
                }
                // HTTP POST
                string url = $"{GlobalProp.Host}/user/signIn";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(@url);

                var postData = $"userName={userName}";
                postData += $"&age={userAge}";
                var data = Encoding.ASCII.GetBytes(postData);

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = data.Length;

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //Getting response

                    //If Login succeeded
                    if (responseString.Equals("true"))
                    {
                        ChoosePartner choosePartner = new ChoosePartner();
                        choosePartner.Show();
                        GlobalProp.CurrentUser = new User() { UserName = userName, Age = userAge };
                    }
                    //Printing the matching error
                    else MessageBox.Show(responseString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}



