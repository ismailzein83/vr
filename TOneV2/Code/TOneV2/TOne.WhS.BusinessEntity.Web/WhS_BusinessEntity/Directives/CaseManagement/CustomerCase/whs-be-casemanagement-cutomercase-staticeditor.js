'use strict';

app.directive('whsBeCasemanagementCutomercaseStaticeditor', ['UtilsService', 'VRUIUtilsService',
    function (utilsService, vruiUtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new cutomerCaseStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CaseManagement/CustomerCase/Templates/CutomerCaseStaticEditorDefinitionTemplate.html"
        };

        function cutomerCaseStaticEditor(ctrl, $scope, $attrs) {

            var selectedValues;

            var customerSelectorAPI;
            var customerSelectorReadyPromiseDeferred = utilsService.createPromiseDeferred();

            //var saleZoneApi;
            //var saleZoneReadyPromiseDeferred = utilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onCustomerSelectorReady = function (api) {
                    customerSelectorAPI = api;
                    customerSelectorReadyPromiseDeferred.resolve();
                };
                //$scope.scopeModel.onSupplierRuleDefinitionReady = function (api) {
                //    supplierRuleDefinitionReadyApi = api;
                //    supplierRuleDefinitionReadyPromiseDeferred.resolve();
                //};

                utilsService.waitMultiplePromises([customerSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineApi();

                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    console.log(payload);
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if(selectedValues != undefined)
                        {
                            $scope.scopeModel.fromDate = selectedValues.FromDate;
                            $scope.scopeModel.toDate=  selectedValues.ToDate;
                            $scope.scopeModel.attempts= selectedValues.Attempts;
                            $scope.scopeModel.asr = selectedValues.ASR;
                            $scope.scopeModel.acd = selectedValues.ACD;
                            $scope.scopeModel.carrierReference = selectedValues.CarrierReference;
                            $scope.scopeModel.description = selectedValues.Description;
                        }
                    }
                    var promises = [];
                    promises.push(loadCustomerSelector());
                    return utilsService.waitMultiplePromises(promises);
                };
                api.setData = function (caseManagementObject) {
                    caseManagementObject.CutomerId = customerSelectorAPI.getSelectedIds();
                    caseManagementObject.FromDate = $scope.scopeModel.fromDate;
                    caseManagementObject.ToDate = $scope.scopeModel.toDate;
                    caseManagementObject.Attempts = $scope.scopeModel.attempts;
                    caseManagementObject.ASR = $scope.scopeModel.asr;
                    caseManagementObject.ACD = $scope.scopeModel.acd;
                    caseManagementObject.CarrierReference = $scope.scopeModel.carrierReference;
                    caseManagementObject.Description = $scope.scopeModel.description;
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadCustomerSelector() {
                var selectorPayload;
                if (selectedValues != undefined) {
                    selectorPayload = {
                        selectedIds: selectedValues.CutomerId
                    }
                }
                return customerSelectorAPI.load(selectorPayload);
            }
        }
        return directiveDefinitionObject;
    }]);