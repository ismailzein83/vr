(function (appControllers) {

    "use strict";

    SMSMessageTemplateEditorController.$inject = ['$scope', 'VRCommon_SMSMessageTemplateAPIService', 'VRCommon_SMSMessageTypeAPIService', 'VRCommon_VRComponentTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function SMSMessageTemplateEditorController($scope, VRCommon_SMSMessageTemplateAPIService, VRCommon_SMSMessageTypeAPIService, VRCommon_VRComponentTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var smsMessageTemplateId;
        var smsMessageTemplateEntity;
        var smsMessageTypeId;
        var fixedSMSMessageTypeId;

        var smsMessageTypeSelectorAPI;
        var smsMessageTypeSelectoReadyDeferred = UtilsService.createPromiseDeferred();
        var onSMSMessageTypeSelectionChangedDeferred;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();
        var drillDownManager;

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                smsMessageTemplateId = parameters.smsMessageTemplateId;
                fixedSMSMessageTypeId = parameters.smsMessageTypeId;
            }
            isEditMode = (smsMessageTemplateId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.objects = [];
            $scope.scopeModel.menuActions = [];
            $scope.scopeModel.disabledSMSMessageType = fixedSMSMessageTypeId != undefined;

            $scope.scopeModel.onSMSMessageTypeSelectorReady = function (api) {
                smsMessageTypeSelectorAPI = api;
                smsMessageTypeSelectoReadyDeferred.resolve();
            };
            $scope.scopeModel.onSMSMessageTypeSelectionChanged = function () {
                smsMessageTypeId = smsMessageTypeSelectorAPI.getSelectedIds();
                if (smsMessageTypeId != undefined) {
                    if (onSMSMessageTypeSelectionChangedDeferred != undefined) {
                        onSMSMessageTypeSelectionChangedDeferred.resolve();
                    }
                    else {
                        $scope.scopeModel.isGridLoading = true;
                        getSMSMessageType(undefined);
                    }
                }
            };
            
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI, $scope.scopeModel.menuActions, true);
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getSMSMessageTemplate().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSMSMessageTypeSelector, loadGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var smsMessageTemplateName = (smsMessageTemplateEntity != undefined) ? smsMessageTemplateEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(smsMessageTemplateName, 'SMS Message Template');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('SMS Message Template');
                }
            }
            function loadStaticData() {
                if (smsMessageTemplateEntity == undefined)
                    return;

                $scope.scopeModel.name = smsMessageTemplateEntity.Name;

                if (smsMessageTemplateEntity.Settings != undefined) {
                    $scope.scopeModel.mobileNumber = smsMessageTemplateEntity.Settings.MobileNumber != undefined ? smsMessageTemplateEntity.Settings.MobileNumber.ExpressionString : undefined;
                    $scope.scopeModel.message = smsMessageTemplateEntity.Settings.Message.ExpressionString;
 
                }
            }
            function loadSMSMessageTypeSelector() {
                var smsMessageTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                smsMessageTypeSelectoReadyDeferred.promise.then(function () {
                    var smsMessageTypeSelectorPayload = null;
                    if (isEditMode) {
                        smsMessageTypeSelectorPayload = {
                            selectedIds: smsMessageTemplateEntity.SMSMessageTypeId
                        };
                    }
                    if (fixedSMSMessageTypeId != undefined) {
                        smsMessageTypeSelectorPayload = {
                            selectedIds: fixedSMSMessageTypeId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(smsMessageTypeSelectorAPI, smsMessageTypeSelectorPayload, smsMessageTypeSelectorLoadDeferred);
                });
                return smsMessageTypeSelectorLoadDeferred.promise;
            }
            function loadGrid() {
                if (!isEditMode) return;

                onSMSMessageTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                var gridLoadDeferred = UtilsService.createPromiseDeferred();

                onSMSMessageTypeSelectionChangedDeferred.promise.then(function () {
                    gridReadyDeferred.promise.then(function () {
                        onSMSMessageTypeSelectionChangedDeferred = undefined;
                        if (smsMessageTypeId != undefined)
                            getSMSMessageType(gridLoadDeferred);
                    });
                });

                return gridLoadDeferred.promise;
            }
        }

        function getSMSMessageTemplate() {
            return VRCommon_SMSMessageTemplateAPIService.GetSMSMessageTemplate(smsMessageTemplateId).then(function (response) {
                smsMessageTemplateEntity = response;
            });
        }
        function getSMSMessageType(loadPromiseDeferred) {
            return VRCommon_VRComponentTypeAPIService.GetVRComponentType(smsMessageTypeId).then(function (response) {
                var smsMessageTypeEntity = response;
                
                if (smsMessageTypeEntity.Settings != undefined) {
                    $scope.scopeModel.objects = [];
                   
                    var smsMessageTypeObjects = smsMessageTypeEntity.Settings.Objects;
                    var smsMessageTypeObject;
                    for (var key in smsMessageTypeObjects) {
                        if (key != "$type") {
                            smsMessageTypeObject = smsMessageTypeObjects[key];
                            $scope.scopeModel.objects.push(smsMessageTypeObject);

                            drillDownManager.setDrillDownExtensionObject(smsMessageTypeObject);
                        }
                    }
                }
                $scope.scopeModel.isGridLoading = false;
                if (loadPromiseDeferred != undefined)
                    loadPromiseDeferred.resolve();
            });
        }
        
        function buildDrillDownTabs() {
            var drillDownTabs = [];

            drillDownTabs.push(buildObjectTypePropertiesTab());

            function buildObjectTypePropertiesTab() {
                var objectTypePropertiesTab = {};

                objectTypePropertiesTab.title = 'Properties';
                objectTypePropertiesTab.directive = 'vr-common-objecttypeproperty-grid';

                objectTypePropertiesTab.loadDirective = function (objectTypePropertyGridAPI, smsMessageTypeObject) {
                    smsMessageTypeObject.objectTypePropertyGridAPI = objectTypePropertyGridAPI;
                    var objectTypePropertyGridPayload = {
                        objectVariable: smsMessageTypeObject
                    };
                    return smsMessageTypeObject.objectTypePropertyGridAPI.load(objectTypePropertyGridPayload);
                };

                return objectTypePropertiesTab;
            }

            return drillDownTabs;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_SMSMessageTemplateAPIService.AddSMSMessageTemplate(buildSMSMessageTemplateObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('SMSMessageTemplate', response, 'Name')) {
                    if ($scope.onSMSMessageTemplateAdded != undefined)
                        $scope.onSMSMessageTemplateAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_SMSMessageTemplateAPIService.UpdateSMSMessageTemplate(buildSMSMessageTemplateObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('SMSMessageTemplate', response, 'Name')) {
                    if ($scope.onSMSMessageTemplateUpdated != undefined) {
                        $scope.onSMSMessageTemplateUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSMSMessageTemplateObjFromScope() {

            return {
                SMSMessageTemplateId: smsMessageTemplateEntity != undefined ? smsMessageTemplateEntity.SMSMessageTemplateId : undefined,
                Name: $scope.scopeModel.name,
                SMSMessageTypeId: smsMessageTypeSelectorAPI.getSelectedIds(),
                Settings: {
                    MobileNumber: { ExpressionString: $scope.scopeModel.mobileNumber },
                    Message: { ExpressionString: $scope.scopeModel.message }
                }
            };
        }
    }

    appControllers.controller('VRCommon_SMSMessageTemplateEditorController', SMSMessageTemplateEditorController);

})(appControllers);