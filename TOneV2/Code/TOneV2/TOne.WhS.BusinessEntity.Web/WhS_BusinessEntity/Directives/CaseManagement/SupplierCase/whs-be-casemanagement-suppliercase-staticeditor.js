'use strict';

app.directive('whsBeCasemanagementSuppliercaseStaticeditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new supplierCaseStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CaseManagement/SupplierCase/Templates/SupplierCaseStaticEditorDefinitionTemplate.html"
        };

        function supplierCaseStaticEditor(ctrl, $scope, $attrs) {

            var selectedValues;

            var supplierSelectorAPI;
            var supplierSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedSupplierPromiseDeferred;

            var supplierZoneSelectorApi;
            var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            
            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSupplierSelectorReady = function (api) {
                    supplierSelectorAPI = api;
                    supplierSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onStatusSelectorReady = function (api) {
                    statusSelectorAPI = api;
                    statusSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.isEditMode = false;
                $scope.scopeModel.onSupplierZoneSelectorReady = function (api) {
                    supplierZoneSelectorApi = api;
                    supplierZoneReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierSelectionChanged = function (value) {
                    if(value != undefined)
                    {
                        if (selectedSupplierPromiseDeferred != undefined)
                            selectedSupplierPromiseDeferred.resolve();
                        else
                        {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingSupplierZoneDirective = value; };
                            var payload = {
                                supplierId: supplierSelectorAPI.getSelectedIds()
                            }
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneSelectorApi, payload, setLoader);
                        }
                    }
                };
                UtilsService.waitMultiplePromises([supplierSelectorReadyPromiseDeferred.promise, statusSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineApi();
                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if(selectedValues != undefined)
                        {
                            $scope.scopeModel.isEditMode = true;
                            if (selectedValues.SupplierId != undefined && selectedValues.SupplierZoneId != undefined)
                                 selectedSupplierPromiseDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModel.fromDate = selectedValues.FromDate;
                            $scope.scopeModel.toDate=  selectedValues.ToDate;
                            $scope.scopeModel.attempts= selectedValues.Attempts;
                            $scope.scopeModel.asr = selectedValues.ASR;
                            $scope.scopeModel.acd = selectedValues.ACD;
                            $scope.scopeModel.carrierReference = selectedValues.CarrierReference;
                            $scope.scopeModel.description = selectedValues.Description;
                            $scope.scopeModel.notes = selectedValues.Notes;
                        }
                    }
                    var promises = [];
                    promises.push(loadSupplierSelector());
                    promises.push(loadStatusSelector());

                    if (selectedValues != undefined && selectedValues.SupplierId != undefined && selectedValues.SupplierZoneId != undefined)
                    {
                        promises.push(loadSupplierZoneSelector());
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.setData = function (caseManagementObject) {
                    caseManagementObject.SupplierId = supplierSelectorAPI.getSelectedIds();
                    caseManagementObject.SupplierZoneId = supplierZoneSelectorApi.getSelectedIds();
                    caseManagementObject.FromDate = $scope.scopeModel.fromDate;
                    caseManagementObject.ToDate = $scope.scopeModel.toDate;
                    caseManagementObject.Attempts = $scope.scopeModel.attempts;
                    caseManagementObject.ASR = $scope.scopeModel.asr;
                    caseManagementObject.ACD = $scope.scopeModel.acd;
                    caseManagementObject.CarrierReference = $scope.scopeModel.carrierReference;
                    caseManagementObject.Description = $scope.scopeModel.description;
                    caseManagementObject.Notes = $scope.scopeModel.notes;
                    caseManagementObject.StatusId = statusSelectorAPI.getSelectedIds();

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadStatusSelector() {
                var selectorPayload;
                if (selectedValues != undefined) {
                    selectorPayload = {
                        selectedIds: selectedValues.StatusId,
                        businessEntityDefinitionId: "81289b9a-0a4d-4eef-9a2a-2c5547d97317"
                    }
                }
                return statusSelectorAPI.load(selectorPayload);
            }
            function loadSupplierSelector() {
                var selectorPayload;
                if (selectedValues != undefined) {
                    selectorPayload = {
                        selectedIds: selectedValues.SupplierId
                    }
                }
                return supplierSelectorAPI.load(selectorPayload);
            }

            function loadSupplierZoneSelector()
            {
                var supplierZoneSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([supplierZoneReadyPromiseDeferred.promise, selectedSupplierPromiseDeferred.promise]).then(function () {
                    selectedSupplierPromiseDeferred = undefined;
                    var supplierZonePayload = {
                        selectedIds: selectedValues.SupplierZoneId,
                        supplierId: selectedValues.SupplierId,
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierZoneSelectorApi, supplierZonePayload, supplierZoneSelectorLoadPromiseDeferred);
                });
                return supplierZoneSelectorLoadPromiseDeferred.promise;
            }
           
        }
        return directiveDefinitionObject;
    }]);