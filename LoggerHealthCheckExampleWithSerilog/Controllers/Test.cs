using System;

namespace LoggerHealthCheckExampleWithSerilog.Controllers
{
    public class Test
    {
        public static void Explode()
        {
            throw new Exception("Kabloui");
        }
    }
}
