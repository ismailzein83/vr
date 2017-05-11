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
                        $scope.scopeModel.number = payload.didObj.Settings.Numbers[0];
                    }
                };

                api.setData = function (didObj) {
                    if (didObj.Settings == undefined)
                        didObj.Settings = {};

                    if (didObj.Settings.Numbers == undefined)
                        didObj.Settings.Numbers = [];

                    if (didObj.Settings.Numbers.length > 0) {
                        didObj.Settings.Numbers.length = 0;
                    }
                    didObj.Settings.Numbers.push($scope.scopeModel.number);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
        return directiveDefinitionObject;
    }
]);