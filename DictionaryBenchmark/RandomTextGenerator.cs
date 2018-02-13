using System;

namespace Helpers
{
    public static class RandomTextGenerator
    {
        public static char[] charsArr = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        static Random rnd = new Random();

        public static string Generate(int length)
        {
            char[] res = new char[length];
            for (int i = 0; i < length; i++)
                res[i] = charsArr[rnd.Next(0, charsArr.Length - 1)];
            return new string(res);

        }
    }
}
