(function (app) {

    'use strict';

    VRDataRecordObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function VRDataRecordObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordObjectType = new DataRecordObjectType($scope, ctrl, $attrs);
                dataRecordObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/VRObjectTypes/Templates/VRDataRecordObjectTypeTemplate.html"

        };
        function DataRecordObjectType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordObjectTypeSelectorAPI;
            var dataRecordObjectTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var context;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var dataRecordObjectTypeSelectionChangedDeferred;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordObjectTypeSelectorReady = function (api) {
                    dataRecordObjectTypeSelectorAPI = api;
                    dataRecordObjectTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordObjectTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem == undefined)
                        return;

                    context.canDefineProperties(true);

                    if (dataRecordObjectTypeSelectionChangedDeferred != undefined) {
                        dataRecordObjectTypeSelectionChangedDeferred.resolve();
                    }
                    else {
                        var beDefinitionSelectorPayload = {
                            filter: {
                                DataRecordTypeIds: [selectedItem.DataRecordTypeId]
                            }
                        };

                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingbeDefinitionSelector = value;
                        };

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, beDefinitionSelectorAPI, beDefinitionSelectorPayload, setLoader);
                    }
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        context = payload.context;

                        context.canDefineProperties(false);
                    }

                    if (payload != undefined && payload.objectType != undefined) {
                        dataRecordObjectTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                        var loadBEDefinitionSelectorPromise = loadBEDefinitionSelector();
                        promises.push(loadBEDefinitionSelectorPromise);
                    }

                    var loadDataRecordObjectTypeSelectorPromise = loadDataRecordObjectTypeSelector();
                    promises.push(loadDataRecordObjectTypeSelectorPromise);

                    function loadDataRecordObjectTypeSelector() {
                        var dataRecordObjectTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordObjectTypeSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload;

                            if (payload != undefined && payload.objectType != undefined)
                                selectorPayload = { selectedIds: payload.objectType.RecordTypeId };

                            VRUIUtilsService.callDirectiveLoad(dataRecordObjectTypeSelectorAPI, selectorPayload, dataRecordObjectTypeSelectorLoadDeferred);
                        });

                        return dataRecordObjectTypeSelectorLoadDeferred.promise;
                    }

                    function loadBEDefinitionSelector() {
                        var loadBEDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([dataRecordObjectTypeSelectionChangedDeferred.promise, beDefinitionSelectorReadyDeferred.promise]).then(function () {
                            dataRecordObjectTypeSelectionChangedDeferred = undefined;

                            var beDefinitionSelectorPayload = {
                                filter: {
                                    DataRecordTypeIds: [dataRecordObjectTypeSelectorAPI.getSelectedIds()]
                                },
                                selectedIds: payload.objectType.BusinessEntityDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, beDefinitionSelectorPayload, loadBEDefinitionSelectorPromiseDeferred);
                        });

                        return loadBEDefinitionSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordObjectType, Vanrise.GenericData.MainExtensions",
                        RecordTypeId: dataRecordObjectTypeSelectorAPI.getSelectedIds(),
                        BusinessEntityDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordobjecttype', VRDataRecordObjectType);

})(app);