using System.Collections.Generic;

namespace Amogus.Language.Resources
{
    public class Log
    {
        public List<object?> SystemOut { get; set; } = new List<object?>();
    }

    public static class LogResource
    {
        public static readonly List<object?> SystemOut = new List<object?>();
    }
}
