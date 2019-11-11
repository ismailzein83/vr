using System;
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
        public static Guid PathConnectionBEDefinitionId = new Guid("24364ed5-1795-468c-a27b-c00013e830ac");
        public static Guid PathBEDefinitionId = new Guid("95DCF8AF-2273-4356-81E7-081034CCD75B");



        public static Guid FreePortStatusDefinitionId = new Guid("a11d2835-89ed-442c-9646-c1f9b23ff213");
        public static Guid ReservedPortStatusDefinitionId = new Guid("c51bb41b-b31a-45ba-b12e-8f521b0323eb");
        public static Guid UsedPortStatusDefinitionId = new Guid("e648730c-4a0c-4354-8c4e-5e0d8c34f855");
        public static Guid FaultyPortStatusDefinitionId = new Guid("dea67efd-d92b-4674-b982-4d6ba1bc6b10");


        public static Guid OLTNodeTypeId = new Guid("3f93d113-b5b8-4f75-b4d8-81dc6bc40cc8");
        public static Guid MDFTypeId = new Guid("c29902a5-a82f-4c4c-ae8a-84a1a405d8e6");

        public static Guid FDBNodeTypeId = new Guid("66b53c44-a373-420a-ae82-a18ee5be7b5c");
        public static Guid DPNodeTypeId = new Guid("a9d29ec3-f968-40b9-8238-1db51f6882f2");


        public static Guid SwitchTypeId = new Guid("8C3EA9B7-07EB-42CC-8B7D-78AACDFC8FF6");
        public static Guid DSLAMTypeId = new Guid("37EA9CA7-9099-4F9A-8279-8985B47591B5");

        public static Guid ReadyPathStatusDefinitionId = new Guid("a7815af4-e6d9-4dd0-bd1a-3f4b8b7b72d0");
        public static Guid DraftPathStatusDefinitionId = new Guid("d5618e4c-50f5-41bd-9b2e-c4e2d70d6715");
    }
}
