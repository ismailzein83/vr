using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class VRWebContext
    {
        static IVRWebContextReader s_requestContextReader;

        public static void SetWebContextReader(IVRWebContextReader requestContextReader)
        {
            if (s_requestContextReader != null)
                throw new Exception("Request Context Reader already set");
            s_requestContextReader = requestContextReader;
        }

        public static bool IsInWebContext()
        {
            return s_requestContextReader != null;
        }

        public static string MapVirtualToPhysicalPath(string virtualPath)
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.MapVirtualToPhysicalPath(virtualPath);
        }

        public static string GetCurrentRequestHeader(string headerKey)
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestHeader(headerKey);
        }

        public static string GetCurrentRequestQueryString(string parameterName)
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestQueryString(parameterName);
        }

        public static VRWebCookie GetCurrentRequestCookie(string cookieName)
        {
            return GetCurrentRequestCookies().GetRecord(cookieName);
        }
        
        public static VRWebCookieCollection GetCurrentRequestCookies()
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestCookies();
        }

        public static string GetCurrentRequestBaseURL()
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestBaseURL();
        }

        public static VRURLParts GetCurrentRequestURLParts()
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestURLParts();
        }

        #region Disabled Code for Future use with .Net Core

        //static string s_rootWebPhysicalPath;

        //static SortedList<int, VRWebVirtualDirectory> s_virtualDirectories = new SortedList<int, VRWebVirtualDirectory>(new VRVirtualDirectoryComparer());

        //private class VRVirtualDirectoryComparer : IComparer<int>
        //{
        //    public int Compare(int x, int y)
        //    {
        //        return -x.CompareTo(y);
        //    }
        //}

        //public static string MapVirtualToPhysicalPath(string virtualPath)
        //{
        //    string rootVirtualPath;
        //    return MapVirtualToPhysicalPath(virtualPath, out rootVirtualPath);
        //}

        //public static string MapVirtualToPhysicalPath(string virtualPath, out string rootVirtualPath)
        //{
        //    s_rootWebPhysicalPath.ThrowIfNull("s_rootWebPhysicalPath");
        //    virtualPath = virtualPath.TrimStart('~');
        //    string physicalPath = Path.Combine(s_rootWebPhysicalPath, virtualPath.Replace("/", @"\").TrimStart('\\'));
        //    if (Utilities.PhysicalPathExists(physicalPath))
        //    {
        //        rootVirtualPath = "";
        //        return physicalPath;
        //    }

        //    foreach (var virtualDirectory in s_virtualDirectories.Values)
        //    {
        //        if (virtualPath.StartsWith(virtualDirectory.VirtualPath))
        //        {
        //            physicalPath = Path.Combine(virtualDirectory.PhysicalPath, virtualPath.Replace(virtualDirectory.VirtualPath, "").Replace("/", @"\").TrimStart('\\'));
        //            if (Utilities.PhysicalPathExists(physicalPath))
        //            {
        //                rootVirtualPath = virtualDirectory.VirtualPath;
        //                return physicalPath;
        //            }
        //        }
        //    }
        //    throw new Exception($"Could not Find Physical Path of Virtual Path '{virtualPath}");
        //}

        //public static void SetRootWebPhysicalPath(string rootWebPhysicalPath)
        //{
        //    s_rootWebPhysicalPath = rootWebPhysicalPath;
        //}

        //public static void AddVirtualDirectory(string virtualPath, string physicalPath)
        //{
        //    s_virtualDirectories.Add(virtualPath.Length, new VRWebVirtualDirectory
        //    {
        //        VirtualPath = virtualPath,
        //        PhysicalPath = physicalPath
        //    });
        //}

        #endregion
    }

    //public class VRWebVirtualDirectory
    //{
    //    public string VirtualPath { get; set; }

    //    public string PhysicalPath { get; set; }
    //}

    public interface IVRWebContextReader
    {
        string MapVirtualToPhysicalPath(string virtualPath);

        string GetCurrentRequestHeader(string headerKey);

        string GetCurrentRequestQueryString(string parameterName);

        VRWebCookieCollection GetCurrentRequestCookies();

        string GetCurrentRequestBaseURL();

        VRURLParts GetCurrentRequestURLParts();
    }

    public class VRWebCookie
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class VRURLParts
    {
        public string Scheme { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }

    public class VRWebCookieCollection : Dictionary<string, VRWebCookie>
    {
    }
}
