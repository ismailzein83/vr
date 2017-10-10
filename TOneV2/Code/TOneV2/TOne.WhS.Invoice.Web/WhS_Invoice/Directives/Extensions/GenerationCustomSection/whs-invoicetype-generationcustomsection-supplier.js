"use strict";

app.directive("whsInvoicetypeGenerationcustomsectionSupplier", ["UtilsService", "VRNotificationService", "VRUIUtilsService","WhS_BE_FinancialAccountAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_FinancialAccountAPIService) {

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

            var timeZoneSelectorAPI;
            var timeZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onTimeZoneSelectorReady = function (api) {
                    timeZoneSelectorAPI = api;
                    timeZoneSelectorReadyDeferred.resolve();
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
                        context = payload.context;
                    }
                    var promises = [];

                    if (customPayload == undefined) {
                        if (financialAccountId != undefined) {
                            var loadTimeZonePromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(loadTimeZonePromiseDeferred.promise);

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
                        promises.push(loadTimeZoneSelector(customPayload.TimeZoneId));
                    }

                    function loadTimeZoneSelector(selectedIds)
                    {
                        var timeZoneSelectorPayload = {
                            selectedIds: selectedIds
                        };
                        if (invoice != undefined && invoice.Details != undefined) {
                            timeZoneSelectorPayload.selectedIds = invoice.Details.TimeZoneId;
                        }
                        return timeZoneSelectorAPI.load(timeZoneSelectorPayload);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Entities.SupplierGenerationCustomSectionPayload,TOne.WhS.Invoice.Entities",
                        TimeZoneId: timeZoneSelectorAPI.getSelectedIds()
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