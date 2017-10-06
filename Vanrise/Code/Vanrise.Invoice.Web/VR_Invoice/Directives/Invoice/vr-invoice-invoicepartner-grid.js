"use strict";

app.directive("vrInvoiceInvoicepartnerGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceAPIService", "VR_Invoice_InvoiceFieldEnum", "VRUIUtilsService", "VR_Invoice_InvoiceService", "VR_Invoice_InvoiceTypeConfigsAPIService", "VR_Invoice_InvoiceActionService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_Invoice_InvoiceFieldEnum, VRUIUtilsService, VR_Invoice_InvoiceService, VR_Invoice_InvoiceTypeConfigsAPIService, VR_Invoice_InvoiceActionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoicepartnerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/InvoicePartnerGridTemplate.html"

        };

        function InvoicepartnerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;

            function initializeController() {

                $scope.invoicePartners = [];

                $scope.isValid = function () {
                    if ($scope.invoicePartners == undefined || $scope.invoicePartners.length == 0)
                        return 'At least one item should exist';

                    return null;
                }

                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (payload) {
                            var query = payload.query;
                            $scope.generationCustomSectionDirective = payload.customPayloadDirective;
                            return gridAPI.retrieveData(query);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    $scope.isLodingGrid = true;
                    var promises = [];
                    return VR_Invoice_InvoiceAPIService.GetFilteredInvoicePartners(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    var currentItem = response.Data[i];
                                    var promise = extendInvoicePartner(currentItem);

                                    function extendInvoicePartner(currentItem) {
                                        currentItem.generationCustomSectionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                                        currentItem.onGenerationCustomSectionDirectiveReady = function (api) {
                                            currentItem.generationCustomSectionDirectiveAPI = api;
                                            var payload = {partnerId:currentItem.PartnerId};
                                            VRUIUtilsService.callDirectiveLoad(currentItem.generationCustomSectionDirectiveAPI, payload, currentItem.generationCustomSectionDirectiveLoadDeferred);
                                        };
                                        return currentItem.generationCustomSectionDirectiveLoadDeferred.promise;
                                    }

                                    promises.push(promise);
                                }
                            }
                            UtilsService.waitMultiplePromises(promises).then(function () { $scope.isLodingGrid = false;});
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }
        }

        return directiveDefinitionObject;

    }
]);