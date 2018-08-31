'use strict';

app.directive('vrCommonCustomclasstype', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomClassTypeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'customCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/Common/Directives/VRNamespace/Templates/VRCustomClassTypeTemplate.html"
        };

        function CustomClassTypeDirectiveCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        $scope.scopeModel.namespace = payload.Namespace;
                        $scope.scopeModel.className = payload.ClassName;
                        $scope.scopeModel.assemblyName = payload.AssemblyName;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Namespace: $scope.scopeModel.namespace,
                        ClassName: $scope.scopeModel.className,
                        AssemblyName: $scope.scopeModel.assemblyName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);