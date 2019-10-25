﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public static class StaticBEDefinitionIDs
    {
        public static Guid CopperTechnology = new Guid("b12bd310-32c1-4afa-b311-d742b6070480");
        public static Guid FiberTechnology = new Guid("a1545bdd-2619-41fb-bc95-443f8407b612");

        public static Guid NodeBEDefinitionId = new Guid("c20d34be-9eda-4dcc-8872-41a415f38468");
        public static Guid ConnectionBEDefinitionId = new Guid("e74d9d5d-cc59-4b40-8626-048a755c054c");
        public static Guid NodePortBEDefinitionId = new Guid("04868fe5-9944-4e2b-b4d2-de9c5f73e2f4");
        public static Guid FDBBEDefinitionId = new Guid("70fb7242-b36b-4e04-8efc-47b8c6aea7f9");
        public static Guid DPBEDefinitionId = new Guid("dac8e0c4-0809-48cc-a050-d32af586af3f");

        public static Guid FreePortStatusDefinitionId = new Guid("a11d2835-89ed-442c-9646-c1f9b23ff213");
        public static Guid ReservedPortStatusDefinitionId = new Guid("c51bb41b-b31a-45ba-b12e-8f521b0323eb");

        public static Guid OLTNodeTypeId = new Guid("3f93d113-b5b8-4f75-b4d8-81dc6bc40cc8");
        public static Guid MDFTypeId = new Guid("c29902a5-a82f-4c4c-ae8a-84a1a405d8e6");
    }
}
