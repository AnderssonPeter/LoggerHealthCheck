#if !NET5
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Used to allow the usage of Record and init in non .net5.0
    /// </summary>
    internal static class IsExternalInit { }
}
#endif