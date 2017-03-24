(function (appControllers) {

    "use strict";

    DataRecordAlertRuleEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Analytic_DAProfCalcOutputSettingsAPIService'];

    function DataRecordAlertRuleEditorController($scope, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService, VR_Analytic_DAProfCalcOutputSettingsAPIService) {

        var isEditMode;
        var dataRecordAlertRuleEntity;
        var vrActionTargetType;
        var recordfields;

        var vRActionManagementAPI;
        var vRActionManagementReadyDeferred = UtilsService.createPromiseDeferred();

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                dataRecordAlertRuleEntity = parameters.dataRecordAlertRuleEntity;

                if (parameters.context != undefined) {
                    vrActionTargetType = parameters.context.vrActionTargetType;
                    recordfields = parameters.context.recordfields;
                }
            }
            isEditMode = dataRecordAlertRuleEntity != undefined;
        };
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.onVRActionManagementDirectiveReady = function (api) {
                vRActionManagementAPI = api;
                vRActionManagementReadyDeferred.resolve();
            };

            $scope.scopeModel.validateRecordFilter = function () {
                if (recordFilterDirectiveAPI != undefined) {
                    var filterObj = recordFilterDirectiveAPI.getData().filterObj;
                    if (filterObj != undefined && filterObj != {})
                        return null;
                    else
                        return 'Record Filter is required';
                }
                return null;
            };

            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        };
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        };

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRActionManagement, loadRecordFilterDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };
        function setTitle() {
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor('Record Alert Rule');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Record Alert Rule');
            }
        };
        function loadStaticData() {
            if (dataRecordAlertRuleEntity == undefined)
                return;
        };
        function loadVRActionManagement() {
            var vRActionManagementLoadDeferred = UtilsService.createPromiseDeferred();

            vRActionManagementReadyDeferred.promise.then(function () {
                var vrActionPayload = {
                    isRequired: false,
                    actions: dataRecordAlertRuleEntity != undefined ? dataRecordAlertRuleEntity.Actions : undefined,
                    vrActionTargetType: vrActionTargetType
                };

                VRUIUtilsService.callDirectiveLoad(vRActionManagementAPI, vrActionPayload, vRActionManagementLoadDeferred);
            });

            return vRActionManagementLoadDeferred.promise;
        };
        function loadRecordFilterDirective() {
            var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            recordFilterDirectiveReadyDeferred.promise.then(function () {
                var recordFilterDirectivePayload = {};
                recordFilterDirectivePayload.context = buildRecordFilterContext(recordfields);
                if (dataRecordAlertRuleEntity != undefined) {
                    recordFilterDirectivePayload.FilterGroup = dataRecordAlertRuleEntity.FilterGroup;
                }

                VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
            });

            return recordFilterDirectiveLoadDeferred.promise;
        };

        function insert() {
            if ($scope.onDataRecordAlertRuleAdded != undefined)
                $scope.onDataRecordAlertRuleAdded(buildDataRecordAlertRuleObjFromScope());
            $scope.modalContext.closeModal();
        };
        function update() {
            if ($scope.onDataRecordAlertRuleUpdated != undefined) {
                $scope.onDataRecordAlertRuleUpdated(buildDataRecordAlertRuleObjFromScope());
            }
            $scope.modalContext.closeModal();
        };

        function buildDataRecordAlertRuleObjFromScope() {
            var recordFilterData = recordFilterDirectiveAPI.getData();
            var actions = vRActionManagementAPI.getData();

            return {
                Entity: {
                    FilterGroup: recordFilterData.filterObj,
                    Actions: actions
                },
                FilterExpression: recordFilterData.expression,
                ActionNames: buildActionNames(actions)
            }
        };

        function getContext() {
            var currentContext = context;

            if (currentContext == undefined)
                currentContext = {};

            return currentContext;
        };

        function buildActionNames(actions) {
            if (actions == undefined || actions == null || actions.length == 0)
                return null;

            var actionNames = [];
            for (var x = 0; x < actions.length; x++) {
                var currentAction = actions[x];
                actionNames.push(currentAction.ActionName);
            }

            return actionNames.join();
        };

        function buildRecordFilterContext(outputFields) {
            var context = {
                getFields: function () {
                    var fields = [];
                    if (outputFields) {
                        for (var i = 0 ; i < outputFields.length; i++) {
                            var field = outputFields[i];
                            fields.push({
                                FieldName: field.Name,
                                FieldTitle: field.Title,
                                Type: field.Type
                            })
                        }
                    }
                    return fields;
                }
            };
            return context;
        };
    }

    appControllers.controller('VR_GenericData_DataRecordAlertRuleEditorController', DataRecordAlertRuleEditorController);

})(appControllers);