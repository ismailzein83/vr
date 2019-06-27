"use strict";
app.directive("vrInvoiceGenericinvoiceaccountSelector", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                ismultipleselection: "@",
                normalColNum: '@',
                isrequired: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var businessObject = new GenericInvoiceAccount($scope, ctrl, $attrs);
                businessObject.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };


        function GenericInvoiceAccount($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var businessEntityDefinitionId;
            var context;
            var selectedIds;

            var genericFinancialAccountSelectorAPI;
            var genericFinancialAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.InvoiceAccountSelectorReady = function (api) {
                    genericFinancialAccountSelectorAPI = api;
                    genericFinancialAccountSelectorReadyPromiseDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([genericFinancialAccountSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function reloadContextFunctions() {
                if (context != undefined) {
                    if (context.reloadPregeneratorActions != undefined) {
                        context.reloadPregeneratorActions();
                    }
                }
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        selectedIds = payload.selectedIds;

                        if (payload.extendedSettings != undefined && payload.extendedSettings.Configuration != undefined) {
                            businessEntityDefinitionId = payload.extendedSettings.Configuration.FinancialAccountBEDefinitionId;
                        }
                    }

                    var loadDataProviderSettingsPromiseDeferred = UtilsService.createPromiseDeferred();

                    genericFinancialAccountSelectorReadyPromiseDeferred.promise.then(function () {

                        var genericFinancialAccountPayload = {
                            businessEntityDefinitionId: businessEntityDefinitionId,
                            selectedIds: selectedIds
                        };
                        VRUIUtilsService.callDirectiveLoad(genericFinancialAccountSelectorAPI, genericFinancialAccountPayload, loadDataProviderSettingsPromiseDeferred);
                    });
                    promises.push(loadDataProviderSettingsPromiseDeferred.promise);
                    reloadContextFunctions();
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return genericFinancialAccountSelectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }

        function getTemplate(attrs) {

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            return ' <vr-generic-financialaccount-selector normal-col-num = "{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" ' + multipleselection + '   on-ready="scopeModel.InvoiceAccountSelectorReady" isrequired></vr-generic-financialaccount-selector> ';
        }

        return directiveDefinitionObject;
    }
]);