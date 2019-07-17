using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace NewsClusteringByGeography
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ToFillComboBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnStartTimer.Enabled = false;
            btnStopTimer.Enabled = true;
            if (comboBox2.Text == "сек.")
            {
                int aw = Int32.Parse(numericUpDown1.Value.ToString());
                timer1.Interval = 1000 * Int32.Parse(numericUpDown1.Value.ToString());
            }
            else
            {
                int aw = Int32.Parse(numericUpDown1.Value.ToString());
                timer1.Interval = 60*1000 * Int32.Parse(numericUpDown1.Value.ToString());
            }
            timer1.Enabled = true;
            numericUpDown1.Enabled = false;
            comboBox2.Enabled = false;
            timer1.Start();
        }

        /// <summary>
        /// Возвращает xml страницы Google.News.
        /// </summary>
        static string GetResponse(string uri)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    sb.Append(Encoding.UTF8.GetString(buf, 0, count));
                }
            }
            while (count > 0);
            return sb.ToString();
        }

        /// <summary>
        /// Заполняет раскрывающийся список comboBox1 городами.
        /// </summary>
        private void ToFillComboBox()
        {
            List<City> listOfCities = Database.GetListOfRussianCitiesFromDatabase(textBox2.Text);
            List<string> listOfCitiesNames = new List<string>();
            for(int i = 0; i< listOfCities.Count; i++)
            {
                listOfCitiesNames.Add(listOfCities[i].Name);
            }
            listOfCitiesNames.Remove("Город не указан");
            listOfCitiesNames.Sort();
            for(int i = 0; i< listOfCitiesNames.Count; i++)
            {
                comboBox1.Items.Add(listOfCitiesNames[i]);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            btnStartTimer.Enabled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowArticles();
        }

        /// <summary>
        /// Отображает статьи в richTextBox1 в соответствии с выбранным городом в comboBox1
        /// </summary>
        private void ShowArticles()
        {
            long cityId = 1;
            //Получаем русские города из бд
            List<City> listOfCities = Database.GetListOfRussianCitiesFromDatabase(textBox2.Text);
            for(int i = 0; i<listOfCities.Count; i++)
            {
                if(listOfCities[i].Name==comboBox1.SelectedItem.ToString())
                {
                    cityId = listOfCities[i].Id;
                }
            }
            List<Article> listOfArticles = Database.GetListOfArticlesFromDatabase(cityId, textBox2.Text);
            richTextBox1.Text = Article.ListOfArticlesToString(listOfArticles);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //richTextBox1.Text +="Tick\n";
            Database.AddArticlesToDatabase(Article.GetListOfArticles(GetResponse(textBox1.Text), textBox2.Text), textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            btnStopTimer.Enabled = false;
            btnStartTimer.Enabled = true;
            numericUpDown1.Enabled = true;
            comboBox2.Enabled = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
