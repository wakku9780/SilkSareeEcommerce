using System;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows DLL
            return LoadUnmanagedDllFromPath(absolutePath + "/libwkhtmltox.dll");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux so file
            return LoadUnmanagedDllFromPath(absolutePath + "/libwkhtmltox.so");
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported OS platform.");
        }
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        // Optional: Implement if you want to load based on unmanagedDllName
        return IntPtr.Zero;
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
