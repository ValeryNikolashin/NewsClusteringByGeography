using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NewsClusteringByGeography
{
    class Article
    {
        string title;
        string mainLink;
        List<Link> links;
        string descrition;
        long cityId;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = String.Copy(value);
            }
        }

        public string MainLink
        {
            get
            {
                return mainLink;
            }
            set
            {
                mainLink = String.Copy(value);
            }
        }

        public List<Link> Links
        {
            get
            {
                return links;
            }
            set
            {
                links = value;
            }
        }

        public Link this[int index]
        {
            get
            {
                return links[index];
            }
            set
            {

                links[index] = new Link();
                links[index].Address = String.Copy(value.Address);
                links[index].Author = String.Copy(value.Author);
            }
        }

        public string Description
        {
            get
            {
                return descrition;
            }
            set
            {
                descrition = String.Copy(value);
            }
        }

        public long CityId
        {
            get
            {
                return cityId;
            }
            set
            {
                cityId = value;
            }
        }

        public void AddLink(Link newLink)
        {
            links.Add(newLink);
        }

        public Article()
        {
            title = "";
            mainLink = "";
            links = new List<Link>();
            descrition = "\n";
            cityId = 1;
        }

        /// <summary>
        /// Проверяет наличие статьи в списке.
        /// </summary>
        static public bool ExistArticle(Article article, List<Article> listOfArticles)
        {
            for (int i = 0; i < listOfArticles.Count; i++)
            {
                if (article.Title == listOfArticles[i].Title && article.MainLink == listOfArticles[i].MainLink)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Возвращает список статей из xml страницы Google.News.
        /// </summary>
        static public List<Article> GetListOfArticles(string xmlCode, string connectionString)
        {
            Regex regex1 = new Regex(@"<item>.*?</item>");
            Regex regex2 = new Regex(@"<title.*?</title>");
            Regex regex3 = new Regex(@"<description>.*?</description>");
            Regex regex4 = new Regex(@"&lt;li&gt;.*?&lt;/li&gt;");
            Regex regex5 = new Regex(@"href=.*? ");
            Regex regex6 = new Regex(@"&lt;font.*?&lt;/font");
            Regex regex7 = new Regex(@"<link>.*?</link>");
            MatchCollection matchesItems = regex1.Matches(xmlCode);
            List<Article> articles = new List<Article>();
            //для каждого найденного <item>, (т.е. для каждой новости)
            foreach (Match matchItem in matchesItems)
            {
                Article article = new Article();
                Match matchTitle = regex2.Match(matchItem.Value);
                article.Title = ClearTitle(matchTitle.Value);
                Match matchLink = regex5.Match(matchItem.Value);
                string mainLink = "";
                for (int i = 0; i < matchLink.Length; i++)
                {
                    if (matchLink.Value[i] == '\"')
                    {
                        for (int j = i + 1; j < matchLink.Length && matchLink.Value[j] != '\"'; j++)
                        {
                            mainLink += matchLink.Value[j];
                        }
                        break;
                    }
                }
                article.MainLink = mainLink;
                Match matchDescription = regex3.Match(matchItem.Value);
                MatchCollection matchesLi = regex4.Matches(matchDescription.Value);
                //для каждого найденного <li>
                foreach (Match matchLi in matchesLi)
                {
                    Match matchA = regex5.Match(matchLi.Value);
                    string addresOfLink = "";
                    for (int i = 0; i < matchA.Length; i++)
                    {
                        if (matchA.Value[i] == '\"')
                        {
                            for (int j = i + 1; j < matchA.Length && matchA.Value[j] != '\"'; j++)
                            {
                                addresOfLink += matchA.Value[j];
                            }
                            break;
                        }
                    }

                    Match matchFont = regex6.Match(matchLi.Value);
                    string authorOfLink = "";
                    for (int i = 0; i < matchFont.Length; i++)
                    {
                        if (i > 2 && matchFont.Value[i] == ';' && matchFont.Value[i - 1] == 't' && matchFont.Value[i - 2] == 'g')
                        {
                            for (int j = i + 1; j < matchFont.Length && matchFont.Value[j] != '&'; j++)
                            {
                                authorOfLink += matchFont.Value[j];
                            }
                            break;
                        }
                    }
                    //последней всегда идёт ссылка на "Взгляд с разных сторон в приложении Google Новости"
                    if (addresOfLink != "" && authorOfLink == "")
                    {
                        authorOfLink = "Взгляд с разных сторон в приложении \"Google Новости\"";
                    }
                    article.AddLink(new Link(addresOfLink, authorOfLink));
                }
                article.CityId = SearchCity(article.Title, connectionString);
                articles.Add(article);
            }
            return articles;
        }

        /// <summary>
        /// Очищает полученный заголовок от тега title.
        /// </summary>
        static string ClearTitle(string title)
        {
            string result = "";
            for (int i = 0; i < title.Length; i++)
            {
                if (title[i] == '>')
                {
                    for (int j = i + 1; j < title.Length && title[j] != '<'; j++)
                    {
                        result += title[j];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Переводит список статей в string.
        /// </summary>
        static public string ListOfArticlesToString(List<Article> listOfArticles)
        {
            string result = "";
            foreach (Article article in listOfArticles)
            {
                result += article.ToString() + "\n";
            }
            return result;
        }

        /// <summary>
        /// Ищет упоминание города в заголовке статьи.
        /// </summary>
        static long SearchCity(string title, string connectionString)
        {
            Regex regex;
            MatchCollection matchCollection;
            List<City> listOfCities = Database.GetListOfRussianCitiesFromDatabase(connectionString);
            for (int i = 0; i < listOfCities.Count; i++)
            {
                regex = new Regex(listOfCities[i].Name + @" (\w*)");
                matchCollection = regex.Matches(title);
                if (matchCollection.Count > 0)
                {
                    return listOfCities[i].Id;
                }
            }
            return 1;
        }

        override
        public string ToString()
        {
            string result = "";
            foreach(Link link in links)
            {
                result += link.ToString()+"\n";
            }
            return (title + "\n" + mainLink.ToString() + "\n" + result + descrition);
        }
    }
}
