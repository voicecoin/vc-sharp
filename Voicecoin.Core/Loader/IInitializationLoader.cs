using System.Text;

namespace Voicecoin.Core.Loader
{
    /// <summary>
    /// Implement a customzied loader
    /// </summary>
    public interface IInitializationLoader
    {
        int Priority { get; }
        void Initialize();
    }
}
