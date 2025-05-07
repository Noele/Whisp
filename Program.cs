// Project: Whisp

using NAudio.Wave;
using Vosk;

namespace Whisp
{
    public class Program
    {
        public static void Main(string[] args) => new Whisp.Whisp().Run(args).GetAwaiter().GetResult();
    }
}