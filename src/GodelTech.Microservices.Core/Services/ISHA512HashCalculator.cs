using System.IO;

namespace GodelTech.Microservices.Core.Services
{
    public interface ISha512HashCalculator
    {
        string ComputeHash(Stream content);
    }
}