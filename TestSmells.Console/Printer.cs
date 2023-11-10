using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSmells.Console
{
    internal class Printer
    {
        private readonly TextWriter WriteOut;

        public Printer(string? outputPath = null)
        {
            if (outputPath == null)
            {
                WriteOut = System.Console.Out;
            }
            else
            {
                try
                {
                    if (File.Exists(outputPath))
                    {
                        File.Delete(outputPath);
                    }
                    FileStream fs = File.Create(outputPath);
                    var writer = new StreamWriter(fs);
                    //writer.AutoFlush = true;
                    WriteOut = writer;
                    System.Console.WriteLine($"\tWriting to {Path.GetFullPath(outputPath)}");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    Environment.Exit(1);
                }
            }
        }


        public void Print(IEnumerable<object> values)
        {
            foreach (var value in values)
            {
                WriteOut.WriteLine(value);
            }
            WriteOut.Flush();
        }

        public void Print(string value)
        {

            WriteOut.WriteLine(value);
            WriteOut.Flush();
        }



    }
}
