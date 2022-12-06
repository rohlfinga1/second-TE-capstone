using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Transactions;
using 


namespace TenmoServer.Tests.DAO
{
    [TestClass]
    public class AccountDaoTests
    {
        private static readonly Account ACCOUNT_1 = new City(1, "City 1", "AA", 11, 111);
        private static readonly City CITY_2 = new City(2, "City 2", "BB", 22, 222);
        private static readonly City CITY_4 = new City(4, "City 4", "AA", 44, 444);

        private City testCity;

        private CitySqlDao dao;

        [TestInitialize] // arranges/sets-up before each test
        public override void Setup()
        {
            dao = new CitySqlDao(ConnectionString);
            testCity = new City(0, "Test City", "CC", 99, 999);
            base.Setup();
        }

        [TestMethod]
        public void GetCity_ReturnsCorrectCityForId() // can we get city based on city id
        {
            // "Act & Assert" usually happen in the test methods
            City city = dao.GetCity(1); // go interact with the mock DB
            AssertCitiesMatch(CITY_1, city); // we have an assert method written for us to check the conversion to C# data object

            city = dao.GetCity(2);
            AssertCitiesMatch(CITY_2, city);
        }
    }
}
