using System.Linq;

namespace IdleFileCleaner {
    public static class AppSettings {

        public static readonly string SourcePaths = ConfigHelper.Read<string>("SourcePaths");
        public static readonly string DistinationPath = ConfigHelper.Read("DistinationPath");
        public static readonly string FileExtension = ConfigHelper.Read("FileExtension");
        public static readonly int IdleLimit = ConfigHelper.Read<int>("IdleLimit");
        public static readonly string Action = ConfigHelper.Read("Action");
        public static readonly int RetryInterval = ConfigHelper.Read<int>("RetryInterval");
        public static string[] GetSourcePaths() {
            return SourcePaths.Split(';').Select(v => v.Trim()).ToArray();            
        }
    }
}
