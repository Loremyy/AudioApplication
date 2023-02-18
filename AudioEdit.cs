using System;

namespace AudioApplication
{
    class AudioEdit
    {
        public string CreateFileName()
        {
            Random random = new Random();
            var arr = new char[] { 'a', 'b', 'c', 'e', 'f', 'g', '1', '2', '3', '4', '5', '0' };
            string name = "";
            for (int i = 0; i < 6; i++)
            {
                int index = random.Next(arr.Length);
                name += arr[index];
            }
            return name;
        }
    }
}