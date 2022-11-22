using System.IO;
using ThreadsFiles;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string dir = @"..\..\..\..\..\" + @"ThreadsFiles\ThreadsFiles\bin\Debug\net6.0\Files";
            int nFiles = Directory.GetFiles(dir).Length;
            long sum = 0;
            var range = Enumerable.Range(1, nFiles);
            foreach (var i in range)
            {
                string path = string.Format(dir + @"\{0}.txt", i);
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = sr.ReadLine();
                    int nNumbers = Convert.ToInt32(s);
                    for (int j = 1; j <= nNumbers && ((s = sr.ReadLine()) != null); j++)
                    {
                        sum += Convert.ToInt64(s);
                    }
                }
            }
            Console.WriteLine(sum);
            Assert.AreEqual(sum, new Handler(dir).SumFiles());
        }
    }
}