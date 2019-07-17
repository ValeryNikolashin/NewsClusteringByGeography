using System;

namespace NewsClusteringByGeography
{
    class City
    {
        long id;
        string name;

        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = String.Copy(value);
            }
        }

        public City()
        {
            id = -2;
            name = "";
        }
    }
}
