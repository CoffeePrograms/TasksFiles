using System.Collections.Concurrent;
using System.IO;
using System.Runtime;
using ThreadsFiles;

Handler handler = new(Directory.GetCurrentDirectory() + "\\Files");

// Создать файлы с данными
handler.MakeFiles();

// Сложить данные из файлов
var sum = handler.SumFiles();

Console.WriteLine(sum);
