'use strict';

app.directive('retailZajilInvoicegpinfoConvertorEditor', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cstr = new ZajilInvoicegpinfoConvertorEditor($scope, ctrl, $attrs);
                cstr.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Zajil/Directives/MainExtensions/InvoiceGPInfo/Templates/InvoiceGPInfoTemplate.html'
        };

        function ZajilInvoicegpinfoConvertorEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};


                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var invoiceTypeId;

                    if (payload != undefined) {
                        invoiceTypeId = payload.InvoiceTypeId;
                        $scope.scopeModel.sourceIdColumn = payload.SourceIdColumn;
                        $scope.scopeModel.gpReferenceNumberColumn = payload.GPReferenceNumberColumn;
                    }
                   
                    var promises = [];
                   
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.Zajil.MainExtensions.Convertors.InvoiceGPInfoConvertor, Retail.Zajil.MainExtensions",                   
                        SourceIdColumn: $scope.scopeModel.sourceIdColumn,
                        GPReferenceNumberColumn: $scope.scopeModel.gpReferenceNumberColumn
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
