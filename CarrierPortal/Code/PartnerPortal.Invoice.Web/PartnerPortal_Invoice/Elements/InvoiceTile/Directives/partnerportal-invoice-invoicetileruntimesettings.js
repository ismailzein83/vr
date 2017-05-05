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
                            $scope.scopeModel.invoiceBalance = response.Details.TotalAmount;
                            $scope.scopeModel.dueDate = response.DueDate;

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