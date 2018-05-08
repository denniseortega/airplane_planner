using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using AirplanePlanner.Models;
using System;

namespace AirplanePlanner.TestTools
{
  [TestClass]
  public class FlightTests: IDisposable
  {
    public void FlightTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=airplane_planner_test;";
    }
    public void Dispose()
    {
      City.DeleteAll();
      Flight.DeleteAll();
    }

    [TestMethod]
    public void GetAll_DbStartsEmpty_0()
    {
        //Arrange
        //Act
        int result = Flight.GetAll().Count;

        //Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Test_AddCity_AddsCityToCategory()
    {
      //Arrange
      Flight testFlight = new Flight("05/14/18 8:46", "05/14/18 12:56");
      testFlight.Save();

      City testCity = new City("LA");
      testCity.Save();

      City testCity2 = new City("NY");
      testCity2.Save();

      //Act
      testFlight.AddCity(testCity);
      testFlight.AddCity(testCity2);

      List<City> result = testFlight.GetCities();
      List<City> testList = new List<City>{testCity, testCity2};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }
 }
}
