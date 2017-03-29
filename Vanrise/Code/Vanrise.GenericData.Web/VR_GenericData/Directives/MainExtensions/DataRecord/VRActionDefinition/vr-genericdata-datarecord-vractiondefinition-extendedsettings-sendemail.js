'use strict';

app.directive('vrGenericdataDatarecordVractiondefinitionExtendedsettingsSendemail', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRMailMessageTypeAPIService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRMailMessageTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new sendEmailActionDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecord/VRActionDefinition/Templates/VRActionDefinitionSendEmail.html'
        };

        function sendEmailActionDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedDataRecordTypeId;
            var selectedMailMessageTypeId;

            var dataRecordTypeAPI;
            var dataRecordTypeReadyDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectionChanged;

            var mailMessageTypeAPI;
            var mailMessageTypeReadyDeferred = UtilsService.createPromiseDeferred();
            var mailMessageTypeSelectionChanged;

            var objectTypeDefinitionSelectorAPI;
            var objectTypeDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.objectTypeDefinitions = [];
                $scope.scopeModel.objectVariables = [];

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeAPI = api;
                    dataRecordTypeReadyDeferred.resolve();
                };

                $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                    mailMessageTypeAPI = api;
                    mailMessageTypeReadyDeferred.resolve();
                };

                $scope.scopeModel.onObjectTypeDefinitionSelectorReady = function (api) {
                    objectTypeDefinitionSelectorAPI = api;
                    objectTypeDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onMailMessageTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem != undefined) {
                        selectedMailMessageTypeId = selectedItem.VRMailMessageTypeId;
                        if (mailMessageTypeSelectionChanged != undefined)
                            mailMessageTypeSelectionChanged.resolve();
                        else {
                            getMailMessageTypePromise();
                        }
                    }
                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem != undefined) {
                        selectedDataRecordTypeId = selectedItem.DataRecordTypeId;
                        if (dataRecordTypeSelectionChanged != undefined)
                            dataRecordTypeSelectionChanged.resolve();
                        else {
                            getMailMessageTypePromise();
                        }
                    }
                };

                $scope.scopeModel.validateGrid = function () {
                    if ($scope.scopeModel.selectDataRecordObjectType == undefined)
                        return;

                    for (var x = 0; x < $scope.scopeModel.objectVariables.length; x++) {
                        var currentObjectVariable = $scope.scopeModel.objectVariables[x];
                        var currentDataRecordField = currentObjectVariable.dataRecordFieldSelectorAPI != undefined ? currentObjectVariable.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;
                        if (currentDataRecordField != undefined && $scope.scopeModel.selectDataRecordObjectType.ObjectName == currentObjectVariable.ObjectName) {
                            return 'invalid mapping';
                        }
                    }
                    return;
                };

                function getMailMessageTypePromise() {
                    if (selectedMailMessageTypeId == undefined || selectedDataRecordTypeId == undefined)
                        return;

                    $scope.scopeModel.onObjectTypeDefinitionsLoading = true;
                    $scope.scopeModel.objectTypeDefinitions.length = 0;
                    $scope.scopeModel.objectVariables.length = 0;
                    VRCommon_VRMailMessageTypeAPIService.GetMailMessageType(selectedMailMessageTypeId).then(function (response) {
                        var settings = response.Settings;
                        var objects = settings.Objects;
                        var prop;
                        for (prop in objects) {
                            if (prop != '$type') {
                                var obj = objects[prop];
                                $scope.scopeModel.objectTypeDefinitions.push({ ObjectName: obj.ObjectName, VRObjectTypeDefinitionId: obj.VRObjectTypeDefinitionId });

                                var objectVariable = { ObjectName: obj.ObjectName, VRObjectTypeDefinitionId: obj.VRObjectTypeDefinitionId };
                                $scope.scopeModel.objectVariables.push(objectVariable);
                                extendObjectVariableItemObject(objectVariable, undefined);
                            }
                        }

                        $scope.scopeModel.onObjectTypeDefinitionsLoading = false;
                    });
                };

                defineAPI();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                        extendedSettings = payload.Settings.ExtendedSettings;
                    }
                    if (extendedSettings != undefined) {
                        mailMessageTypeSelectionChanged = UtilsService.createPromiseDeferred();
                        dataRecordTypeSelectionChanged = UtilsService.createPromiseDeferred();
                    }

                    var promises = [];

                    var dataRecordTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    dataRecordTypeReadyDeferred.promise.then(function () {
                        var dataRecordTypePayload;
                        if (extendedSettings != undefined) {
                            dataRecordTypePayload = { selectedIds: extendedSettings.DataRecordTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeAPI, dataRecordTypePayload, dataRecordTypeLoadDeferred);
                    });
                    promises.push(dataRecordTypeLoadDeferred.promise);

                    var mailMessageTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTypeReadyDeferred.promise.then(function () {
                        var mailMessageTypePayload;
                        if (extendedSettings != undefined) {
                            mailMessageTypePayload = { selectedIds: extendedSettings.MailMessageTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(mailMessageTypeAPI, mailMessageTypePayload, mailMessageTypeLoadDeferred);
                    });
                    promises.push(mailMessageTypeLoadDeferred.promise);

                    if (extendedSettings != undefined) {
                        var getMailMessageTypePromise = VRCommon_VRMailMessageTypeAPIService.GetMailMessageType(extendedSettings.MailMessageTypeId).then(function (response) {

                            UtilsService.waitMultiplePromises([mailMessageTypeSelectionChanged.promise, dataRecordTypeSelectionChanged.promise]).then(function () {
                                mailMessageTypeSelectionChanged = undefined;
                                dataRecordTypeSelectionChanged = undefined;
                                var settings = response.Settings;
                                var objects = settings.Objects;
                                var prop;
                                for (prop in objects) {
                                    if (prop != '$type') {
                                        var obj = objects[prop];
                                        if (extendedSettings.DataRecordObjectName != undefined && extendedSettings.DataRecordObjectName == obj.ObjectName) {
                                            $scope.scopeModel.selectDataRecordObjectType = { ObjectName: obj.ObjectName, VRObjectTypeDefinitionId: obj.VRObjectTypeDefinitionId };
                                        }
                                        $scope.scopeModel.objectTypeDefinitions.push({ ObjectName: obj.ObjectName, VRObjectTypeDefinitionId: obj.VRObjectTypeDefinitionId });

                                        var objectVariable = { ObjectName: obj.ObjectName, VRObjectTypeDefinitionId: obj.VRObjectTypeDefinitionId };
                                        $scope.scopeModel.objectVariables.push(objectVariable);

                                        var selectedObjectVariable;
                                        if (extendedSettings.ObjectFieldMappings != undefined) {
                                            selectedObjectVariable = UtilsService.getItemByVal(extendedSettings.ObjectFieldMappings, obj.ObjectName, "ObjectName");
                                        }

                                        promises.push(extendObjectVariableItemObject(objectVariable, selectedObjectVariable));
                                    }
                                }
                            });
                        });
                        promises.push(getMailMessageTypePromise);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.Notification.DataRecordSendEmailDefinitionSettings, Vanrise.GenericData.Notification',
                        DataRecordTypeId: dataRecordTypeAPI.getSelectedIds(),
                        MailMessageTypeId: mailMessageTypeAPI.getSelectedIds(),
                        DataRecordObjectName: $scope.scopeModel.selectDataRecordObjectType != undefined ? $scope.scopeModel.selectDataRecordObjectType.ObjectName : undefined,
                        ObjectFieldMappings: buildObjectFieldMappings()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            function extendObjectVariableItemObject(objectVariable, selectedObjectVariable) {
                objectVariable.dataRecordFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                objectVariable.onDataRecordFieldSelectorReady = function (api) {
                    objectVariable.dataRecordFieldSelectorAPI = api;

                    var filters = [];
                    var dataRecordFieldPayload = {
                        dataRecordTypeId: selectedDataRecordTypeId
                    };

                    if (selectedObjectVariable != undefined) {
                        dataRecordFieldPayload.selectedIds = selectedObjectVariable.DataRecordFieldName;
                    }
                    VRUIUtilsService.callDirectiveLoad(objectVariable.dataRecordFieldSelectorAPI, dataRecordFieldPayload, objectVariable.dataRecordFieldSelectorLoadDeferred);
                };
                return objectVariable.dataRecordFieldSelectorLoadDeferred.promise;
            };

            function buildObjectFieldMappings() {
                var objectFieldMappings = [];
                for (var x = 0; x < $scope.scopeModel.objectVariables.length; x++) {
                    var currentObjectVariable = $scope.scopeModel.objectVariables[x];
                    var currentDataRecordField = currentObjectVariable.dataRecordFieldSelectorAPI.getSelectedIds();
                    if (currentDataRecordField != undefined) {
                        objectFieldMappings.push({
                            ObjectName: currentObjectVariable.ObjectName,
                            VRObjectTypeDefinitionId: currentObjectVariable.VRObjectTypeDefinitionId,
                            DataRecordFieldName: currentDataRecordField
                        });
                    }
                }
                return objectFieldMappings;
            };
        }
    }]);