(function (app) {

    'use strict';

    GenericbeGenericbegridviewDefinitionViewDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function GenericbeGenericbegridviewDefinitionViewDirective(UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericbeGenericbegridviewDefinitionViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEViewDefinition/Templates/GenericbeGenericbegridviewDefinitionViewTemplate.html'
        };

        function GenericbeGenericbegridviewDefinitionViewCtor($scope, ctrl) {

            this.initializeController = initializeController;
            var bEDefinitionSelectorAPI;
            var bEDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var context;
            var onBusinessEntityDefinitionSelectionChangeDeffered;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                    bEDefinitionSelectorAPI = api;
                    bEDefinitionSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onBusinessEntityDefinitionSelectionChange = function (beDefinitionId) {
                    if (beDefinitionId != undefined) {
                        if (onBusinessEntityDefinitionSelectionChangeDeffered != undefined) {
                            onBusinessEntityDefinitionSelectionChangeDeffered.resolve();
                        }
                        else {
                            $scope.scopeModel.mappingItems = [];

                            $scope.scopeModel.dataRecordTypeId = undefined;
                            var bEDefinitionId = beDefinitionId.BusinessEntityDefinitionId;
                            if (bEDefinitionId != undefined) {
                                VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(bEDefinitionId).then(function (response) {
                                    $scope.scopeModel.dataRecordTypeId = response.DataRecordTypeId;
                                });
                            }
                        }
                    }
                }
                $scope.scopeModel.removeMappingItem = function (dataItem) {
                    var index = $scope.scopeModel.mappingItems.indexOf(dataItem);
                    $scope.scopeModel.mappingItems.splice(index, 1);

                };
                $scope.scopeModel.addMappingItem = function () {
                    var mappingItem = {};

                    mappingItem.onParentDataRecordTypeFieldSelectorReady = function (api) {
                        mappingItem.parentDataRecordTypeFieldSelectorAPI = api;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingMappings = value;
                        };
                        var selectorPayload = {
                            dataRecordTypeId: context.getDataRecordTypeId()
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, mappingItem.parentDataRecordTypeFieldSelectorAPI, selectorPayload, setLoader);
                    };

                    mappingItem.onChildDataRecordTypeFieldSelectorReady = function (api) {
                        mappingItem.childDataRecordTypeFieldSelectorAPI = api;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingMappings = value;
                        };
                        var selectorPayload = {
                            dataRecordTypeId: $scope.scopeModel.dataRecordTypeId
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, mappingItem.childDataRecordTypeFieldSelectorAPI, selectorPayload, setLoader);
                    };

                    $scope.scopeModel.mappingItems.push(mappingItem);
                }
                $scope.scopeModel.validateMappingItems = function () {
                   
                    if ($scope.scopeModel.mappingItems.length > 0) {
                            var parentDataRecordFieldsIds = [];
                            for (var i = 0; i < $scope.scopeModel.mappingItems.length; i++) {
                                var mappingItem = $scope.scopeModel.mappingItems[i];
                                if (mappingItem.parentDataRecordTypeFieldSelectorAPI != undefined && mappingItem.parentDataRecordTypeFieldSelectorAPI.getSelectedIds() != undefined) {
                                    parentDataRecordFieldsIds.push(mappingItem.parentDataRecordTypeFieldSelectorAPI.getSelectedIds());
                                }
                            }
                            while (parentDataRecordFieldsIds.length > 0) {
                                var idToValidate = parentDataRecordFieldsIds[0];
                                parentDataRecordFieldsIds.splice(0, 1);
                                if (!validateId(idToValidate, parentDataRecordFieldsIds)) {
                                    return 'Two or more parents have the same data field type.';
                                }
                            }
                            return null;
                            function validateId(name, array) {
                                for (var j = 0; j < array.length; j++) {
                                    if (array[j] == name)
                                        return false;
                                }
                                return true;
                            }
                        }
                        return false;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    $scope.scopeModel.mappingItems = [];
                    context = payload != undefined ? payload.context : undefined;
                    if (payload != undefined && payload.parameterEntity != undefined && payload.parameterEntity.GenericBEDefinitionId) {
                        var promise = UtilsService.createPromiseDeferred();
                        promises.push(promise.promise);
                        var gridPromises = [];
                        onBusinessEntityDefinitionSelectionChangeDeffered = UtilsService.createPromiseDeferred();
                        gridPromises.push(onBusinessEntityDefinitionSelectionChangeDeffered.promise);
                        VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(payload.parameterEntity.GenericBEDefinitionId).then(function (response) {

                            
                            $scope.scopeModel.dataRecordTypeId = response.DataRecordTypeId;

                            if (payload.parameterEntity.Mappings != undefined) {
                                for (var index in payload.parameterEntity.Mappings) {

                                    var gridItem = {
                                        parentColumnName: payload.parameterEntity.Mappings[index].ParentColumnName,
                                        subviewColumnName: payload.parameterEntity.Mappings[index].SubviewColumnName,
                                        parentDataRecordTypeFieldSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                                        parentDataRecordTypeFieldSelectorLoadDeferred: UtilsService.createPromiseDeferred(),
                                        childDataRecordTypeFieldSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                                        childDataRecordTypeFieldSelectorLoadDeferred: UtilsService.createPromiseDeferred(),
                                    };
                                    gridPromises.push(gridItem.childDataRecordTypeFieldSelectorLoadDeferred.promise);
                                    gridPromises.push(gridItem.parentDataRecordTypeFieldSelectorLoadDeferred.promise);
                                    addItemtoGrid(gridItem);
                                }
                            }

                            function addItemtoGrid(gridItem) {

                                gridItem.onParentDataRecordTypeFieldSelectorReady = function (api) {
                                    gridItem.parentDataRecordTypeFieldSelectorAPI = api;
                                    gridItem.parentDataRecordTypeFieldSelectorReadyDeferred.resolve();
                                };

                                gridItem.onChildDataRecordTypeFieldSelectorReady = function (api) {
                                    gridItem.childDataRecordTypeFieldSelectorAPI = api;
                                    gridItem.childDataRecordTypeFieldSelectorReadyDeferred.resolve();
                                };

                                gridItem.parentDataRecordTypeFieldSelectorReadyDeferred.promise.then(function () {
                                    var dataRecordTypeFieldSelectorPayload = {
                                        dataRecordTypeId: context.getDataRecordTypeId(),
                                        selectedIds: gridItem.parentColumnName
                                    };
                                    VRUIUtilsService.callDirectiveLoad(gridItem.parentDataRecordTypeFieldSelectorAPI, dataRecordTypeFieldSelectorPayload, gridItem.parentDataRecordTypeFieldSelectorLoadDeferred);
                                });

                                gridItem.childDataRecordTypeFieldSelectorReadyDeferred.promise.then(function () {
                                    var dataRecordTypeFieldSelectorPayload = {
                                        dataRecordTypeId: $scope.scopeModel.dataRecordTypeId,
                                        selectedIds: gridItem.subviewColumnName
                                    };
                                    VRUIUtilsService.callDirectiveLoad(gridItem.childDataRecordTypeFieldSelectorAPI, dataRecordTypeFieldSelectorPayload, gridItem.childDataRecordTypeFieldSelectorLoadDeferred);
                                });

                                $scope.scopeModel.mappingItems.push(gridItem);
                            }
                            UtilsService.waitMultiplePromises(gridPromises).then(function () { promise.resolve(); onBusinessEntityDefinitionSelectionChangeDeffered = undefined; });
                        })
                            .catch(function (error) {
                                promise.resolve();
                                onBusinessEntityDefinitionSelectionChangeDeffered=undefined
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                    }

                    promises.push(loadBEDefinitionSelector());
                    function loadBEDefinitionSelector() {
                        var bEDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        bEDefinitionSelectorReadyDeferred.promise.then(function () {
                            var bEDefinitionSelectorPayload;
                            if (payload != undefined && payload.parameterEntity != undefined && payload.parameterEntity.GenericBEDefinitionId != undefined)
                                bEDefinitionSelectorPayload = {
                                    selectedIds: payload.parameterEntity.GenericBEDefinitionId
                                };
                            VRUIUtilsService.callDirectiveLoad(bEDefinitionSelectorAPI, bEDefinitionSelectorPayload, bEDefinitionSelectorLoadDeferred);
                        });
                        return bEDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };


                api.getData = function () {
                    var genericBEDefinitionView = {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBEDefinitionView, Vanrise.GenericData.MainExtensions",
                        GenericBEDefinitionId: bEDefinitionSelectorAPI.getSelectedIds()
                    };
                    var mappings = [];
                    for (var i = 0; i < $scope.scopeModel.mappingItems.length; i++) {
                        var mappingitem = {
                            ParentColumnName: $scope.scopeModel.mappingItems[i].parentDataRecordTypeFieldSelectorAPI.getSelectedIds(),
                            SubviewColumnName: $scope.scopeModel.mappingItems[i].childDataRecordTypeFieldSelectorAPI.getSelectedIds()
                        };
                        mappings.push(mappingitem);
                    }
                    genericBEDefinitionView.Mappings = mappings;
                    return genericBEDefinitionView;
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeGenericbegridviewDefinition', GenericbeGenericbegridviewDefinitionViewDirective);

})(app);