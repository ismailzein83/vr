'use strict';

app.directive('vrWhsRoutingExcludedranges', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ExcludedRangesCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RoutingExcludedDestinations/Templates/RoutingExcludedRangesTemplate.html';
            }
        };

        function ExcludedRangesCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var excludedDestinations;
            var isLinkedRouteRule;
            var linkedCode;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.excludedRanges = [];

                $scope.scopeModel.addExcludedCode = function () {
                    var rangeIsValid = true;

                    var fromCode = $scope.scopeModel.fromCode;
                    var toCode = $scope.scopeModel.toCode;

                    if (fromCode.length == 0 || toCode.length == 0 || fromCode.length != toCode.length || fromCode == toCode) {
                        rangeIsValid = false;
                    }
                    else {
                        angular.forEach($scope.scopeModel.excludedRanges, function (item) {
                            if (fromCode == item.FromCode && toCode == item.ToCode) {
                                rangeIsValid = false;
                            }
                        });
                    }

                    if (rangeIsValid) {
                        $scope.scopeModel.excludedRanges.push({ FromCode: fromCode, ToCode: toCode });
                    }
                };

                $scope.scopeModel.disableButton = function () {
                    var fromCode = $scope.scopeModel.fromCode;
                    var toCode = $scope.scopeModel.toCode;

                    if (fromCode == undefined || toCode == undefined)
                        return true;

                    if (fromCode.length == 0 || toCode.length == 0)
                        return true;

                    if (fromCode.length == toCode.length && fromCode < toCode)
                        return false;

                    return true;
                };

                $scope.scopeModel.validateExcludedRange = function () {
                    var fromCode = $scope.scopeModel.fromCode;
                    var toCode = $scope.scopeModel.toCode;

                    if (fromCode == undefined || toCode == undefined)
                        return null;

                    if (fromCode.length != toCode.length)
                        return "From and To Codes should have same lengths";

                    if (fromCode >= toCode)
                        return "From Code should be less than To Code";

                    return null;
                };

                $scope.scopeModel.validateExcludedRanges = function () {
                    if (isLinkedRouteRule) {
                        if ($scope.scopeModel.excludedRanges != undefined) {
                            for (var x = 0; x < $scope.scopeModel.excludedRanges.length; x++) {
                                var codeRange = $scope.scopeModel.excludedRanges[x];
                                if (codeRange.FromCode.length != linkedCode.length || codeRange.ToCode.length != linkedCode.length)
                                    continue;

                                if (linkedCode >= codeRange.FromCode && linkedCode <= codeRange.ToCode)
                                    return linkedCode + ' cannot be excluded.';
                            }
                        }
                    }

                    if ($scope.scopeModel.excludedRanges == undefined || $scope.scopeModel.excludedRanges.length == 0)
                        return 'At least one Excluded Range should be added';

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

                    if (excludedDestinations != undefined && excludedDestinations.CodeRanges != undefined) {
                        $scope.scopeModel.excludedRanges = excludedDestinations.CodeRanges;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.ExcludedRanges, TOne.WhS.Routing.Business",
                        CodeRanges: $scope.scopeModel.excludedRanges
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);