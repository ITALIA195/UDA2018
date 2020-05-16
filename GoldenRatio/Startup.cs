using System.Reflection;

namespace GoldenRatio
{
    public static class Startup
    {
        public static Assembly Assembly => Assembly.GetExecutingAssembly();

        public static void Main()
        {
            using var window = new Window();
            window.Run();
        }
    }
}
