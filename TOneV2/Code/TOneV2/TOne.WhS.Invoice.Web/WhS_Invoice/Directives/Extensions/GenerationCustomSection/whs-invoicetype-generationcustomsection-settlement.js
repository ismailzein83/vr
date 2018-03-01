"use strict";

app.directive("whsInvoicetypeGenerationcustomsectionSettlement", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_FinancialAccountAPIService", "WhS_BE_CommisssionTypeEnum",
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

                var ctor = new CustomerGenerationCustomSection($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlSettlementGeneration",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/Extensions/GenerationCustomSection/Templates/SettlementGenerationCustomSectionTemplate.html"
        };

        function CustomerGenerationCustomSection($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;

            var oldCommission;
            var selectedCommissionTypeReadyDeferred;
            function initializeController() {
                $scope.scopeModel = {};
              
                $scope.scopeModel.commissionTypes = UtilsService.getArrayEnum(WhS_BE_CommisssionTypeEnum);

                $scope.scopeModel.selectedCommissionType = WhS_BE_CommisssionTypeEnum.Display;

                $scope.scopeModel.onCommissionFocusChanged = function () {
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
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    console.log(payload);

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

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        selectedCommissionTypeReadyDeferred = undefined;
                    });
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Entities.SettlementGenerationCustomSectionPayload,TOne.WhS.Invoice.Entities",
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
