using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;

namespace DictionaryBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestDefDict();
            var summary = BenchmarkRunner.Run<DictionaryBenchmarkDotNet>();
            //var summary = BenchmarkRunner.Run<IntroIParam>();
            Console.ReadLine();
        }

        static void TestIDict(int count)
        {
            Console.WriteLine("{");
            Dictionary<string, object> idict = new Dictionary<string, object>(count);

            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw.Start();
            List<string> keys=new List<string>();
            int magicNum = count / 4;
            for (int i = 0; i < count; i++)
            {
                
                string newKey = RandomTextGenerator.Generate(20);
                object val = i.ToString();
                sw2.Start();
                idict.Add(newKey,val );
                sw2.Stop();
                if (i% magicNum==0)
                {
                    keys.Add(newKey);
                }
            }
            Console.WriteLine($"\tFill time - {count} records: {sw.ElapsedMilliseconds} ms.");
            Console.WriteLine($"\tFill time - {count} records ('Add' calls only): {sw2.ElapsedMilliseconds} ms.");
            sw2.Stop();
            long kbytes=0;
            if (count>2000000)
            {
                kbytes = GC.GetTotalMemory(true) / (1024 * 1024);
            }
            else
            {
                kbytes = GetObjSize(idict) / (1024 * 1024);
            }

            Console.WriteLine($"\tSize: {kbytes} MBs.");

            object bufVal;
            foreach (var key in keys)
            {
                int  nanoseconds;
                Console.WriteLine("\t"+key + "{");
                sw.Restart();
                bufVal = idict.ContainsKey(key);
                sw.Stop();
                nanoseconds =(int) (sw.ElapsedTicks * 1000000000 / Stopwatch.Frequency);
                Console.WriteLine($"\t\tContains key operation: {nanoseconds} ns.");

                sw.Restart();
                bufVal = idict[key];
                sw.Stop();
                nanoseconds = (int)(sw.ElapsedTicks * 1000000000 / Stopwatch.Frequency) ;
                Console.WriteLine($"\t\tGet value operation: {nanoseconds} ns.");
                Console.WriteLine($"\t\tValue: {bufVal}");


                sw.Restart();
                bufVal = idict.First(kv => kv.Value == bufVal).Value;
                sw.Stop();
                nanoseconds = (int)(sw.ElapsedTicks * 1000000000 / Stopwatch.Frequency);
                Console.WriteLine($"\t\tFind by value with LINQ: {nanoseconds} ns.");

                sw.Restart();
                foreach(var kv in idict)
                {
                    if (kv.Value == bufVal)
                    {
                        bufVal = kv.Value;
                        break;
                    }
                }
                sw.Stop();
                nanoseconds = (int)(sw.ElapsedTicks * 1000000000 / Stopwatch.Frequency);
                Console.WriteLine($"\t\tFind by value with cycle: {nanoseconds} ns.");


                Console.WriteLine("\t}");
            }

            sw.Restart();
            bufVal = idict.OrderBy(k => k.Key);
            Console.WriteLine($"\tOrder operation: {sw.ElapsedMilliseconds} ms.");

            Console.WriteLine("}\n");

        }

        static void TestDefDict()
        {
            TestIDict(1000000);
            TestIDict(10000000);
        }


        static long GetObjSize(object o)
        {
            try
            {
                long size = 0;
                using (Stream s = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(s, o);
                    size = s.Length;
                }
                return size;
            }
            catch
            {
                return long.MinValue;
            }
        }
    }
}
