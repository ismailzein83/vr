//"use strict";

//app.directive("bpGenericbeStartbpprocessCustomaction", ["UtilsService", "VRUIUtilsService", 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_VRWorkflowAPIService', 'VRNotificationService', 'VRWorkflowArgumentDirectionEnum',
//    function (UtilsService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_VRWorkflowAPIService, VRNotificationService, VRWorkflowArgumentDirectionEnum) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new StartBPProcessCustomActionCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/MainExtensions/GenericBECustomActionSettings/Templates/StartBPProcessCustomActionTemplate.html"
//        };

//        function StartBPProcessCustomActionCtor($scope, ctrl) {
//            this.initializeController = initializeController;

//            var settings;
//            var context;
//            var inputArguments = [];

//            var bpDefinitionSelectorAPI;
//            var bpDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//            var bpDefinitionEntity;

//            var gridAPI;
//            var gridDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            var selectedBPDefinitionSelectionChangeDeffered;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.inputFields = [];

//                $scope.scopeModel.onBPDefinitionDirectiveReady = function (api) {
//                    bpDefinitionSelectorAPI = api;
//                    bpDefinitionSelectorReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onBPDefinitionSelectionChanged = function (item) {
//                    if (item != undefined) {
//                        if (selectedBPDefinitionSelectionChangeDeffered != undefined) {
//                            selectedBPDefinitionSelectionChangeDeffered.resolve();
//                        }
//                        else {

//                            inputArguments.length = 0;
//                            $scope.scopeModel.inputFields.length = 0;

//                            $scope.scopeModel.isGridLoading = true;
//                            var rootPromiseNode = {
//                                promises: [getBPDefintion(item.BPDefinitionID)],
//                                getChildNode: function () {
//                                    var directivePromises = [];
//                                    if (bpDefinitionEntity != undefined && bpDefinitionEntity.VRWorkflowId != undefined)
//                                        directivePromises.push(getVRWorkflowArguments(bpDefinitionEntity.VRWorkflowId));
//                                    return {
//                                        promises: directivePromises,
//                                    };
//                                }
//                            };
//                            UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
//                                if (inputArguments.length > 0) {
//                                    for (var i = 0; i < inputArguments.length; i++) {
//                                        var value = inputArguments[i];
//                                        addItemToGridAfterBPDefinitionSelectionChanged(value)
//                                    }
//                                }
//                            }).finally(function () {
//                                bpDefinitionEntity = undefined;
//                                $scope.scopeModel.isGridLoading = false;
//                            });
//                        }
//                    }
//                };

//                $scope.scopeModel.onGridReady = function (api) {
//                    gridAPI = api;
//                    gridDirectiveReadyPromiseDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    context = payload.context;

//                    inputArguments.length = 0;
//                    $scope.scopeModel.inputFields.length = 0;

//                    if (payload != undefined) {
//                        if (payload.settings != undefined) {
//                            settings = payload.settings;
//                        }
//                    }

//                    function loadBPDefinitions() {
//                        var loadBPDefinitionsPromiseDeferred = UtilsService.createPromiseDeferred();
//                        bpDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
//                            var payload = {
//                                filter: {
//                                    $type: "Vanrise.BusinessProcess.Entities.BPDefinitionInfoFilter,Vanrise.BusinessProcess.Entities"
//                                },
//                                selectedIds: (settings != undefined) ? settings.BPDefinitionId : undefined
//                            };
//                            VRUIUtilsService.callDirectiveLoad(bpDefinitionSelectorAPI, payload, loadBPDefinitionsPromiseDeferred);
//                        });

//                        return loadBPDefinitionsPromiseDeferred.promise;
//                    }

//                    var initialPromises = [];

//                    if (settings != undefined) {
//                        selectedBPDefinitionSelectionChangeDeffered = UtilsService.createPromiseDeferred();
//                        initialPromises.push(getBPDefintion(settings.BPDefinitionId));
//                    }
//                    initialPromises.push(loadBPDefinitions());


//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            if (bpDefinitionEntity != undefined && bpDefinitionEntity.VRWorkflowId != undefined)
//                                directivePromises.push(getVRWorkflowArguments(bpDefinitionEntity.VRWorkflowId));

//                            return {
//                                promises: directivePromises,
//                                getChildNode: function () {
//                                    var dataItemsLoadPromises = [];

