using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;
using AirplanePlanner.Models;

namespace AirplanePlanner.Models
{
  public class Flight
  {
    private string _departure;
    private string _arrival;
    private bool _status;
    private bool _canceled;
    private int _id;

    public Flight (string departure, string arrival, bool status = true, bool canceled = false, int id = 0)
    {
      _departure = departure;
      _arrival = arrival;
      _status = status;
      _canceled = canceled;
      _id = id;
    }
    public override bool Equals(System.Object otherFlight)
    {
      if (!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
         Flight newFlight = (Flight) otherFlight;
         bool idEquality = this.GetId() == newFlight.GetId();
         bool departureEquality = this.GetDeparture() == newFlight.GetDeparture();
         bool arrivalEquality = this.GetArrival() == newFlight.GetArrival();
         bool statusEquality = this.GetStatus() == newFlight.GetStatus();
         bool canceledEquality = this.GetCanceled() == newFlight.GetCanceled();

         return (idEquality && departureEquality && arrivalEquality && statusEquality && canceledEquality);
       }
    }
    // public override int GetHashCode()
    // {
    //      return this.GetDeparture().GetHashCode();
    // }
    public void SetDeparture(string newDeparture)
    {
      _departure = newDeparture;
    }
    public void SetArrival(string newArrival)
    {
      _arrival = newArrival;
    }
    public void SetStatus(bool newStatus)
    {
      _status = newStatus;
    }
    public void SetCanceled(bool maybeCanceled)
    {
      _canceled = maybeCanceled;
    }
    public void SetId(int newId)
    {
      _id = newId;
    }
    public string GetDeparture()
    {
      return _departure;
    }
    public string GetArrival()
    {
      return _arrival;
    }
    public  bool GetStatus()
    {
      return _status;
    }
    public bool GetCanceled()
    {
      return _canceled;
    }
    public int GetId()
    {
      return _id;
    }

    public void Save()
    {
       MySqlConnection conn = DB.Connection();
       conn.Open();

       var cmd = conn.CreateCommand() as MySqlCommand;
       cmd.CommandText = @"INSERT INTO flights (departure, arrival, status, canceled) VALUES (@FlightDeparture, @FlightArrival, @FlightStatus, @FlightCanceled);";

       MySqlParameter departure = new MySqlParameter();
       departure.ParameterName = "@FlightDeparture";
       departure.Value = this._departure;
       cmd.Parameters.Add(departure);

       MySqlParameter status = new MySqlParameter();
       status.ParameterName = "@FlightStatus";
       status.Value = this._status;
       cmd.Parameters.Add(status);

       MySqlParameter arrival = new MySqlParameter();
       arrival.ParameterName = "@FlightArrival";
       arrival.Value = this._arrival;
       cmd.Parameters.Add(arrival);

       MySqlParameter canceled = new MySqlParameter();
       canceled.ParameterName = "@FlightCanceled";
       canceled.Value = this._canceled;
       cmd.Parameters.Add(canceled);

       cmd.ExecuteNonQuery();
       _id = (int) cmd.LastInsertedId;
       conn.Close();
       if (conn != null)
       {
           conn.Dispose();
       }
    }
    public void AddCity(City newCity)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"INSERT INTO cities_flights (city_id, flight_id) VALUES (@CityId, @FlightId);";

        MySqlParameter city_id = new MySqlParameter();
        city_id.ParameterName = "@CityId";
        city_id.Value = newCity.GetId();
        cmd.Parameters.Add(city_id);

        MySqlParameter flight_id = new MySqlParameter();
        flight_id.ParameterName = "@FlightId";
        flight_id.Value = _id;
        cmd.Parameters.Add(flight_id);

        cmd.ExecuteNonQuery();
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
    }

    public List<City> GetCities()
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT cities.* FROM flights
            JOIN cities_flights ON (flights.id = cities_flights.flight_id)
            JOIN cities ON (cities_flights.city_id = cities.id)
            WHERE flights.id = @FlightId;";

        MySqlParameter flightIdParameter = new MySqlParameter();
        flightIdParameter.ParameterName = "@FlightId";
        flightIdParameter.Value = _id;
        cmd.Parameters.Add(flightIdParameter);

        MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
        List<City> cities = new List<City>{};

        while(rdr.Read())
        {
          int cityId = rdr.GetInt32(0);
          string cityName = rdr.GetString(1);
          City newCity = new City(cityName,cityId);
          cities.Add(newCity);
        }
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return cities;
   }

    public static List<Flight> GetAll()
    {
       List<Flight> allFlights = new List<Flight> {};
       MySqlConnection conn = DB.Connection();
       conn.Open();
       var cmd = conn.CreateCommand() as MySqlCommand;
       cmd.CommandText = @"SELECT * FROM flights;";
       var rdr = cmd.ExecuteReader() as MySqlDataReader;
       while(rdr.Read())
       {
         int flightId = rdr.GetInt32(0);
         string flightDeparture = rdr.GetString(1);
         string flightArrival = rdr.GetString(2);
         bool flightStatus = rdr.GetBoolean(3);
         bool flightCanceled = rdr.GetBoolean(4);
         Flight newFlight = new Flight(flightDeparture, flightArrival, flightStatus, flightCanceled, flightId);
         allFlights.Add(newFlight);
       }
       conn.Close();
       if (conn != null)
       {
           conn.Dispose();
       }
       return allFlights;
    }
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM flights;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn !=null)
      {
        conn.Dispose();
      }
    }
  }
}
