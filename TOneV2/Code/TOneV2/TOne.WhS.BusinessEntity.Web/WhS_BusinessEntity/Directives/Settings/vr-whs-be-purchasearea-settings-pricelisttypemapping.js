"use strict";

app.directive("vrWhsBePurchaseareaSettingsPricelisttypemapping", ["UtilsService", "WhS_SupPL_SupplierPriceListTypeEnum",
function (UtilsService , WhS_SupPL_SupplierPriceListTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var mappingGrid = new MappingGrid($scope, ctrl, $attrs);
            mappingGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/PricelistTypeMappingTemplate.html"
    };

    function MappingGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        //var pricelisttypeSelectorAPI;
        //var pricelisttypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var pricelistTypesOptions = UtilsService.getArrayEnum(WhS_SupPL_SupplierPriceListTypeEnum);

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.pricelistTypeMappers = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

            };



            $scope.scopeModel.addColumn = function (data) {
                var gridItem = {
                        id: $scope.scopeModel.pricelistTypeMappers.length + 1,
                };
                //gridItem.onPricelistTypeSelectorReady = function (api) {
                //    console.log("onPricelistTypeSelectorReady");
                //    pricelisttypeSelectorAPI = api;
                //    pricelisttypeSelectorReadyDeferred.resolve();
                //};

                ////pricelisttypeSelectorReadyDeferred.promise.then
                    setPricelistTypes(gridItem);
                    if (data != undefined) {
                        gridItem.Subject = data.Subject;
                        gridItem.selectedPricelistType = data.PricelistType;
                    }
                    $scope.scopeModel.pricelistTypeMappers.push(gridItem);


            };

            $scope.scopeModel.removeColumn = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.pricelistTypeMappers, dataItem.Subject, 'Subject');
                if (index > -1) {
                    $scope.scopeModel.pricelistTypeMappers.splice(index, 1);
                }
            };
            $scope.scopeModel.validateColumns = function () {
                //if ($scope.scopeModel.pricelistTypeMappers.length == 0) {
                //    return 'Please, one record must be added at least.';
                //}

                var columnSubject = [];
                for (var i = 0; i < $scope.scopeModel.pricelistTypeMappers.length; i++) {
                    if ($scope.scopeModel.pricelistTypeMappers[i].Subject != undefined) {
                        columnSubject.push($scope.scopeModel.pricelistTypeMappers[i].Subject);
                    }
                }
                while (columnSubject.length > 0) {
                    var nameToValidate = columnSubject[0];
                    columnSubject.splice(0, 1);
                    if (!validateName(nameToValidate, columnSubject)) {
                        return 'Two or more columns have the same Subject';
                    }
                }
                return null;
                function validateName(name, array) {
                    for (var j = 0; j < array.length; j++) {
                        if (array[j] == name) 
                            return false;
                    }
                    return true;
                }
            };

        }
        function getDirectiveAPI() {
            var directiveAPI = {};

            directiveAPI.load = function (payload) {

                if (payload != undefined) {
                    var pricelistTypeMappingList = payload.pricelistTypeMappingList;

                    if (pricelistTypeMappingList != null) {
                        for (var i = 0; i < pricelistTypeMappingList.length; i++) {
                            var item = pricelistTypeMappingList[i];
                            setPricelistTypes(item);

                            item.selectedPricelistType = UtilsService.getItemByVal(pricelistTypesOptions, item.PricelistType, 'value');
                            if (item != undefined) {
                                $scope.scopeModel.pricelistTypeMappers.push(item);
                            }
                        }
                    }
                }

            };

            directiveAPI.getData = function () {
                var columns = [];
                for (var i = 0; i < $scope.scopeModel.pricelistTypeMappers.length; i++) {
                    var column = $scope.scopeModel.pricelistTypeMappers[i];
                    columns.push({
                        Subject: column.Subject,
                        PricelistType: column.selectedPricelistType.value,
                    });
                }
                return columns;
            };
            return directiveAPI;
        }

        function setPricelistTypes(gridItem) { 
            gridItem.pricelistTypesOptions = [];
            for (var i = 0; i < pricelistTypesOptions.length; i++) {
                gridItem.pricelistTypesOptions.push(pricelistTypesOptions[i]);
            }
        }

    }

    return directiveDefinitionObject;

}]);