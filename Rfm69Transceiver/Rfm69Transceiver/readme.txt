IMPORTANT: You need to do a manual step. See below.

When adding this NuGet package to your project there were 2 extra files added to your project. ~/NativeLibs/RaspberryPiRfm69Wrapper.o, and this readme.txt file
For this to work, you will need to manually mark the ~/NativeLibs/RaspberryPiRfm69Wrapper.o file to be output to the target directory. If you're using Visual Studio, right-click the file and click properties. Then make sure the Copy to Output Directory option is set to Copy Always or Copy If Newer.
You can then delete this readme.txt file. It is no longer needed.
