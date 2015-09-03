app.constant('TrafficStatisticGroupKeysEnum', {
    OurZone: { title: "Zone", value: 0, description: "Our Zone", gridHeader: "Zone", isShownInGroupKey: true, filterProp: "zoneIds" },
    SupplierZoneId: { title: "Supplier Zone", value: 1, description: "Supplier Zone", gridHeader: "Supplier Zone", isShownInGroupKey: false, filterProp: "" },
    CustomerId: { title: "Customer", value: 2, description: "Customer", gridHeader: "Customer", isShownInGroupKey: true, filterProp: "customerIds" },
    SupplierId: { title: "Suppliers", value: 3, description: "Supplier", gridHeader: "Supplier", isShownInGroupKey: true, filterProp: "supplierIds" },
    CodeGroup: { title: "Code Group", value: 4, description: "Code Group", gridHeader: "Code Group", isShownInGroupKey: true, filterProp: "codeGroups" },
    Switch: { title: "Switch", value: 5, description: "Switch", gridHeader: "Switch", isShownInGroupKey: true, filterProp: "switchIds" },

    GateWayIn: { title: "GateWayIn", value: 6, description: "GateWayIn", gridHeader: "GateWay In", isShownInGroupKey: true, filterProp: "gateWayIn" },
    GateWayOut: { title: "GateWayOut", value: 7, description: "GateWayOut", gridHeader: "GateWay Out", isShownInGroupKey: true, filterProp: "gateWayOut" },

    PortIn: { title: "Port In", value: 8, description: "Port In", gridHeader: "Port In", isShownInGroupKey: true, filterProp: "portIn" },
    PortOut: { title: "Port Out", value: 9, description: "Port Out", gridHeader: "Port Out", isShownInGroupKey: true, filterProp: "portOut" },
   
    CodeSales: { title: "Code Sales", value: 10, description: "Code Sales", gridHeader: "Code Sales", isShownInGroupKey: true, filterProp: "codeSales" },
    CodeBuy: { title: "Code Buy", value: 11, description: "Code Buy", gridHeader: "Code Buy", isShownInGroupKey: true, filterProp: "codeBuy" }
    
    //OriginatingZoneId: { title: "OriginatingZoneId", value: 5, description: "Originating Zone", gridHeader: "Originating Zone", groupKeyEnumValue: 5, isShownInGroupKey: false },
   
   
  
});