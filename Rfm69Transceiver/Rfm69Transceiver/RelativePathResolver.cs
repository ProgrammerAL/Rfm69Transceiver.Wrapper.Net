using System;
using System.Collections.Generic;
using System.IO;
using NativeLibraryLoader;

namespace ProgrammerAl.HardwareSpecific.RF
{
    public class RelativePathResolver : PathResolver
    {
        private readonly string _libRelativeFolderPath;

        public RelativePathResolver(string libRelativeFolderPath) => _libRelativeFolderPath = libRelativeFolderPath;

        public override IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, _libRelativeFolderPath, name);
            yield return filePath;
        }
    }
}
