﻿using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public class RouteCase
    {
        public int RCNumber { get; set; }

        public string RouteCaseAsString { get; set; }
    }

    public class DeserializedRouteCase
    {
        public List<BranchRoute> BranchRoutes { get; set; }
        public List<RouteCaseOption> RouteCaseOptions { get; set; }
    }

    public class RORangeBRWithRouteCaseOptions
    {
        public RORangeBRWithRouteCaseOptions()
        {
            RouteCaseOptions = new List<RouteCaseOption>();
        }
        public RORangeBranchRoute BranchRoute { get; set; }
        public List<RouteCaseOption> RouteCaseOptions { get; set; }
    }

    public class BaseBRWithRouteCaseOptions
    {
        public BaseBRWithRouteCaseOptions()
        {
            RouteCaseOptions = new List<RouteCaseOption>();
        }
        public BaseBranchRoute BranchRoute { get; set; }
        public List<RouteCaseOption> RouteCaseOptions { get; set; }
    }

    public class BRWithRouteCaseOptions
    {
        public BRWithRouteCaseOptions()
        {
            RouteCaseOptions = new List<RouteCaseOption>();
        }
        public BranchRoute BranchRoute { get; set; }
        public List<RouteCaseOption> RouteCaseOptions { get; set; }
    }

    public class RouteCaseOption
    {
        public string SupplierId { get; set; }
        public int? Percentage { get; set; }
        public bool IsSwitch { get; set; }
        public string OutTrunk { get; set; }
        public TrunkType Type { get; set; }
        public int BNT { get; set; }
        public short SP { get; set; }
        public int? TrunkPercentage { get; set; }
        public bool IsBackup { get; set; }
        public int GroupID { get; set; }
    }
}