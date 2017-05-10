"use strict";
app.directive("partnerportalInvoiceInvoicetileruntimesettings", ["UtilsService", "VRUIUtilsService", "PartnerPortal_Invoice_InvoiceAPIService",
    function (UtilsService, VRUIUtilsService, PartnerPortal_Invoice_InvoiceAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
                title:'='
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
                        return  PartnerPortal_Invoice_InvoiceAPIService.GetRemoteLastInvoice(definitionSettings.VRConnectionId,definitionSettings.InvoiceTypeId).then(function(response){
                            if(response != undefined)
                            {
                                if (response.Entity != undefined)
                                    $scope.scopeModel.dueDate = UtilsService.createDateFromString(response.Entity.DueDate);
                                if(response.Items != undefined)
                                {
                                    for (var i = 0, length = response.Items.length ; i < length; i++)
                                    {
                                        var item = response.Items[i];
                                        if (item.FieldName == "TotalAmount")
                                        {
                                            $scope.scopeModel.invoiceBalance = item.Description;
                                        } else if (item.FieldName == "CurrencyId")
                                        {
                                            $scope.scopeModel.invoiceCurrency = item.Description;
                                        }
                                    }
                                }
                            }

                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);