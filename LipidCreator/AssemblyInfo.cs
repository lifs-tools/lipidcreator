using System;
using System.Linq;
using System.Reflection;
namespace LipidCreator
{
    public static class AssemblyInfo
    {
        /// <summary> Gets the git hash value from the assembly
        /// or null if it cannot be found. </summary>
        public static string GetGitHash()
        {
            var asm = typeof(AssemblyInfo).Assembly;
            var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
            return attrs.FirstOrDefault(a => a.Key == "GitHash")?.Value;
        }

        /// <summary> Gets the build timestamp value from the assembly
        /// or null if it cannot be found. </summary>
        public static string GetBuildTime()
        {
            var asm = typeof(AssemblyInfo).Assembly;
            var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
            return attrs.FirstOrDefault(a => a.Key == "BuildTime")?.Value;
        }

        /// <summary> Gets the git branch name value from the assembly
        /// or null if it cannot be found. </summary>
        public static string GetGitBranch()
        {
            var asm = typeof(AssemblyInfo).Assembly;
            var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
            return attrs.FirstOrDefault(a => a.Key == "GitBranch")?.Value;
        }
    }
}
