using RedOwlConsulting.JointPrediction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RedOwlConsulting.JointPredictionTest
{
    
    
    /// <summary>
    ///This is a test class for VelocityTest and is intended
    ///to contain all VelocityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VelocityTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Velocity Constructor
        ///</summary>
        [TestMethod()]
        public void VelocityConstructorTest()
        {
            Velocity target = new Velocity();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for DateTime
        ///</summary>
        [TestMethod()]
        public void DateTimeTest()
        {
            DateTime dtg = DateTime.Now ;
            Velocity target = new Velocity { DateTime = dtg} ;
            DateTime expected = dtg;
            DateTime actual;
            target.DateTime = expected;
            actual = target.DateTime;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for X
        ///</summary>
        [TestMethod()]
        public void XTest()
        {
            Velocity target = new Velocity { X = 1F };
            double expected = 1F; 
            double actual;
            target.X = expected;
            actual = target.X;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Y
        ///</summary>
        [TestMethod()]
        public void YTest()
        {
            Velocity target = new Velocity { Y = 2F };
            double expected = 2F;
            double actual;
            target.Y = expected;
            actual = target.Y;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Z
        ///</summary>
        [TestMethod()]
        public void ZTest()
        {
            Velocity target = new Velocity { Z = 3F };
            double expected = 3F;
            double actual;
            target.Z = expected;
            actual = target.Z;
            Assert.AreEqual(expected, actual);
        }
    }
}
