"use strict";

app.directive("whsInvoicetypeGenerationcustomsectionSupplier", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_FinancialAccountAPIService", "WhS_BE_CommisssionTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_FinancialAccountAPIService, WhS_BE_CommisssionTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SupplierGenerationCustomSection($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlSupplierGeneration",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/Extensions/GenerationCustomSection/Templates/SupplierGenerationCustomSectionTemplate.html"
        };

        function SupplierGenerationCustomSection($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;

            var oldCommission;

            var timeZoneSelectorAPI;
            var timeZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var emptyTimeZonePromiseDeferred;
            var selectedTimeZoneReadyDeferred;

            var selectedCommissionTypeReadyDeferred;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onTimeZoneSelectorReady = function (api) {
                    timeZoneSelectorAPI = api;
                    timeZoneSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.commissionTypes = UtilsService.getArrayEnum(WhS_BE_CommisssionTypeEnum);
                $scope.scopeModel.selectedCommissionType = WhS_BE_CommisssionTypeEnum.Display;
                $scope.scopeModel.onTimeZoneSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedTimeZoneReadyDeferred != undefined) {
                            selectedTimeZoneReadyDeferred.resolve();
                        }
                        else {
                            onvaluechanged();
                        }
                    }
                    else {
                        if (emptyTimeZonePromiseDeferred != undefined) {
                            emptyTimeZonePromiseDeferred.resolve();
                        }
                        else {
                            onvaluechanged();
                        }
                    }
                };

                $scope.scopeModel.onCommissionFocusChanged = function (value) {
                    if (oldCommission != $scope.scopeModel.commission)
                        onvaluechanged();

                    oldCommission != $scope.scopeModel.commission;
                };

                $scope.scopeModel.onCommissionTypeSelectionChanged = function (value) {
                    if (selectedCommissionTypeReadyDeferred != undefined) {
                        selectedCommissionTypeReadyDeferred.resolve();
                    }
                    else {
                        onvaluechanged();
                    }
                };

                function onvaluechanged() {
                    if (context != undefined && context.onvaluechanged != undefined && typeof (context.onvaluechanged) == 'function') {
                        context.onvaluechanged();
                    }
                };

                UtilsService.waitMultiplePromises([timeZoneSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoice;
                    var financialAccountId;
                    var customPayload;
                    if (payload != undefined) {
                        customPayload = payload.customPayload;
                        invoice = payload.invoice;
                        financialAccountId = payload.partnerId;
                        if (financialAccountId == undefined) {
                            financialAccountId = payload.financialAccountId;
                        }
                        if (customPayload != undefined) {
                            selectedCommissionTypeReadyDeferred = UtilsService.createPromiseDeferred();
                            oldCommission = $scope.scopeModel.commission = customPayload.Commission;
                            $scope.scopeModel.selectedCommissionType = UtilsService.getItemByVal($scope.scopeModel.commissionTypes, customPayload.CommissionType, "value");
                        }
                        if (invoice != undefined && invoice.Details != undefined) {
                            oldCommission = $scope.scopeModel.commission = invoice.Details.Commission;
                            $scope.scopeModel.selectedCommissionType = UtilsService.getItemByVal($scope.scopeModel.commissionTypes, invoice.Details.CommissionType, "value");
                        }
                        context = payload.context;
                    }
                    var promises = [];

                    if (customPayload == undefined) {
                        if (financialAccountId != undefined) {
                            var loadTimeZonePromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(loadTimeZonePromiseDeferred.promise);
                            selectedTimeZoneReadyDeferred = UtilsService.createPromiseDeferred();


                            WhS_BE_FinancialAccountAPIService.GetSupplierTimeZoneId(financialAccountId).then(function (response) {
                                loadTimeZoneSelector(response).then(function () {
                                    loadTimeZonePromiseDeferred.resolve();
                                }).catch(function (error) {
                                    loadTimeZonePromiseDeferred.reject(error);
                                });
                            }).catch(function (error) {
                                loadTimeZonePromiseDeferred.reject(error);
                            });
                        } else {
                            promises.push(loadTimeZoneSelector());
                        }
                    }
                    else {
                        selectedTimeZoneReadyDeferred = UtilsService.createPromiseDeferred();
                        emptyTimeZonePromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(emptyTimeZonePromiseDeferred.promise);

                        promises.push(loadTimeZoneSelector(customPayload.TimeZoneId));
                    }

                    function loadTimeZoneSelector(selectedIds) {
                        var timeZoneSelectorPayload = {
                            selectedIds: selectedIds
                        };
                        if (invoice != undefined && invoice.Details != undefined) {
                            timeZoneSelectorPayload.selectedIds = invoice.Details.TimeZoneId;
                        }

                        if (timeZoneSelectorPayload.selectedIds != undefined && timeZoneSelectorPayload.selectedIds > 0)
                            promises.push(selectedTimeZoneReadyDeferred.promise);

                        return timeZoneSelectorAPI.load(timeZoneSelectorPayload);
                    }
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        selectedTimeZoneReadyDeferred = undefined;
                        emptyTimeZonePromiseDeferred = undefined;
                        selectedCommissionTypeReadyDeferred = undefined;
                    });
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Entities.SupplierGenerationCustomSectionPayload,TOne.WhS.Invoice.Entities",
                        TimeZoneId: timeZoneSelectorAPI.getSelectedIds(),
                        Commission: $scope.scopeModel.commission,
                        CommissionType: $scope.scopeModel.selectedCommissionType != undefined ? $scope.scopeModel.selectedCommissionType.value : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;
    }
]);