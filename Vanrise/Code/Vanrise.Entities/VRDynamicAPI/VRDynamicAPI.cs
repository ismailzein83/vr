﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPI
    {
        public long VRDynamicAPIId { get; set; }
        public string Name { get; set; }
        public int ModuleId{ get; set; }
        public VRDynamicAPISettings Settings { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
    }
    public class VRDynamicAPISettings
    {
        public List<VRDynamicAPIMethod> Methods { get; set; }
    }
    public class VRDynamicAPIMethod
    {
        public Guid VRDynamicAPIMethodId { get; set; }
        public string Name { get; set; }
        public VRDynamicAPIMethodSettings Settings { get; set; }
    }
    public abstract class VRDynamicAPIMethodSettings
    {
        public abstract Guid ConfigId { get; }
    }
    public enum VRDynamicAPIMethodType { Get = 1, Post = 2 }
    public class VRDynamicAPIMethodParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
    }
}