//                                    if (inputArguments.length > 0) {
//                                        $scope.scopeModel.isGridLoading = true;
//                                        for (var i = 0; i < inputArguments.length; i++) {
//                                            var dataItem = {
//                                                payload: inputArguments[i],
//                                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
//                                            };
//                                            dataItemsLoadPromises.push(dataItem.loadPromiseDeferred.promise);
//                                            addItemToGrid(dataItem, settings.InputArgumentsMapping);

//                                        }
//                                        $scope.scopeModel.isGridLoading = false;
//                                    }

//                                    return {
//                                        promises: dataItemsLoadPromises,
//                                    };
//                                }
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
//                        selectedBPDefinitionSelectionChangeDeffered = undefined;
//                        bpDefinitionEntity = undefined;
//                    });
//                };

//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.BusinessProcess.MainExtensions.StartBPProcessCustomAction, Vanrise.BusinessProcess.MainExtensions",
//                        BPDefinitionId: bpDefinitionSelectorAPI.getSelectedIds(),
//                        InputArgumentsMapping: getInputArgumentsMappings()
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function getBPDefintion(bpDefinitionId) {
//                return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionId).then(function (response) {
//                    bpDefinitionEntity = response;
//                });
//            }

//            function getVRWorkflowArguments(vrWorkflowId) {
//                return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowArguments(vrWorkflowId).then(function (response) {
//                    var workflowsArguments = response;
//                    for (var i = 0; i < workflowsArguments.length; i++) {
//                        var workflowargument = workflowsArguments[i];
//                        if (workflowargument.Direction == VRWorkflowArgumentDirectionEnum.In.value) {
//                            inputArguments.push(workflowargument);
//                        }
//                    }
//                });
//            }

//            function addItemToGrid(gridItem, inputArgumentsMapping) {
//                var dataItem = {
//                    id: $scope.scopeModel.inputFields.length + 1,
//                    fieldName: gridItem.payload.Name
//                };

//                if (inputArgumentsMapping != undefined && inputArgumentsMapping.length > 0) {
//                    var item = UtilsService.getItemByVal(inputArgumentsMapping, dataItem.fieldName, "InputArgumentName");
//                    if (item != undefined) {
//                        gridItem.mappedFieldName = item.MappedFieldName;
//                    }
//                }

//                dataItem.onDataRecordTypeFieldsSelectorReady = function (api) {
//                    dataItem.dataRecordTypeFieldsAPI = api;
//                    gridItem.readyPromiseDeferred.resolve();
//                };

//                gridItem.readyPromiseDeferred.promise.then(function () {
//                    var dataRecordTypeFieldSelectorPayload = {
//                        dataRecordTypeId: context.getDataRecordTypeId(),
//                        selectedIds: gridItem.mappedFieldName
//                    };
//                    VRUIUtilsService.callDirectiveLoad(dataItem.dataRecordTypeFieldsAPI, dataRecordTypeFieldSelectorPayload, gridItem.loadPromiseDeferred);
//                });

//                $scope.scopeModel.inputFields.push(dataItem);
//            }

//            function addItemToGridAfterBPDefinitionSelectionChanged(payload) {
//                var gridItem = {
//                    id: $scope.scopeModel.inputFields.length + 1,
//                    fieldName: payload.Name
//                };

//                gridItem.onDataRecordTypeFieldsSelectorReady = function (api) {
//                    gridItem.dataRecordTypeFieldsAPI = api;
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isGridLoading = value;
//                    };
//                    var selectorPayload = {
//                        dataRecordTypeId: context.getDataRecordTypeId()
//                    };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.dataRecordTypeFieldsAPI, selectorPayload, setLoader);
//                };

//                $scope.scopeModel.inputFields.push(gridItem);
//            }

//            function getInputArgumentsMappings() {
//                var columns = [];
//                for (var i = 0; i < $scope.scopeModel.inputFields.length; i++) {
//                    var column = $scope.scopeModel.inputFields[i];
//                    var mappedFieldName = column.dataRecordTypeFieldsAPI.getSelectedIds();
//                    if (mappedFieldName != undefined) {
//                        columns.push({
//                            InputArgumentName: column.fieldName,
//                            MappedFieldName: mappedFieldName
//                        });
//                    }
//                }
//                return columns;
//            }
//        }

//        return directiveDefinitionObject;
//    }]);