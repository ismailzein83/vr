using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace CoreWebStandardLib
{
    public class VRVirtualDirectoriesConfig : ConfigurationSection
    {
        public static VRVirtualDirectoriesConfig GetConfig()
        {
            return System.Configuration.ConfigurationManager.GetSection("vrvirtualDirectories") as VRVirtualDirectoriesConfig;
        }

        [System.Configuration.ConfigurationProperty("virtualDirectories")]
        [ConfigurationCollection(typeof(VRVirtualDirectoryCollection), AddItemName = "virtualDirectory")]
        public VRVirtualDirectoryCollection VirtualDirectories
        {
            get
            {
                return this["virtualDirectories"] as VRVirtualDirectoryCollection;
            }
        }

    }

    public class VRVirtualDirectoryCollection : ConfigurationElementCollection
    {

        public VRVirtualDirectory this[int index]
        {
            get
            {
                return base.BaseGet(index) as VRVirtualDirectory;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new VRVirtualDirectory this[string responseString]
        {
            get { return (VRVirtualDirectory)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new VRVirtualDirectory();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((VRVirtualDirectory)element).VirtualPath;
        }
    }

    public class VRVirtualDirectory : ConfigurationElement
    {
        [ConfigurationProperty("virtualPath", IsRequired = true)]
        public string VirtualPath
        {
            get
            {
                return this["virtualPath"] as string;
            }
        }
        
        [ConfigurationProperty("physicalPath", IsRequired = true)]
        public string PhysicalPath
        {
            get
            {
                return this["physicalPath"] as string;
            }
        }
    }
}
