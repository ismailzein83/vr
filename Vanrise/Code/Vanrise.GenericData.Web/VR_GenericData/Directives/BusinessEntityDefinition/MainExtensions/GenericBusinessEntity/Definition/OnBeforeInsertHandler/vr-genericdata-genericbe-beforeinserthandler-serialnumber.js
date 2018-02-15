"use strict";

app.directive("vrGenericdataGenericbeBeforeinserthandlerSerialnumber", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SerialNumberHandler($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/OnBeforeInsertHandler/Templates/SerialNumberHandlerEditor.html"
        };

        function SerialNumberHandler($scope, ctrl, $attrs) {

            var context;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

               
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one part.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "VariableName in each part should be unique.";

                    return null;
                };

                ctrl.addSerialNumberPart = function () {
                    var onSerialNumberPartAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBESerialNumberPart(onSerialNumberPartAdded, getContext());
                };
              
                ctrl.removeSerialNumberPart = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };


                defineMenuActions();

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var parts;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        parts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            parts.push({
                                GenericBESerialNumberPartId: currentItem.GenericBESerialNumberPartId,
                                VariableName: currentItem.VariableName,
                                VariableDescription: currentItem.VariableDescription,
                                Settings: currentItem.Settings
                            });
                        }
                    }

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers.SerialNumberOnBeforeInsertHandler, Vanrise.GenericData.MainExtensions",
                        PartDefinitions: parts,
                        InfoType: $scope.scopeModel.infoType,
                        FieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds()
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        api.clearDataSource();                        
                    }


                    function loadStaticData() {
                        if (payload == undefined || payload.settings == undefined)
                            return;

                        $scope.scopeModel.infoType = payload.settings.InfoType;
                    }

                    function loadPartsDefinitionGrid() {
                        if (payload.settings != undefined && payload.settings.PartDefinitions != undefined) {
                            var data = payload.settings.PartDefinitions;
                            for (var i = 0; i < data.length; i++) {
                                ctrl.datasource.push(data[i]);
                            }
                        }
                    }

                    function loadDataRecordTypeFieldsSelector() {
                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var typeFieldsPayload = {
                                dataRecordTypeId: getContext().getDataRecordTypeId() ,
                                selectedIds: payload != undefined && payload.settings != undefined ? payload.settings.FieldName : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadPartsDefinitionGrid, loadDataRecordTypeFieldsSelector]);
                };


                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSerialNumberPart
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editSerialNumberPart(partObj) {
                var onSerialNumberPartUpdated = function (part) {
                    var index = ctrl.datasource.indexOf(partObj);
                    ctrl.datasource[index] = part;
                };
                VR_GenericData_GenericBEDefinitionService.editGenericBESerialNumberPart(onSerialNumberPartUpdated, partObj, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i];
                    for (var j = 0; j < ctrl.datasource.length; j++) {
                        if (i != j && ctrl.datasource[j].VariableName == currentItem.VariableName)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);