"use strict";

app.directive("vrInvoicePartnerinvoiecsettingSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_Invoice_PartnerInvoiceSettingService','VR_Invoice_PartnerInvoiceSettingAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VR_Invoice_PartnerInvoiceSettingService, VR_Invoice_PartnerInvoiceSettingAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var partnerInvoiceSettingSearch = new PartnerInvoiceSettingSearch($scope, ctrl, $attrs);
            partnerInvoiceSettingSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Invoice/Directives/PartnerInvoiceSetting/Templates/PartnerInvoiceSettingSearch.html"

    };

    function PartnerInvoiceSettingSearch($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceSettingId;
        var invoiceTypeId;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.linkPartner = function () {
                var onPartnerInvoiceSettingAdded = function (partnerInvoiceSettingObj) {
                    if (gridAPI != undefined) {
                        gridAPI.onPartnerInvoiceSettingAdded(partnerInvoiceSettingObj);
                    }
                };
                VR_Invoice_PartnerInvoiceSettingService.addPartnerInvoiceSetting(onPartnerInvoiceSettingAdded,invoiceTypeId, invoiceSettingId);
            };

            $scope.scopeModel.HasAssignPartnerAccess = function () {
                return VR_Invoice_PartnerInvoiceSettingAPIService.HasAssignPartnerAccess(invoiceSettingId);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridPromiseDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                $scope.scopeModel.isLoading = true;
                var promises = [];
                if (payload != undefined) {
                    invoiceSettingId = payload.invoiceSettingId;
                    invoiceTypeId = payload.invoiceTypeId;
                }
                return UtilsService.waitMultiplePromises(promises).finally(function () {
                    loadPartnerInvoiceSettingGrid();
                    $scope.scopeModel.isLoading = false;
                });
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getFilterObject() {
            var filter = {
                invoiceTypeId:invoiceTypeId,
                query: {
                    InvoiceSettingId: invoiceSettingId
                }
            };
            return filter;
        }

        function loadPartnerInvoiceSettingGrid() {
            return gridPromiseDeferred.promise.then(function () {
                gridAPI.loadGrid(getFilterObject());
            });
        }
    }

    return directiveDefinitionObject;

}]);
