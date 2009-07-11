using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SolZipBasis;
using System.Windows.Forms;

namespace SolZipTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ZippingTest
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

        private static string m_SolutionFileName;
        private static string m_ZipFileName;


        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void ZippingTestClassInitialize(TestContext testContext) 
        {
            m_SolutionFileName = GetSolutionFileName();
            if (!File.Exists(m_SolutionFileName))
                throw new InvalidOperationException(string.Format("Test cannot initialize because {0} does not exist", m_SolutionFileName)); 

            m_ZipFileName = GetZipFileName();

            SolZipHelper.ZipSolution(m_ZipFileName, m_SolutionFileName, true, true); 
            
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void ZippingTestClassCleanup() 
        {
            File.Delete(m_ZipFileName);
        }

        private static string GetSolutionFileName()
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            DirectoryInfo solutionDir = currentDir.Parent.Parent.Parent;
            return Path.Combine(solutionDir.FullName, "SolZip.sln");
        }

        private static string GetZipFileName()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), string.Format("SolZipTest{0}.zip", Guid.NewGuid().ToString().Replace("{","").Replace("}","").Replace("-","")));
        }

        #endregion

        [TestMethod]
        public void ZipFileCreated()
        {
            Assert.IsTrue(File.Exists(m_ZipFileName), string.Format("The file {0} was not created", m_ZipFileName)); 
        }

        [TestMethod]
        public void ZipFileHasApproximatelyRightContent()
        {
            byte[] content = File.ReadAllBytes(m_ZipFileName);
            Assert.IsTrue(content.Length > 500000, string.Format("The file {0} is smaller than 500000 bytes", m_ZipFileName));
        }

        [TestMethod]
        public void ClipboardWorks()
        {
            const string something = "Hello";
            SolZipHelper.CopyFileNameToClipboard(something);
            Assert.AreEqual(something, Clipboard.GetText());
        }
    }
}
