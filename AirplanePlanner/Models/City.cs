using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;
using AirplanePlanner.Models;

namespace AirplanePlanner.Models
{
    public class City
    {
        private int _id;
        private string _name;

        public City(string name, int id = 0)
        {
            _name = name;
            _id = id;
        }

        public void SetId(int newId)
        {
            _id = newId;
        }

        public void SetName(string newName)
        {
            _name =newName;
        }

        public int GetId()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }

        public override bool Equals(System.Object otherCity)
        {
          if (!(otherCity is City))
          {
            return false;
          }
          else
          {
             City newCity = (City) otherCity;
             bool idEquality = this.GetId() == newCity.GetId();
             bool nameEquality = this.GetName() == newCity.GetName();
             return (idEquality && nameEquality);
           }
         }

         public override int GetHashCode()
         {
              return this.GetName().GetHashCode();
         }

         public static List<City> GetAll()
         {
            List<City> allCities = new List<City> {};
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM cities ORDER by name ASC;";
            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
              int cityId = rdr.GetInt32(0);
              string cityName = rdr.GetString(1);
              City newCity = new City(cityName, cityId);
              allCities.Add(newCity);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return allCities;
         }

         public static void DeleteAll()
         {

         }
    }
}
