"use strict";

app.directive("vrWhsSalesCodecompareGrid", ["UtilsService", "VRNotificationService", "WhS_Sales_CodeCompareAPIService", "WhS_Sales_CodeCompareService",
function (UtilsService, VRNotificationService, WhS_Sales_CodeCompareAPIService, WhS_Sales_CodeCompareService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var codeCompareGrid = new CodeCompareGrid($scope, ctrl, $attrs);
            codeCompareGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/WhS_Sales/Directives/CodeCompare/Templates/CodeCompareGrid.html'

    };

    function CodeCompareGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.gridColumnDefinitions = [];
            $scope.codeCompareItems = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        $scope.gridColumnDefinitions = [];
                        for (var i = 0; i < query.supplierIds.length; i++) {
                            var item = {
                                headerText: 'Supplier ' + [i + 1] + ' Zone Name',
                                field: 'SupplierItems[' + i + '].SupplierZone',
                                tag: '' + i + ''
                            };
                            $scope.gridColumnDefinitions.push(item);
                            item = {
                                headerText: 'Supplier ' +[i+1]+ ' Zone Code',
                                field: 'SupplierItems[' + i + '].SupplierCode',
                                color: $scope.getSupplierCodeStatusColor,
                                tag: '' + i + ''
                            };
                            $scope.gridColumnDefinitions.push(item);
                        }
                        var gridCol = {
                            headerText: 'Occurrence Code In Suppliers',
                            field: 'OccurrenceInSuppliers',
                            color: $scope.getOccurrenceInSupplierStatusColor
                        };
                        $scope.gridColumnDefinitions.push(gridCol);

                        gridCol = {
                            headerText: 'Absence Code In Suppliers',
                            field: 'AbsenceInSuppliers',
                            color: $scope.getAbssenceInSupplierStatusColor
                        };
                        $scope.gridColumnDefinitions.push(gridCol);

                        gridCol = {
                            headerText: 'Action',
                            field: 'Action'
                        };
                        $scope.gridColumnDefinitions.push(gridCol);

                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_CodeCompareAPIService.GetFilteredCodeCompare(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                SetActionName(item);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            $scope.getSaleCodeStatusColor = function (dataItem) {
                return WhS_Sales_CodeCompareService.getStatusColor(dataItem.SaleCodeIndicator);

            };
            $scope.getAbssenceInSupplierStatusColor = function (dataItem) {
                return WhS_Sales_CodeCompareService.getStatusColor(dataItem.AbsenceInSuppliersIndicator);

            };
            $scope.getOccurrenceInSupplierStatusColor = function (dataItem) {
                return WhS_Sales_CodeCompareService.getStatusColor(dataItem.OccurrenceInSuppliersIndicator);

            };
            $scope.getSupplierCodeStatusColor = function (dataItem, colDef) {

                return WhS_Sales_CodeCompareService.getStatusColor(dataItem.SupplierItems[colDef.tag].SupplierCodeIndicator);

            };

        }
        function SetActionName(dataItem) {
      
            if (dataItem.Action == 0)
                dataItem.Action = "N";
            else {
                if (dataItem.Action == 1)
                    dataItem.Action = "D";
            }
            
        }

    }

    return directiveDefinitionObject;

}]);
