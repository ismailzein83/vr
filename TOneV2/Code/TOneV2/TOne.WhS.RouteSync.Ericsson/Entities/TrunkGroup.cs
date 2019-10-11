using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Ericsson
{
    public class TrunkGroup
	{
		public List<CustomerTrunkGroup> CustomerTrunkGroups { get; set; }

		public List<CodeGroupTrunkGroup> CodeGroupTrunkGroups { get; set; }

		public List<TrunkTrunkGroup> TrunkTrunkGroups { get; set; }

		public bool IsBackup { get; set; }

        public bool LoadSharing { get; set; }
    }

    public class CustomerTrunkGroup
	{
		public int CustomerId { get; set; }
	}

	public class CodeGroupTrunkGroup
	{
		public int CodeGroupId { get; set; }
	}

	public class TrunkTrunkGroup
	{
		public Guid TrunkId { get; set; }

		public int? Percentage { get; set; }

        public List<SupplierTrunkGroup> Backups { get; set; }
	}

    public class SupplierTrunkGroup
    {
        public int SupplierId { get; set; }

        public List<SupplierTrunk> Trunks { get; set; }
    }

    public class SupplierTrunk
    {
        public Guid TrunkId { get; set; } 
    }
}