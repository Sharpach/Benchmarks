using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Code;
using Helpers;

namespace DictionaryBenchmark
{
    public class DictionaryBenchmarkDotNet
    {
        private const int count = 3;
        private readonly Dictionary<int, string> _dictionary;

        public DictionaryBenchmarkDotNet()
        {
            _dictionary = new Dictionary<int, string>();
        }

        [ParamsSource(nameof(Parameters))]
        public BenchDictKeyValue KeyValue;

        public IEnumerable<IParam> Parameters()
        {
            for (int i = 0; i < count; i++)
            {
                var newVal = RandomTextGenerator.Generate(7);
                yield return new DictParams(new BenchDictKeyValue(i, newVal));
            }
        }

        public Dictionary<int, string> AddToDict(BenchDictKeyValue keyValue)
        {
            _dictionary[keyValue.Key]= keyValue.Value;
            return _dictionary;
        }

        [Benchmark]
        public void AddValue() => AddToDict(KeyValue);


    }

    public class DictParams : IParam
    {
        private readonly BenchDictKeyValue _value;

        public DictParams(BenchDictKeyValue value) => _value = value;

        public string ToSourceCode() => $"new BenchDictKeyValue({_value.Key}, \"{_value.Value}\")";

        public string DisplayText => $"({_value.Key}, \"{_value.Value}\")";

        public object Value => _value;
    }

    public class BenchDictKeyValue
    {
        public readonly int Key;
        public readonly string Value;

        public BenchDictKeyValue(int key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
