using RedOwlConsulting.JointPrediction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RedOwlConsulting.JointPredictionTest
{
    
    
    /// <summary>
    ///This is a test class for JointPredictorTest and is intended
    ///to contain all JointPredictorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class JointPredictorTest
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
        ///A test for JointPredictor Constructor
        ///</summary>
        [TestMethod()]
        public void JointPredictorConstructorTest()
        {
            double decay = 2F;
            JointPredictor target = new JointPredictor(decay);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Update
        ///</summary>
        [TestMethod()]
        public void UpdateTest()
        {
            DateTime dtg = DateTime.Now;
            double decay = 0.5F;
            double weight = Math.Exp(-1 * decay);

            JointPredictor target = new JointPredictor(decay);
            Assert.IsNull(target.EwmaVelocity);

            Observation observation1 = new Observation { DateTime = dtg.AddSeconds(0.0), X=0, Y=0, Z=0 };
            target.Update(observation1);

            Assert.AreEqual(target.EwmaVelocity.DateTime, observation1.DateTime);
            Assert.AreEqual(target.EwmaVelocity.X, 0);
            Assert.AreEqual(target.EwmaVelocity.Y, 0);
            Assert.AreEqual(target.EwmaVelocity.Z, 0);

            Observation observation2 = new Observation { DateTime = dtg.AddSeconds(1.0), X=1, Y=-1, Z=0 };
            target.Update(observation2);

            Assert.AreEqual(target.EwmaVelocity.DateTime, observation2.DateTime);
            Assert.AreEqual(target.EwmaVelocity.X, weight);
            Assert.AreEqual(target.EwmaVelocity.Y, -1 * weight);
            Assert.AreEqual(target.EwmaVelocity.Z, 0);

            Observation observation3 = new Observation { DateTime = dtg.AddSeconds(2.0), X=1, Y=-1, Z=0 };
            target.Update(observation3);

            Assert.AreEqual(target.EwmaVelocity.DateTime, observation3.DateTime);
            Assert.AreEqual(target.EwmaVelocity.X, weight * (1-weight));
            Assert.AreEqual(target.EwmaVelocity.Y, -1 * weight * (1 - weight));
            Assert.AreEqual(target.EwmaVelocity.Z, 0);

            target.Update(observation1);
        }

        /// <summary>
        ///A test for DecayConstant
        ///</summary>
        [TestMethod()]
        public void DecayConstantTest()
        {
            double decay = 2F;
            JointPredictor target = new JointPredictor(decay); 
            double expected = 2F;
            double actual;
            target.DecayConstant = expected;
            actual = target.DecayConstant;
            Assert.AreEqual(expected, actual);
        }
    }
}
