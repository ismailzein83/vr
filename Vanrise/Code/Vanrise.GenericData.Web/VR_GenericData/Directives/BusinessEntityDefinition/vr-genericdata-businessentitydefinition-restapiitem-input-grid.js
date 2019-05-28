'use strict';

app.directive('vrGenericdataBusinessentitydefinitionRestapiitemInputGrid', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RestAPIInputItemGridCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/Templates/BERestAPIInputItemGridTemplate.html';
            }
        };

        function RestAPIInputItemGridCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.restApiItems = [];
                $scope.scopeModel.dataRecordTypeFields = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAddRestAPIItemClicked = function () {
                    extendAndAddRestAPIItemToGrid();
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.restApiItems, deletedItem.tempId, 'tempId');
                    $scope.scopeModel.restApiItems.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var beRestAPIItems;
                    var context;

                    $scope.scopeModel.restApiItems.length = 0;
                    $scope.scopeModel.dataRecordTypeFields.length = 0;

                    var promises = [];

                    if (payload != undefined) {
                        beRestAPIItems = payload.beRestAPIItems;
                        context = payload.context;
                    }

                    if (context != undefined) {
                        $scope.scopeModel.dataRecordTypeFields = context.getFields();
                    }

                    if (beRestAPIItems != undefined && beRestAPIItems.length > 0) {
                        var loadRestAPIItemGridPromise = getRestAPIItemsLoadPromise();
                        promises.push(loadRestAPIItemGridPromise);
                    }

                    function getRestAPIItemsLoadPromise() {
                        var loadRestAPIItemsPromiseDeferred = UtilsService.createPromiseDeferred();

                        gridPromiseDeferred.promise.then(function () {
                            var _promises = [];
                            for (var i = 0; i < beRestAPIItems.length; i++) {
                                var currentRestAPIItem = beRestAPIItems[i];
                                _promises.push(extendAndAddRestAPIItemToGrid(currentRestAPIItem));
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                loadRestAPIItemsPromiseDeferred.resolve();
                            });
                        });

                        return loadRestAPIItemsPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var restAPIItems = [];

                    for (var i = 0; i < $scope.scopeModel.restApiItems.length; i++)
                        restAPIItems.push(buildBusinessEntityRestAPIItem($scope.scopeModel.restApiItems[i]));

                    return restAPIItems.length > 0 ? restAPIItems : undefined;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendAndAddRestAPIItemToGrid(restApiItem) {

                var extendOptionPromises = [];

                var restApiItemDataItem = { tempId: UtilsService.guid() };

                if (restApiItem != undefined) {
                    restApiItemDataItem.propertyName = restApiItem.PropertyName;
                    restApiItemDataItem.isRequired = restApiItem.IsRequired;
                }

                //Loading DataRecordFieldsSelector
                restApiItemDataItem.dataRecordFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                extendOptionPromises.push(restApiItemDataItem.dataRecordFieldsSelectorReadyPromiseDeferred.promise);

                restApiItemDataItem.onDataRecordFieldsSelectorReady = function (api) {
                    restApiItemDataItem.dataRecordFieldsSelectorAPI = api;

                    if (restApiItem != undefined && restApiItem.FieldName != undefined) {
                        var selectedValue = UtilsService.getItemByVal($scope.scopeModel.dataRecordTypeFields, restApiItem.FieldName, "FieldName");
                        if (selectedValue != null)
                            restApiItemDataItem.selectedItem = selectedValue;
                    }

                    restApiItemDataItem.dataRecordFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.restApiItems.push(restApiItemDataItem);

                return UtilsService.waitMultiplePromises(extendOptionPromises);
            }

            function buildBusinessEntityRestAPIItem(restApiItemDataItem) {
                return {
                    FieldName: restApiItemDataItem.selectedItem != undefined ? restApiItemDataItem.selectedItem.FieldName : null,
                    PropertyName: restApiItemDataItem.propertyName,
                    IsRequired: restApiItemDataItem.isRequired
                };
            }
        }

        return directiveDefinitionObject;
    }]);