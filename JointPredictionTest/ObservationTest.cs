using RedOwlConsulting.JointPrediction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RedOwlConsulting.JointPredictionTest
{
    
    
    /// <summary>
    ///This is a test class for ObservationTest and is intended
    ///to contain all ObservationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObservationTest
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
        ///A test for Observation Constructor
        ///</summary>
        [TestMethod()]
        public void ObservationConstructorTest()
        {
            Observation target = new Observation();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for ApproximateVelocity
        ///</summary>
        [TestMethod()]
        public void ApproximateVelocityTest()
        {
            DateTime dtg = DateTime.Now;
            Observation target = new Observation { DateTime = dtg, X = 1, Y = 1, Z = 1 };
            Observation pastObservation = new Observation { DateTime = dtg.AddSeconds(3), X = 0, Y = 1, Z = 2 };
            Velocity expected = new Velocity { DateTime = dtg, X = (-1F / 3F), Y = 0, Z = (1F / 3F) };
            Velocity actual;
            actual = target.ApproximateVelocity(pastObservation);
            Assert.IsTrue( Math.Abs(actual.X - expected.X) < 0.0001);
            Assert.IsTrue(Math.Abs(actual.Y - expected.Y) < 0.0001);
            Assert.IsTrue(Math.Abs(actual.Z - expected.Z) < 0.0001);
        }

        /// <summary>
        ///A test for TimeElapsed
        ///</summary>
        [TestMethod()]
        public void TimeElapsedTest()
        {
            DateTime dtg = DateTime.Now;
            Observation target = new Observation { DateTime = dtg.AddSeconds(3F) };
            Observation pastObservation = new Observation { DateTime = dtg };
            double expected = 3F;
            double actual;
            actual = target.TimeElapsed(pastObservation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DateTime
        ///</summary>
        [TestMethod()]
        public void DateTimeTest()
        {
            DateTime dtg = DateTime.Now;
            Observation target = new Observation { DateTime = dtg };
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
            Observation target = new Observation { X = 1F };
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
            Observation target = new Observation { Y = 2F };
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
            Observation target = new Observation { Z = -4F };
            double expected = -4F; 
            double actual;
            target.Z = expected;
            actual = target.Z;
            Assert.AreEqual(expected, actual);
        }
    }
}
