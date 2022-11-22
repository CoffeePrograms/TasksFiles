using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsFiles
{
    public class Handler
    {
        static readonly Random rand = new();
        readonly string dir;
        readonly int nFilesOnThread = 4;
        int nFiles;
        
        public Handler(string dir)
        {
            this.dir = dir;
            nFiles = rand.Next(4, 160);
            //nFiles = rand.Next(4, 16);
        }

        private async void WriteFile(object path)
        {
            int nNumbers = rand.Next(20, 500);
            //int nNumbers = rand.Next(2, 5);
            using (StreamWriter sw = File.CreateText((string)path))
            {
                await sw.WriteLineAsync(nNumbers.ToString());
                foreach (var j in Enumerable.Range(1, nNumbers))
                {
                    await sw.WriteLineAsync(rand.Next(1, 100).ToString());
                }
            }
        }

        public void MakeFiles()
        {
            Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            {
                var range = Enumerable.Range(1, nFiles);

                Task[] tasks = new Task[nFiles];
                foreach (var i in range)
                {
                    string path = string.Format(dir + @"\{0}.txt", i);
                    tasks[i - 1] = Task.Run(() => WriteFile(path));
                }
                Task.WaitAll(tasks);
            }
        }

        private async Task<long> ReadFileAndSum(string path)
        {
            long sum = 0;
            using (StreamReader sr = File.OpenText(path))
            {
                string s = await sr.ReadLineAsync();
                int nNumbers = Convert.ToInt32(s);
                for (int j = 1; j <= nNumbers && ((s = await sr.ReadLineAsync()) != null); j++)
                {
                    sum += Convert.ToInt64(s);
                }
            }
            return sum;
        }

        private async Task<long> SumPartFiles(int iSplit)
        {
            string path;
            long sum = 0;
            int n = iSplit * nFilesOnThread;
            int j = (iSplit == 1) ? 1 : (n - nFilesOnThread + 1);
            if (n > nFiles)
                n = nFiles;

            for (int i = 0; j <= n; j++, i++)
            {
                path = string.Format(dir + @"\{0}.txt", j);
                //Console.WriteLine(path);
                sum += await ReadFileAndSum(path);
            }
            return sum;
        }

        public long SumFiles()
        {
            nFiles = Directory.GetFiles(dir).Length;
            long sum = 0;
            {
                var range = Enumerable.Range(1, nFiles);

                int nThreads = nFiles / nFilesOnThread + (nFiles % nFilesOnThread == 0 ? 0 : 1);
                if (nThreads == 0)
                    nThreads = 1;
                var tasksForSum = new Task<long>[nThreads];
                foreach (var iThread in Enumerable.Range(1, nThreads))
                {
                    tasksForSum[iThread - 1] = Task.Run(() => SumPartFiles(iThread));
                }
                Task.WaitAll(tasksForSum);

                sum += tasksForSum.Sum(l => l.Result);
            }
            return sum;
        }
    }
}
