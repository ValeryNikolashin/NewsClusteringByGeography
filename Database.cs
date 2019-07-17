using System.Collections.Generic;
using System.Data.SqlClient;

namespace NewsClusteringByGeography
{
    class Database
    {
        /// <summary>
        /// Добавляет список статей в БД.
        /// </summary>
        static public void AddArticlesToDatabase(List<Article> listOfArticles, string connectionString)
        {
            //Получаем статьи уже находящиеся в БД
            List<Article> listOfExistArticles = GetListOfArticlesFromDatabase(connectionString);
            //Добавляем НОВЫЕ статьи в БД
            for (int i = 0; i < listOfArticles.Count; i++)
            {
                //Проверяем, что статьи ещё нет в БД
                if (!Article.ExistArticle(listOfArticles[i], listOfExistArticles))
                {
                    string sqlExpression = "INSERT INTO article (title, main_link, city_id) VALUES ('" + listOfArticles[i].Title + "', '" + listOfArticles[i].MainLink + "', " + listOfArticles[i].CityId + ")\n";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.ExecuteNonQuery();
                    }
                    AddLinksToDatabase(listOfArticles[i].Links, connectionString);
                }
            }
        }

        /// <summary>
        /// Добавляет список ссылок статьи в БД.
        /// </summary>
        static public void AddLinksToDatabase(List<Link> listOfLinks, string connectionString)
        {
            //Вычисляем id последней добавленной статьи в БД, чтобы привязать ссылки к этой статье
            object id_article = 0;
            string sqlExpression = "SELECT * FROM article";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        id_article = reader.GetValue(0);
                    }
                }

                reader.Close();
            }
            //Добавляем ссылки в БД с привязкой к статье 
            for (int i = 0; i < listOfLinks.Count; i++)
            {
                sqlExpression = "INSERT INTO link (address, author, article_id) VALUES ('" + listOfLinks[i].Address + "', '" + listOfLinks[i].Author + "', " + id_article + ")";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получает из БД список статей соответствующих городу с id = cityId
        /// </summary>
        static public List<Article> GetListOfArticlesFromDatabase(long cityId, string connectionString)
        {
            string sqlExpression = "SELECT * FROM article WHERE city_id = " + cityId;
            List<Article> listOfArticles = new List<Article>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        Article article = new Article();
                        article.Title = reader.GetString(1);
                        article.MainLink = reader.GetString(2);
                        article.CityId = reader.GetInt64(3);
                        article.Links = GetListOfArticleLinksFromDatabase(reader.GetInt64(0), connectionString);
                        listOfArticles.Add(article);
                    }
                }
                reader.Close();
            }
            return listOfArticles;
        }

        /// <summary>
        /// Получает из БД список ссылок соответствующих статье с id = idArticle
        /// </summary>
        static public List<Link> GetListOfArticleLinksFromDatabase(long idArticle, string connectionString)
        {
            string sqlExpression = "SELECT * FROM link WHERE article_id = " + idArticle;
            List<Link> listOfLinks = new List<Link>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        Link link = new Link();
                        link.Address = reader.GetString(1);
                        link.Author = reader.GetString(2);
                        listOfLinks.Add(link);
                    }
                }
                reader.Close();
            }
            return listOfLinks;
        }

        /// <summary>
        /// Получает из БД список всех статей
        /// </summary>
        static public List<Article> GetListOfArticlesFromDatabase(string connectionString)
        {
            string sqlExpression = "SELECT * FROM article ";
            List<Article> listOfArticles = new List<Article>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        Article article = new Article();
                        article.Title = reader.GetString(1);
                        article.MainLink = reader.GetString(2);
                        article.CityId = reader.GetInt64(3);
                        article.Links = GetListOfArticleLinksFromDatabase(reader.GetInt64(0),connectionString);
                        listOfArticles.Add(article);
                    }
                }
                reader.Close();
            }
            return listOfArticles;
        }

        /// <summary>
        /// Получает из БД список всех городов России
        /// </summary>
        static public List<City> GetListOfRussianCitiesFromDatabase(string connectionString)
        {
            //connectionString = @"Data Source=LAPTOP-ADLJOG0N\SQLEXPRESS;Initial Catalog=Cities;Integrated Security=True";
            string sqlExpression = "SELECT * FROM dbo.russian_cities";
            List<City> listOfCities = new List<City>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        City city = new City();
                        city.Id = reader.GetInt64(0);
                        city.Name = reader.GetString(1);
                        listOfCities.Add(city);
                    }
                }
                reader.Close();
            }
            return listOfCities;
        }
    }
}
