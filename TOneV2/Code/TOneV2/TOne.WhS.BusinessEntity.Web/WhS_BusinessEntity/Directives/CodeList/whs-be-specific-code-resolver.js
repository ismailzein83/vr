'use strict';
app.directive('whsBeSpecificCodeResolver', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                $scope.codeCriteriaArray = [];

                var ctor = new codeCriteriaCtor($scope, ctrl);


                ctor.initializeController();


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
                    {
                        $scope.codeCriteriaArray.push(codeCriteria);
                        $scope.addedCode = undefined;
                    }
                };
                $scope.isValid = function () {
                    if ($scope.codeCriteriaArray.length==0)
                        return "at least choose one code";
                    return null;
                };


            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CodeList/templates/SpecificCodeResolverTemplate.html"

        };

        function codeCriteriaCtor( $scope,ctrl) {
            this.initializeController = initializeController;
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
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CodeList.SpecificCodeResolver,TOne.WhS.BusinessEntity.MainExtensions",
                        Codes: $scope.codeCriteriaArray
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

          
        }
        return directiveDefinitionObject;
    }]);