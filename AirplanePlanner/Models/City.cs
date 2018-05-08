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

         public void Save()
         {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO cities (name) VALUES (@cityname);";

            MySqlParameter name = new MySqlParameter();
            name.ParameterName = "@cityname";
            name.Value = this._name;
            cmd.Parameters.Add(name);

            cmd.ExecuteNonQuery();
            _id = (int) cmd.LastInsertedId;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
         }

         public void AddFlight(Flight newFlight)
         {
              MySqlConnection conn = DB.Connection();
              conn.Open();
              var cmd = conn.CreateCommand() as MySqlCommand;
              cmd.CommandText = @"INSERT INTO cities_flights (flight_id, city_id) VALUES (@FlightId, @CityId);";

              MySqlParameter flight_id = new MySqlParameter();
              flight_id.ParameterName = "@FlightId";
              flight_id.Value = newFlight.GetId();
              cmd.Parameters.Add(flight_id);

              MySqlParameter city_id = new MySqlParameter();
              city_id.ParameterName = "@CityId";
              city_id.Value = _id;
              cmd.Parameters.Add(city_id);

              cmd.ExecuteNonQuery();
              conn.Close();
              if (conn != null)
              {
                  conn.Dispose();
              }
         }

         public List<Flight> GetFlights()
         {
             MySqlConnection conn = DB.Connection();
             conn.Open();
             MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
             cmd.CommandText = @"SELECT flights.* FROM cities
                 JOIN cities_flights ON (cities.id = cities_flights.city_id)
                 JOIN flights ON (cities_flights.flight_id = flights.id)
                 WHERE cities.id = @CityId;";

             MySqlParameter cityIdParameter = new MySqlParameter();
             cityIdParameter.ParameterName = "@CityId";
             cityIdParameter.Value = _id;
             cmd.Parameters.Add(cityIdParameter);

             MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
             List<Flight> flights = new List<Flight>{};

             while(rdr.Read())
             {
               int flightId = rdr.GetInt32(0);
               string flightDeparture = rdr.GetString(1);
               string flightArrival = rdr.GetString(2);
               bool flightStatus = rdr.GetBoolean(3);
               bool flightCanceled = rdr.GetBoolean(4);
               Flight newFlight = new Flight(flightDeparture, flightArrival, flightStatus, flightCanceled, flightId);
               flights.Add(newFlight);
             }
             conn.Close();
             if (conn != null)
             {
                 conn.Dispose();
             }
             return flights;
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
         public static City Find(int id)
         {
             MySqlConnection conn = DB.Connection();
             conn.Open();
             var cmd = conn.CreateCommand() as MySqlCommand;
             cmd.CommandText = @"SELECT * FROM cities WHERE id = (@searchId);";

             MySqlParameter searchId = new MySqlParameter();
             searchId.ParameterName = "@searchId";
             searchId.Value = id;
             cmd.Parameters.Add(searchId);

             var rdr = cmd.ExecuteReader() as MySqlDataReader;
             int cityId = 0;
             string cityName = "";

             while(rdr.Read())
             {
               cityId = rdr.GetInt32(0);
               cityName = rdr.GetString(1);
             }

             City newCity = new City(cityName, cityId);

             conn.Close();
             if (conn != null)
             {
                 conn.Dispose();
             }

             return newCity;
         }
         public void Delete()
         {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM cities WHERE id = @CityId; DELETE FROM cities_flights WHERE city_id = @CityId;";

            MySqlParameter cityIdParameter = new MySqlParameter();
            cityIdParameter.ParameterName = "@CityId";
            cityIdParameter.Value = this.GetId();
            cmd.Parameters.Add(cityIdParameter);

            cmd.ExecuteNonQuery();
            if (conn != null)
            {
                conn.Close();
            }
         }

         public static void DeleteAll()
         {
             MySqlConnection conn = DB.Connection();
             conn.Open();
             var cmd = conn.CreateCommand() as MySqlCommand;
             cmd.CommandText = @"DELETE FROM cities;";
             cmd.ExecuteNonQuery();
             conn.Close();
             if (conn != null)
             {
                 conn.Dispose();
             }
         }
    }
}
