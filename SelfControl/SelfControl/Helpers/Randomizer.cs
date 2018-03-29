using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Helpers
{
    public class Randomizer
    {
        Random random;

        public Randomizer()
        {
            random = new Random();
        }

        public List<int> GetIndecies(int Count, int seed)
        {
            List<int> list = new List<int>(Count);
            for (int i = 0; i < Count; i++)
            {
                int number;

                do
                {
                    number = random.Next(seed);
                } while (list.Contains(number));

                list.Add(number);
            }
            return list;
        }
    }
}
