(function (appControllers) {

    'use strict';

    itemGroupingSubSectionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function itemGroupingSubSectionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var subSectionEntity;

        var isEditMode;

        var itemGroupingSubSectionSettingsAPI;
        var itemGroupingSubSectionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var recordFilterAPI;
        var recordFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                subSectionEntity = parameters.subSectionEntity;
            }
            isEditMode = (subSectionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onItemGroupingSubSectionSettingsReady = function (api) {
                itemGroupingSubSectionSettingsAPI = api;
                itemGroupingSubSectionSettingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onRecordFilterReady = function (api) {
                recordFilterAPI = api;
                recordFilterReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSubSection() : addeSubSection();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builSubSectionObjFromScope() {
                var filterGroup = recordFilterAPI.getData();
                return {
                    InvoiceSubSectionId: subSectionEntity != undefined && subSectionEntity.InvoiceSubSectionId  != undefined? subSectionEntity.InvoiceSubSectionId : UtilsService.guid(),
                    SectionTitle: $scope.scopeModel.sectionTitle,
                    Settings: itemGroupingSubSectionSettingsAPI.getData(),
                    SubSectionFilter: filterGroup != undefined ? filterGroup.filterObj : undefined,
                };
            }
            function addeSubSection() {
                var subSectionObj = builSubSectionObjFromScope();
                if ($scope.onItemGroupingSubSectionAdded != undefined) {
                    $scope.onItemGroupingSubSectionAdded(subSectionObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateSubSection() {
                var subSectionObj = builSubSectionObjFromScope();
                if ($scope.onItemGroupingSubSectionUpdated != undefined) {
                    $scope.onItemGroupingSubSectionUpdated(subSectionObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && subSectionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(subSectionEntity.SectionTitle, 'Sub Section');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Sub Section');
                }

                function loadStaticData() {
                    if (subSectionEntity != undefined) {
                        $scope.scopeModel.sectionTitle = subSectionEntity.SectionTitle;
                    }
              
                }

                function loadItemGroupingSubSectionSettings() {
                    var itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    itemGroupingSubSectionSettingsReadyPromiseDeferred.promise.then(function () {
                        var itemGroupingDirectivePayload = { context: getContext() };
                        if(subSectionEntity != undefined)
                            itemGroupingDirectivePayload.subSectionSettingsEntity = subSectionEntity.Settings;
                        VRUIUtilsService.callDirectiveLoad(itemGroupingSubSectionSettingsAPI, itemGroupingDirectivePayload, itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred);
                    });
                    return itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred.promise;
                }

                function loadRecordFilterDirective() {
                    var recordFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    recordFilterReadyPromiseDeferred.promise.then(function () {
                        var recordFilterPayload = { context: getContext() };
                        if (subSectionEntity != undefined) {
                            recordFilterPayload.FilterGroup = subSectionEntity.SubSectionFilter;
                        }
                        VRUIUtilsService.callDirectiveLoad(recordFilterAPI, recordFilterPayload, recordFilterLoadPromiseDeferred);
                    });
                    return recordFilterLoadPromiseDeferred.promise;
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadItemGroupingSubSectionSettings, loadRecordFilterDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
            
        }
        function getContext()
        {
            var currentContext = context;
            if(currentContext == undefined)
            {
                currentContext = {};
            }
            currentContext.getFields = function () {
                var fields = [];
                if (context != undefined && context.getItemGroupingId != undefined) {

                    var itemGroupingId = context.getItemGroupingId();
                    var dimensions = context.getGroupingDimensions(itemGroupingId);
                    var measures = context.getGroupingMeasures(itemGroupingId);

                    for (var i = 0; i < dimensions.length; i++) {
                        var dimensionField = dimensions[i];
                        fields.push({ FieldName: dimensionField.FieldName, FieldTitle: dimensionField.FieldDescription, Type: dimensionField.FieldType });
                    }
                    for (var i = 0; i < measures.length; i++) {
                        var measureField = measures[i];
                        fields.push({ FieldName: measureField.FieldName, FieldTitle: measureField.FieldDescription, Type: measureField.FieldType });
                    }
                }
                return fields;
            }
            return currentContext;
        }
    }
    appControllers.controller('VR_Invoice_ItemGroupingSubSectionEditorController', itemGroupingSubSectionEditorController);

})(appControllers);