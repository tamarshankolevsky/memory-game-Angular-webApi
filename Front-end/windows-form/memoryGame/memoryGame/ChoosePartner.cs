using memoryGame.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;

namespace memoryGame
{
    public partial class ChoosePartner : Form
    {

        List<User> users;
        string whoChose = "";//the player who invites the partner
        public ChoosePartner()
        {
            users = GetUsersList();
            InitializeComponent();
        }

        private void ChoosePartner_Load(object sender, EventArgs e)
        {
            dataGridView_partnerList.DataSource = users.Select(c => new { c.UserName, c.Age }).ToList();
        }

        private void dataGridView_partnerList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            User partner = users[e.RowIndex];
            whoChose = GlobalProp.CurrentUser.UserName;
            //Request via put method to choose partner
            string url = $"{GlobalProp.Host}/user/setPartner";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(@url);

            string currentUserName = GlobalProp.CurrentUser.UserName;
            string partnerUserName = partner.UserName;
            var postData = $"currentUserName={currentUserName}";
            postData += $"&partnerUserName={partnerUserName}";
            var data = Encoding.ASCII.GetBytes(postData);

            httpWebRequest.Method = "PUT";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = data.Length;

            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            //Read response
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
            {
                string result = streamReader.ReadToEnd();
                //If request succeeded
                if (result.Contains("player1"))
                {
                    //Switch the screen to "startGame"
                    StartGame startGame = new StartGame(partner);
                    startGame.Show();
                    Close();
                }
                //Print the matching error
                else MessageBox.Show(result);

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Every 1000 ms.:

            //Update the shown userslist
            users = GetUsersList();
            dataGridView_partnerList.DataSource = users.Select(c => new { c.UserName, c.Age }).ToList();

            User user = GetUser(GlobalProp.CurrentUser.UserName);

            //If anyone chose me:
            if (user.PartnerUserName != null)
            {
                GlobalProp.CurrentUser = user;
                User partner = GetUser(GlobalProp.CurrentUser.PartnerUserName);
                partner.PartnerUserName = user.UserName;
                if (whoChose != GlobalProp.CurrentUser.UserName)
                {
                    StartGame startGame = new StartGame(partner);
                    startGame.Show();
                    Close();
                }
            }
        }

        public List<User> GetUsersList()
        {
            //Get request-GetUsers
            HttpClient client = new HttpClient();
            string url=$"{GlobalProp.Host}/user/getPartners";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(url).Result; 
            if (response.IsSuccessStatusCode)
            {
                var usersJson = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(usersJson);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return users;
        }

        public User GetUser(string username)
        {
            //Get request-GetCurrentUser
            User user = new User();
            HttpClient client = new HttpClient();
            string url = $"{GlobalProp.Host}/user/getUser/{username}";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var userJson = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(userJson);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return user;
        }
    }

}
    
