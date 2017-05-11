'use strict';

app.directive('retailBeDidnumbertypeRange', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new rangeCtor(ctrl, $scope);
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
                return '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/DID/Templates/DIDRange.html';
            }
        };

        function rangeCtor(ctrl, $scope) {

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.validateRangeData = function () {
                    if ($scope.scopeModel.from == undefined || $scope.scopeModel.to == undefined)
                        return null;

                    if ($scope.scopeModel.from.length != $scope.scopeModel.to.length)
                        return "From value and To value must have same length";

                    if ($scope.scopeModel.to <= $scope.scopeModel.from)
                        return "To value must be greater than From value";

                    return null;
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.didObj != undefined && payload.didObj.Settings != undefined
                        && payload.didObj.Settings.Ranges != undefined && payload.didObj.Settings.Ranges.length > 0) {
                        var range = payload.didObj.Settings.Ranges[0];
                        $scope.scopeModel.from = range.From;
                        $scope.scopeModel.to = range.To;
                    }
                };

                api.setData = function (didObj) {
                    if (didObj.Settings == undefined)
                        didObj.Settings = {};

                    if (didObj.Settings.Ranges == undefined)
                        didObj.Settings.Ranges = [];

                    var rangeObj = { From: $scope.scopeModel.from, To: $scope.scopeModel.to };
                    if (didObj.Settings.Ranges.length > 0) {
                        didObj.Settings.Ranges.length = 0;
                    }
                    didObj.Settings.Ranges.push(rangeObj);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
        return directiveDefinitionObject;
    }
]);