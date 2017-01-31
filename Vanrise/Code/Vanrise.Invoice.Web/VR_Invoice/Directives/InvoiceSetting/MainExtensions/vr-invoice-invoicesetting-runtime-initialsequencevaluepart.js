'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeInitialsequencevaluepart', ['UtilsService', 'VRUIUtilsService',
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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/InitialSequenceValuePartTemplate.html';
            }

        };

        function DirectiveCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.initialValue = 1;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined && payload.fieldValue != undefined)
                    {
                        $scope.scopeModel.initialValue = payload.fieldValue.InitialValue;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.Entities.InitialSequenceValueSettingPart,Vanrise.Invoice.Entities",
                        InitialValue: $scope.scopeModel.initialValue
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