using System;

namespace NewsClusteringByGeography
{
    class Link
    {
        string address;
        string author;

        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = String.Copy(value);
            }
        }

        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                author = String.Copy(value);
            }
        }

        public Link()
        {
            address = "";
            author = "";
        }

        public Link(string newAddres, string newAuthor)
        {
            address = String.Copy(newAddres);
            author = String.Copy(newAuthor);
        }

        override
        public string ToString()
        {
            return(author+", ссылка: "+address+"");
        }
    }
}
