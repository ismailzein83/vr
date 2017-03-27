'use strict';

app.directive('vrAccountbalanceAccounttypeSourceInvoicesummary', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var invoiceSourceSetting = new InvoiceSourceSetting($scope, ctrl, $attrs);
                invoiceSourceSetting.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_InvToAccBalanceRelation/Elements/InvToAccBalanceRelationDefinition/Directives/MainExtensions/Templates/InvoiceSummarySourceSetting.html'
        };

        function InvoiceSourceSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
         
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.InvToAccBalanceRelation.Business.InvoiceSummaryFieldSourceSetting, Vanrise.InvToAccBalanceRelation.Business",
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
