using Microsoft.VisualStudio.TestTools.UnitTesting;
using OutOfRange.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfRange.Utils.Tests
{
    [TestClass()]
    public class PointsUtilsTests
    {
        int actualRarity, expectedRarity;

        [TestMethod()]
        public void getCurrentBadgeRarityTest1()
        {
            actualRarity = PointsUtils.getCurrentScoreBadgeRarity(10);
            expectedRarity = 0;
            Assert.AreEqual(expectedRarity, actualRarity);
        }

        [TestMethod()]
        public void getCurrentBadgeRarityTest2()
        {
            actualRarity = PointsUtils.getCurrentScoreBadgeRarity(57);
            expectedRarity = 1;
            Assert.AreEqual(expectedRarity, actualRarity);
        }

        [TestMethod()]
        public void getCurrentBadgeRarityTest3()
        {
            actualRarity = PointsUtils.getCurrentScoreBadgeRarity(499);
            expectedRarity = 2;
            Assert.AreEqual(expectedRarity, actualRarity);
        }

        [TestMethod()]
        public void getCurrentBadgeRarityTest4()
        {
            actualRarity = PointsUtils.getCurrentScoreBadgeRarity(1024);
            expectedRarity = 3;
            Assert.AreEqual(expectedRarity, actualRarity);
        }


        [TestMethod()]
        public void getQuestionBadgeRarityTest1()
        {
            actualRarity = PointsUtils.getQuestionBadgeRarity(7);
            expectedRarity = -1;
            Assert.AreEqual(expectedRarity, actualRarity);
        }

        [TestMethod()]
        public void getQuestionBadgeRarityTest2()
        {
            actualRarity = PointsUtils.getQuestionBadgeRarity(1);
            expectedRarity = 0;
            Assert.AreEqual(expectedRarity, actualRarity);
        }

        [TestMethod()]
        public void getQuestionBadgeRarityTest3()
        {
            actualRarity = PointsUtils.getQuestionBadgeRarity(10);
            expectedRarity = 1;
            Assert.AreEqual(expectedRarity, actualRarity);
        }

        [TestMethod()]
        public void getQuestionBadgeRarityTest4()
        {
            actualRarity = PointsUtils.getQuestionBadgeRarity(100);
            expectedRarity = 2;
            Assert.AreEqual(expectedRarity, actualRarity);
        }

        [TestMethod()]
        public void getQuestionBadgeRarityTest5()
        {
            actualRarity = PointsUtils.getQuestionBadgeRarity(500);
            expectedRarity = 3;
            Assert.AreEqual(expectedRarity, actualRarity);
        }

    }
}