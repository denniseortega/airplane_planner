using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using AirplanePlanner.Models;
using System;

namespace AirplanePlanner.TestTools
{
  [TestClass]
  public class CityTest : IDisposable
  {
    public CityTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=airplane_planner_test;";
    }
    public void Dispose()
    {
      City.DeleteAll();
      Flight.DeleteAll();
    }

    [TestMethod]
    public void GetAll_DatabaseEmptyAtFirst_0()
    {
      //Arrange, Act
      int result = City.GetAll().Count;

      //Assert
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Equals_TrueForSameDescription_Item()
    {
      //Arrange, Act
      City firstCity = new City("LA");
      City secondCity = new City("LA");

      //Assert
      Assert.AreEqual(firstCity, secondCity);
    }

    [TestMethod]
    public void Save_CitySavesToDatabase_CityList()
    {
      //Arrange
      City testCity = new City("LA");
      testCity.Save();

      //Act
      List<City> result = City.GetAll();
      List<City> testList = new List<City>{testCity};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void Find_FindsItemInDatabase_Item()
    {
      //Arrange
      City testCity = new City("LA");
      testCity.Save();

      //Act
      City result = City.Find(testCity.GetId());

      //Assert
      Assert.AreEqual(testCity, result);
    }

    [TestMethod]
    public void Delete_DeletesCityAssociationsFromDatabase_CityList()
    {
      //Arrange
      Flight testFlight = new Flight("05/14/18 8:46","05/14/18 12:56");
      testFlight.Save();

      string testName = "LA";
      City testCity = new City(testName);
      testCity.Save();

      //Act
      testCity.AddFlight(testFlight);
      testCity.Delete();

      List<City> resultFlightCities = testFlight.GetCities();
      List<City> testFlightCities = new List<City> {};

      //Assert
      CollectionAssert.AreEqual(testFlightCities, resultFlightCities);
    }
  }
}
