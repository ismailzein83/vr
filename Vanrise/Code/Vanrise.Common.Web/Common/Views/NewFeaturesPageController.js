NewFeaturesController.$inject = ['$scope', 'UtilsService'];

function NewFeaturesController($scope, UtilsService) {

    $scope.nodeCategories = [
        { categoryId: 1, categoryName: "IMS", isGroup: true },
        { categoryId: 2, categoryName: "OLT", isGroup: true },
        { categoryId: 3, categoryName: "SLOT", isGroup: true },
        { categoryId: 4, categoryName: "Port", isGroup: false },
        { categoryId: 5, categoryName: "PortRj", isGroup: false }
    ];
    $scope.nodeDataArray = [
            { Id: 7, category: "IMS", Name: " IMS", isGroup: true },
            { Id: 8, category: "OLT", Name: " OLT", isGroup: true },
            { Id: 9, Parent: 7, category: "Port", get Name() { return this.Name = this.category + " " + this.Id; } },
            { Id: 12, Parent: 7, category: "Port", Name: " Port" },
            { Id: 11, Parent: 7, category: "Port", Name: " Port" },
            { Id: 10, Parent: 8, category: "Port", Name: " Port 2" },
            { Id: 13, Parent: 8, category: "Port", Name: " Port 3" },
            { Id: 14, Parent: 155, category: "PortRj", Name: " Port 4" },
            { Id: 15, Parent: 155, category: "PortRj", Name: " Port 5" },
            { Id: 164, Parent: 155, category: "PortRj", Name: " Port 6" },
            { Id: 165, Parent: 155, category: "PortRj", Name: " Port 7" },
            { Id: 167, Parent: 155, category: "PortRj", Name: " Port 8" },
            { Id: 168, Parent: 155, category: "PortRj", Name: " Port 9" },
            { Id: 155, Parent: 7, category: "SLOT", Name: "Slot 1", isGroup: true }
    ];

    var diagramApi;
    $scope.onDiagramReady = function (api) {
        diagramApi = api;
    };
   
    $scope.addDiagramNode = function () {
        var nodeType = $scope.selectedNodeType;
        var node = {
            id: UtilsService.replaceAll(UtilsService.guid(), '-', ''),
            category: nodeType.categoryName,
            Name: $scope.nodeName,
            isGroup: nodeType.isGroup
        };
        diagramApi.addNodeData(node);
    };

    $scope.addDiagramLink = function () {
        var nodefrom = $scope.fromselectedNode;
        var nodeto = $scope.toselectedNode;
        var node = {
            key: UtilsService.replaceAll(UtilsService.guid(), '-', ''),
            from: nodefrom.Id,
            to: nodeto.Id
        };
        diagramApi.addLinkData(node);
    };

}

appControllers.controller('Common_NewFeaturesController', NewFeaturesController);