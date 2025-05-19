using System;
using System.Reflection;
using System.Runtime.Loader;
using System.IO;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    private IntPtr nativeLibraryPtr;

    public CustomAssemblyLoadContext() : base(true)
    {
    }

    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        nativeLibraryPtr = LoadUnmanagedDll(absolutePath);
        return nativeLibraryPtr;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllPath)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null; // No managed assemblies to load here
    }
}


//using System.Reflection;
//using System.Runtime.Loader;

//public class CustomAssemblyLoadContext : AssemblyLoadContext
//{
//    public IntPtr LoadUnmanagedLibrary(string absolutePath)
//    {
//        return LoadUnmanagedDll(absolutePath);
//    }

//    protected override IntPtr LoadUnmanagedDll(string unmanagedDllPath)
//    {
//        return LoadUnmanagedDllFromPath(unmanagedDllPath);
//    }

//    protected override Assembly Load(AssemblyName assemblyName)
//    {
//        return null!;
//    }
//}
