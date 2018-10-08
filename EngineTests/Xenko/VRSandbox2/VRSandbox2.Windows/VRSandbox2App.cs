using Xenko.Engine;

namespace VRSandbox2
{
    class VRSandbox2App
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
