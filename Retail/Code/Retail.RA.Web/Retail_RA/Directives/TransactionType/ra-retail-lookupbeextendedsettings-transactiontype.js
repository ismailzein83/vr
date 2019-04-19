'use strict';
app.directive('raRetailLookupbeextendedsettingsTransactiontype', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new LookUpBEExtendedSettings(ctrl, $scope);
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
            templateUrl: '/Client/Modules/Retail_RA/Directives/TransactionType/Templates/TransactionTypeLKUpBEExtendedSettingsTemplate.html'

        };

        function LookUpBEExtendedSettings(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var rootPromiseNode = {
                        promises: []
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.TransactionTypeLKUPBEDefinitionSettings, Retail.RA.Business"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);