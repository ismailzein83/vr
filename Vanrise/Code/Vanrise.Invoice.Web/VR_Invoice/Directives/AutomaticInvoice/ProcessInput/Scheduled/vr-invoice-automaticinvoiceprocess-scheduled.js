"use strict";

app.directive("vrInvoiceAutomaticinvoiceprocessScheduled", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/VR_Invoice/Directives/AutomaticInvoice/ProcessInput/Scheduled/Templates/AutomaticInvoiceProcessScheduledTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var invoiceTypeSelectorAPI;
        var invoiceTypeSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.invoiceTypeSelectorReady = function (api) {
                invoiceTypeSelectorAPI = api;
                invoiceTypeSelectorAPIReadyDeferred.resolve();
            };
            $scope.endDateOffsetFromToday = 0;
            $scope.issueDateOffsetFromToday = 0;
            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getData = function () {
                return {
                    $type: "Vanrise.Invoice.BP.Arguments.AutomaticInvoiceProcessInput, Vanrise.Invoice.BP.Arguments",
                    InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                    EndDateOffsetFromToday: $scope.endDateOffsetFromToday,
                    IssueDateOffsetFromToday: $scope.issueDateOffsetFromToday
                };
            };

            api.load = function (payload) {
                var promises = [];
                promises.push(loadInvoiceTypeSelector(payload));
                if (payload != undefined && payload.data != undefined) {
                    $scope.endDateOffsetFromToday = payload.data.EndDateOffsetFromToday;
                    $scope.issueDateOffsetFromToday = payload.data.IssueDateOffsetFromToday;
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadInvoiceTypeSelector(payload) {
            var invoiceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            invoiceTypeSelectorAPIReadyDeferred.promise.then(function () {
                var payloadSelector;
                if (payload != undefined && payload.data != undefined) {
                    payloadSelector = { selectedIds: payload.data.InvoiceTypeId }
                }
                VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, payloadSelector, invoiceTypeSelectorLoadDeferred);
            });

            return invoiceTypeSelectorLoadDeferred.promise;
        }
    }

    return directiveDefinitionObject;
}]);
