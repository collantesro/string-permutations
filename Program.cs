using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StringPermutations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Error.WriteLine("Starting program");
            RunTestCase("ROLANDO");
            RunTestCase("COLLANTES");

            // // Debugging QuickSort
            // byte[] myArr = new byte[] { 5, 2, 1, 3, 4 };
            // Span<byte> mySpan = myArr.AsSpan();
            // Console.WriteLine(string.Join("", myArr.Select(b => (char)(b + '0'))));
            // PermutationSpace.DoQuickSort(mySpan, 1);
            // Console.WriteLine(string.Join("", myArr.Select(b => (char)(b + '0'))));
        }

        private static void RunTestCase(string word) {
            Console.Error.WriteLine($"== Running test case \"{word}\"");
            Stopwatch watch = new Stopwatch();
            byte[] encodedWord = Encoding.ASCII.GetBytes(word);

            Console.Error.WriteLine("== Creating Permutation Space");
            PermutationSpace space = new PermutationSpace(encodedWord.Length);

            Console.Error.WriteLine("== Creating Permutations");
            watch.Restart();
            Permute(space, encodedWord);
            watch.Stop();
            Console.Error.WriteLine($"== Time to permute: {watch.Elapsed.TotalMilliseconds}ms");

            Console.Error.WriteLine("== Sorting & Printing");
            watch.Restart();
            space.PrintPermutations(printDuplicates: false);
            watch.Stop();
            Console.Error.WriteLine($"== Time to sort and print: {watch.Elapsed.TotalMilliseconds}ms");
        }

        private static void Permute(PermutationSpace space, byte[] word) {
            Permute(space, word, 0, word.Length - 1);
        }

        private static void Permute(PermutationSpace space, byte[] word, int start, int end) {
            if (start == end) {
                space.AddWord(word);
                return;
            }

            for (int i = start; i <= end; i++) {
                PermutationSpace.SwapWord(word, start, i, 1);
                Permute(space, word, start + 1, end);
                PermutationSpace.SwapWord(word, start, i, 1);
            }
        }
    }
}
