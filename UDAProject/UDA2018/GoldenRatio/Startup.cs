using System.Reflection;

namespace UDA2018.GoldenRatio
{
    public static class Startup
    {
        public static Assembly Assembly => Assembly.GetExecutingAssembly();

        public static void Main()
        {
            using (Window window = new Window())
                window.Run(60.0);
        }
    }
}
