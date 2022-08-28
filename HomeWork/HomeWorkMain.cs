using System;

namespace HomeWork
{
    public class HomeWorkMain
    {
        private static HomeWorks HomeWorks;

        static void Main(string[] args)
        {
        }

        static public void Start()
        {
            HomeWorks = new HomeWorks();
        }

        static public string GetHomeWorksString()
        {
            if (HomeWorks == null)
            {
                Start();
            }

            return HomeWorks.Get();
        }
    }
}
