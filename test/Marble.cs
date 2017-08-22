using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickTest
{
    public class Marbles
    {
        private static readonly Random mRandom = new Random(); // use to generate random color marble

        public const int RED_MARBLE = 1;
        public const int BLUE_MARBLE = 2;
        public const int GREEN_MARBLE = 3;
        public const int ORANGE_MARBLE = 4;

        static void Main(string[] args)
        {
            int red, green, blue, orange;

            red = PromptInt("Ratio red marbles?");
            green = PromptInt("Ratio green marbles?");
            blue = PromptInt("Ratio blue marbles?");
            orange = PromptInt("Ratio orange marbles?");

            int[] results = Solve(red, green, blue, orange, 1000);
            WriteOutStats(results);
        }

        public static int PromptInt(string message)
        {
            int ret = -1;
            while (true)
            {
                Console.WriteLine(message);
                string str = Console.ReadLine();
                if (Int32.TryParse(str, out ret))
                    break;
                else
                    Console.WriteLine("'{0}' is invalid", str);
            }
            return ret;
        }

        public static int[] Solve(int red, int green, int blue, int orange, int count)
        {
            // TOOD: Return an array of integers of length [count]
            // each element in the array should contain a value from 1 to 4
            // the value represents a marble color (see constants above)
            // using the passed in values, you need to infer the probablility of each colored marble.
            // You should then *randomly* generate [count] number of marbles based on that probability

            // (i.e. if you were passed the values 10, 5, 5, 1 for the red, green, blue and orange parameters respectively
            // you should have approximately 10 red marbles for every 5 green and for every five blue marbles, and
            // there should 10 red marbles for approximately every orange marble you get)
            int[] array = new int[count];
            int[] probablility = { red, green, blue, orange };
            for (int i = 0; i < count; i++)
            {
                array[i] = RandomGenerateNumber(probablility);
            }
            return array;
        }

        public static int RandomGenerateNumber(int[] probablility)
        {
            int sum = 0;
            int random = mRandom.Next(0, probablility.Sum());
            int i = 0;
            foreach (int color in probablility)
            {
                sum += color;
                i++;
                if (random < sum)
                {
                    break;
                }
            }
            return i;
        }

        public static void WriteOutStats(int[] results)
        {
            // TODO: output the total number of red, green, blue and orange marbles based on the array of results passed into you.
            // This array is the same array you generated in the Solve function above.
            int[] counts = { 0, 0, 0, 0 };
            for (int i = 0; i < results.Length; i++)
            {
                counts[results[i] - 1]++;
            }
            Console.WriteLine("Red:" + counts[0]);
            Console.WriteLine("Blue:" + counts[1]);
            Console.WriteLine("Green:" + counts[2]);
            Console.WriteLine("Orange:" + counts[3]);
        }

    }
}