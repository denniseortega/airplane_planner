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
  }
}
