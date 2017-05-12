'use strict';

app.directive('retailBeDidnumbertypeNumber', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new numberCtor(ctrl, $scope);
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
                return '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/DID/Templates/DIDNumber.html';
            }
        };

        function numberCtor(ctrl, $scope) {

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.didObj != undefined && payload.didObj.Settings != undefined
                        && payload.didObj.Settings.Numbers != undefined && payload.didObj.Settings.Numbers.length > 0) {
                        $scope.scopeModel.numbers = payload.didObj.Settings.Numbers;
                    }
                };

                api.setData = function (didObj) {
                    if (didObj.Settings == undefined)
                        didObj.Settings = {};

                    didObj.Settings.Numbers = $scope.scopeModel.numbers;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
        return directiveDefinitionObject;
    }
]);