NewFeaturesController.$inject = ['$scope', 'UtilsService', 'DiagramElementTypesEnum'];

function NewFeaturesController($scope, UtilsService, DiagramElementTypesEnum) {

    $scope.nodeCategories = [
        { categoryId: 1, categoryName: "IMS", hasChilds: true },
        { categoryId: 2, categoryName: "OLT", hasChilds: true },
        { categoryId: 3, categoryName: "SLOT", hasChilds: true },
        { categoryId: 4, categoryName: "Port", hasChilds: false },
        { categoryId: 5, categoryName: "PortRj", hasChilds: false }
    ];
    $scope.nodeDataArray = [
            { Id: 7, elementType: DiagramElementTypesEnum.IMS.value , Name: " IMS", hasChilds: true },
            { Id: 8, elementType:  DiagramElementTypesEnum.OLT.value, Name: " OLT", hasChilds: true },
            { Id: 9, Parent: 7, elementType: DiagramElementTypesEnum.Port.value, get Name() { return this.Name = DiagramElementTypesEnum.Port.description + " " + this.Id; } },
            { Id: 12, Parent: 7, elementType: DiagramElementTypesEnum.Port.value, Name: " Port" },
            { Id: 11, Parent: 7, elementType: DiagramElementTypesEnum.Port.value, Name: " Port" },
            { Id: 10, Parent: 8, elementType: DiagramElementTypesEnum.Port.value, Name: " Port 2" },
            { Id: 13, Parent: 8, elementType: DiagramElementTypesEnum.Port.value, Name: " Port 3" },
            { Id: 14, Parent: 155, elementType: DiagramElementTypesEnum.PortRj.value, Name: " Port 4" },
            { Id: 15, Parent: 155, elementType: DiagramElementTypesEnum.PortRj.value, Name: " Port 5" },
            { Id: 164, Parent: 155, elementType: DiagramElementTypesEnum.PortRj.value, Name: " Port 6" },
            { Id: 165, Parent: 155, elementType: DiagramElementTypesEnum.PortRj.value, Name: " Port 7" },
            { Id: 167, Parent: 155, elementType: DiagramElementTypesEnum.PortRj.value, Name: " Port 8" },
            { Id: 168, Parent: 155, elementType: DiagramElementTypesEnum.PortRj.value, Name: " Port 9" },
            { Id: 155, Parent: 7, elementType:DiagramElementTypesEnum.SLOT.value, Name: "Slot 1", hasChilds: true }
    ];

    var diagramApi;
    $scope.onDiagramReady = function (api) {
        diagramApi = api;
    };
   

}

appControllers.controller('Common_NewFeaturesController', NewFeaturesController);