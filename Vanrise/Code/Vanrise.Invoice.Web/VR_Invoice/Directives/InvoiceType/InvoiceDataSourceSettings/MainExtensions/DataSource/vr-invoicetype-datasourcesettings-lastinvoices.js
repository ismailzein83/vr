"use strict";

app.directive("vrInvoicetypeDatasourcesettingsLastinvoices", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new LastInvoicesDataSourceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceDataSourceSettings/MainExtensions/DataSource/Templates/LastInvoicesDataSourceSettings.html"

        };

        function LastInvoicesDataSourceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
              
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var dataSourceEntity;
                    if (payload != undefined) {
                        dataSourceEntity = payload.dataSourceEntity;
                        if (dataSourceEntity != undefined)
                        $scope.scopeModel.lastInvoices = dataSourceEntity.LastInvoices;
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.LastInvoicesDataSourceSettings ,Vanrise.Invoice.MainExtensions",
                        LastInvoices: $scope.scopeModel.lastInvoices
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);