using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Ballance.Retry.Tests
{
    [TestFixture]
    public class FibonacciRetryTests
    {
        [Test]
        public void ShouldRunBaseCase_WhenCalledWithDefault()
        {
            var i = 0;
            var ret = new FibonacciRetry().Do<object>((() => $"Hello {i++}"));
            Assert.AreEqual("Hello 0", ret);
        }

        [Test]
        public void ShouldDoSimpleTask_WhenAsked()
        {
            var two = new FibonacciRetry().Do(() => 1+1);
            Assert.AreEqual(2, two);
           
        }


        [Test]
        public void ShouldReturnDefaultOfT_WhenDividingByZero()
        {
            var fibonacciRetry = new FibonacciRetry();
            var i = 0;
            Assert.AreEqual(default(object), fibonacciRetry.Do<object>(() => 27 / i));
        }

        internal static Random rand;

        /// <summary>
        /// Tries to find an even integer from a random input
        /// Rand is declared static above to prevent issues with proper seeing of the random number generator
        /// </summary>
        [Test]
        public void ShouldEventuallyFindAnEvenRandomInteger_WhenGivenRandomInteger()
        {
            
            var exceptionList = new List<Exception>();
            var evenNumberFound = new FibonacciRetry().Do(() =>
            {
                var randInt = rand.Next(0, 10000);
                if (randInt % 2 == 0)
                    return true;
                throw new ApplicationException($"Random int [{randInt}] was odd.");
            }, ref exceptionList, 100, 5);

            if (evenNumberFound)
            {
                Debug.WriteLine("Successfully found an even integer after {0} tries.", exceptionList.Count);
            }
            var exceptionsText = new StringBuilder();
            foreach (var exceptionString in exceptionList)
            {
                exceptionsText.Append(exceptionString);
            }
            var message = String.Format("Failed to find an even number [{0}] times.  Details: {1}", exceptionList.Count,
                exceptionsText);
            Assert.IsTrue(evenNumberFound, message);
        }

        [Test]
        public void ShouldPegCPU_ThenReturnTrue()
        {
            new FibonacciRetry().Do<bool>(() =>
            {
                var b = DateTime.Now.AddSeconds(15);
                System.Threading.Tasks.Parallel.For(0, 1 << 24, (i, state) =>
                {
                    if (DateTime.Now > b) state.Stop();
                });
                return true;
            });
        }

        private static int _tenTimeFailCounter = 10;

        [Test]
        public void ShouldFailTenTimesThenSucceed()
        {
            var exceptions = new List<Exception>();
            var trueAfterTenTries = new FibonacciRetry().Do<bool>(() =>
            {
                if (_tenTimeFailCounter-- > 0)
                {
                    throw new ApplicationException($"Counter is at {_tenTimeFailCounter+1}, thus greater than zero");
                }
                return true;
            }, ref exceptions, 11, 5);

            Assert.IsTrue(trueAfterTenTries, "Test delegate never succeeded");

            Assert.AreEqual(10, exceptions.Count);
        }
    }
}