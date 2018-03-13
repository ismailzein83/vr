"use strict";
app.directive("demoModuleStandardDimensions", ["UtilsService",
    function (UtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var standardDimensions = new StandardDimensions($scope, ctrl, $attrs);
                standardDimensions.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Directives/Setting/Dimensions/Templates/StandardDimensionsTemplate.html"
        };
        function StandardDimensions($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            };
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined && payload != null) {
                        $scope.scopeModel.width = payload.Width != null ? payload.Width : null;
                        $scope.scopeModel.length = payload.Length != null ? payload.Length : null;                       
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    return {
                        $type: "Demo.Module.Entities.Standard,Demo.Module.Entities",
                        Width: $scope.scopeModel.width,
                        Length: $scope.scopeModel.length
                        
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);