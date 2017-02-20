'use strict';
app.directive('vrWhsBeCodecriteriagroupSelective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                disableadding: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.codeCriteriaArray = [];

                $scope.addCode = function () {
                    var codeCriteria = {
                        Code: $scope.addedCode,
                        WithSubCodes: false
                    };

                    var codeIsValid = true;

                    if ($scope.addedCode == undefined || $scope.addedCode.length == 0) {
                        codeIsValid = false;
                    }
                    else {
                        angular.forEach($scope.codeCriteriaArray, function (item) {
                            if ($scope.addedCode === item.Code) {
                                codeIsValid = false;
                            }
                        });
                    }

                    if (codeIsValid)
                        $scope.codeCriteriaArray.push(codeCriteria);
                };



                var ctor = new codeCriteriaCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/CodeCriteriaGroup/Templates/SelectiveCodeCriteriaDirectiveTemplate.html';
            }

        };

        function codeCriteriaCtor(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        angular.forEach(payload.Codes, function (item) {
                            $scope.codeCriteriaArray.push(item);
                        });
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups.SelectiveCodeCriteriaGroup, TOne.WhS.BusinessEntity.MainExtensions",
                        Codes: $scope.codeCriteriaArray
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);