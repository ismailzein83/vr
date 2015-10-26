﻿'use strict';
app.directive('vrWhsBeCodecriteriagroupSelective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.codeCriteriaArray = [];

                $scope.addCode = function () {
                    var codeCriteria = {
                        Code: $scope.addedCode,
                        WithSubCodes: false
                    };

                    $scope.codeCriteriaArray.push(codeCriteria);
                };

                $scope.removeCode = function (codeToRemove) {
                    $scope.codeCriteriaArray.splice($scope.codeCriteriaArray.indexOf(codeToRemove), 1);
                }

                var beCodeCriteriaCtor = new beCodeCriteria(ctrl, $scope);
                beCodeCriteriaCtor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return getBeSelectiveCodeCriteriaTemplate(attrs);
            }

        };

        function getBeSelectiveCodeCriteriaTemplate(attrs) {
            return '/Client/Modules/WhS_BusinessEntity/Directives/CodeCriteria/Templates/SelectiveCodeCriteriaDirectiveTemplate.html';
        }

        function beCodeCriteria(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups.SelectiveCodeCriteriaGroup, TOne.WhS.BusinessEntity.MainExtensions",
                        Codes: $scope.codeCriteriaArray
                    };
                }

                api.setData = function (codeCriteriaGroupSettings) {
                    angular.forEach(codeCriteriaGroupSettings.Codes, function (item) {
                        $scope.codeCriteriaArray.push(item);
                    });
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);