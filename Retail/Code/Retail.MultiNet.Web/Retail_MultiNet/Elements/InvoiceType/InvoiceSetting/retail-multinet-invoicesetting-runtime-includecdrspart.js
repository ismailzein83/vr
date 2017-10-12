'use strict';
app.directive('retailMultinetInvoicesettingRuntimeIncludecdrspart', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new DirectiveCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_MultiNet/Elements/InvoiceType/InvoiceSetting/Templates/IncludeCDRsPartTemplate.html';
            }

        };

        function DirectiveCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.fieldValue != undefined) {
                        $scope.scopeModel.includeCDRs = payload.fieldValue.IncludeCDRs;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.MultiNet.Business.IncludeCDRsInvoiceSettingPart,Retail.MultiNet.Business",
                        IncludeCDRs: $scope.scopeModel.includeCDRs
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);