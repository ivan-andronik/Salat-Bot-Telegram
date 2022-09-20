using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace SalatBot.Scripts.Utilities
{
    public static class Utils
    {
        public static IEnumerable<string> GetNextChars(this string str, int iterateCount )
        {
            var words = new List<string>();

            for ( int i = 0; i < str.Length; i += iterateCount )
                words.Add(str.Length - i >= iterateCount
                    ? str.Substring(i, iterateCount)
                    : str.Substring(i, str.Length - i));

            return words;
        }

        public static List<int> GetRandomList(int count, int min, int max)
        {
            var list = new List<int>();
            
            for (int i = 0; i < count; i++)
            {
                int rand;
                do { rand = Random.Range(min, max); } while (list.Contains(rand));
                list.Add(rand);
            }
            return list;
        }
        
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int amount)
        {
            var enumerable = source.ToList();
            return enumerable.Skip(Math.Max(0, enumerable.Count - amount));
        }

        public static int GetDigitsAmount(int number)
        {
            var digits = new List<int>();
            do
            {   
                var k = number % 10;
                number /= 10;
                digits.Add(k);
            } while (number > 0);

            return digits.Count;
        }
    }
}