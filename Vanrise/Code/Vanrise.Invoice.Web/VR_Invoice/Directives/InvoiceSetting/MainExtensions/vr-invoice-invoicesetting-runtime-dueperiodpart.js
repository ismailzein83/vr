'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeDueperiodpart', ['UtilsService', 'VRUIUtilsService',
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

                var ctor = new RowCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/DuePeriodPartTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined && payload.fieldValue != undefined)
                    {
                        $scope.scopeModel.duePeriod = payload.fieldValue.DuePeriod;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.InvoiceSettingParts.DuePeriodInvoiceSettingPart,Vanrise.Invoice.MainExtensions",
                        DuePeriod: $scope.scopeModel.duePeriod
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