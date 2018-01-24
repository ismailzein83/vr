'use strict';

app.directive('vrWhsRoutingExcludedcodes', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ExcludedCodesCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RoutingExcludedDestinations/Templates/RoutingExcludedCodesTemplate.html';
            }
        };

        function ExcludedCodesCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var excludedDestinations;
            var isLinkedRouteRule;
            var linkedCode;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.excludedCodes = [];

                $scope.scopeModel.addExcludedCode = function () {
                    var codeIsValid = true;

                    if ($scope.scopeModel.excludedCode == undefined || $scope.scopeModel.excludedCode.length == 0) {
                        codeIsValid = false;
                    }
                    else {
                        angular.forEach($scope.scopeModel.excludedCodes, function (item) {
                            if ($scope.scopeModel.excludedCode === item) {
                                codeIsValid = false;
                            }
                        });
                    }

                    if (codeIsValid)
                        $scope.scopeModel.excludedCodes.push($scope.scopeModel.excludedCode);
                };

                $scope.scopeModel.validateExcludedCodes = function () {
                    if (isLinkedRouteRule) {
                        if ($scope.scopeModel.excludedCodes != null && $scope.scopeModel.excludedCodes.length > 0) {
                            for (var x = 0; x < $scope.scopeModel.excludedCodes.length; x++) {
                                var currentExcludedCode = $scope.scopeModel.excludedCodes[x];
                                if (linkedCode == currentExcludedCode) {
                                    return linkedCode + ' cannot be excluded.';
                                }
                            }
                        }
                    }
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        excludedDestinations = payload.excludedDestinations;
                        isLinkedRouteRule = payload.isLinkedRouteRule;
                        linkedCode = payload.linkedCode;
                    }

                    if (excludedDestinations != undefined && excludedDestinations.Codes != undefined) {
                        $scope.scopeModel.excludedCodes = excludedDestinations.Codes;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.ExcludedCodes, TOne.WhS.Routing.Business",
                        Codes: $scope.scopeModel.excludedCodes.length > 0 ? $scope.scopeModel.excludedCodes : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);