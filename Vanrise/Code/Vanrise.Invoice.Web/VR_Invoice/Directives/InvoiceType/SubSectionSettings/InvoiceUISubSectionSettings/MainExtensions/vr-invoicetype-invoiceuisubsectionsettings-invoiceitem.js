"use strict";

app.directive("vrInvoicetypeInvoiceuisubsectionsettingsInvoiceitem", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceItemSubSection($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/SubSectionSettings/InvoiceUISubSectionSettings/MainExtensions/Templates/InvoiceItemSubSectionTemplate.html"

        };

        function InvoiceItemSubSection($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var subSectionGridColumnsAPI;
            var subSectionGridColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var invoiceItemSubSectionGridColumnsAPI;
            var invoiceItemSubSectionGridColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSubSectionGridColumnsReady = function(api)
                {
                    subSectionGridColumnsAPI = api;
                    subSectionGridColumnsReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.onInvoiceItemSubSectionGridColumnsReady = function(api)
                {
                    invoiceItemSubSectionGridColumnsAPI = api;
                    invoiceItemSubSectionGridColumnsReadyPromiseDeferred.resolve();

                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.itemSetName = payload.ItemSetName;
                    }
                    var promises = [];
                    var subSectionGridColumnsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    subSectionGridColumnsReadyPromiseDeferred.promise.then(function () {
                        var gridColumnsDirectivePayload = payload != undefined ? { gridColumns: payload.GridColumns } : undefined;
                        VRUIUtilsService.callDirectiveLoad(subSectionGridColumnsAPI, gridColumnsDirectivePayload, subSectionGridColumnsDeferredLoadPromiseDeferred);
                    });
                    promises.push(subSectionGridColumnsDeferredLoadPromiseDeferred.promise);

                    var invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceItemSubSectionGridColumnsReadyPromiseDeferred.promise.then(function () {
                        var invoiceItemDirectivePayload = payload != undefined ? { subSections: payload.SubSections } : undefined;
                        VRUIUtilsService.callDirectiveLoad(invoiceItemSubSectionGridColumnsAPI, invoiceItemDirectivePayload, invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred);
                    });
                    promises.push(invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.Business.InvoiceItemSubSection ,Vanrise.Invoice.Business",
                        ItemSetName: $scope.scopeModel.itemSetName,
                        GridColumns: subSectionGridColumnsAPI.getData(),
                        SubSections: invoiceItemSubSectionGridColumnsAPI.getData()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);