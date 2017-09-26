"use strict";
app.directive("partnerportalInvoiceInvoicetileruntimesettings", ["UtilsService", "VRUIUtilsService", "PartnerPortal_Invoice_InvoiceAPIService","VRNavigationService",
    function (UtilsService, VRUIUtilsService, PartnerPortal_Invoice_InvoiceAPIService, VRNavigationService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
                title: '=',
                index :'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoiceTileRuntimeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceTile/Directives/Templates/InvoiceTileRuntimeSettings.html"
        };
        function InvoiceTileRuntimeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
         
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tileTitle = ctrl.title;
                $scope.scopeModel.fields = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var definitionSettings;
                    if (payload != undefined)
                    {
                        definitionSettings = payload.definitionSettings;
                    }
                    if(definitionSettings != undefined)
                    {
                        promises.push(loadLastInvoice());
                    }
                    function loadLastInvoice()
                    {
                        return PartnerPortal_Invoice_InvoiceAPIService.GetRemoteLastInvoice(definitionSettings.VRConnectionId, definitionSettings.InvoiceTypeId,definitionSettings.ViewId).then(function (response) {
                            if (response != undefined) {
                                var invoiceBalance = "";
                                var invoiceCurrency = "";
                                var toDate = "";
                                if (response.InvoiceDetail != undefined) {
                                    if (response.InvoiceDetail.Items != undefined) {
                                      
                                        for (var i = 0, length = response.InvoiceDetail.Items.length ; i < length; i++) {
                                            var item = response.InvoiceDetail.Items[i];
                                            if (item.FieldName == "TotalAmount") {
                                                invoiceBalance = item.Description;
                                            } else if (item.FieldName == "CurrencyId") {
                                                invoiceCurrency = item.Description;
                                            }
                                        }
                                    }
                                   
                                }
                                if (response != undefined) {
                                    toDate = response.FormattedDate;
                                }
                                $scope.scopeModel.fields.push({
                                    name: "Amount",
                                    value: invoiceBalance + " " + invoiceCurrency
                                });
                                $scope.scopeModel.fields.push({
                                        name: "Date",
                                        value: toDate
                                    });
                                $scope.scopeModel.url = response.ViewURL;
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }

        return directiveDefinitionObject;
    }
]);