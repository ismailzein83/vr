//"use strict";

//app.directive("demoBestpracticesChildChildshapeSquare", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
//    function (UtilsService, VRNotificationService, VRUIUtilsService) {

//        var directiveDefinitionObject = {

//            restrict: "E",
//            scope:
//            {
//                onReady: "=",
//            },

//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;

//                var ctor = new SquareShape($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },

//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/Demo_BestPractices/Elements/Child/Directives/MainExtensions/Templates/SquareShapeTemplate.html"
//        };

//        function SquareShape($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            function initializeController() {
//                $scope.scopeModel = {};
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    if (payload != undefined && payload.childShapeEntity != undefined) {
//                        $scope.scopeModel.width = payload.childShapeEntity.Width;
//                        $scope.scopeModel.height = payload.childShapeEntity.Height;
//                    }
//                    var promises = [];
//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    return {
//                        $type: "Demo.BestPractices.MainExtentions.Child.SquareShape,Demo.BestPractices.MainExtentions",
//                        Width: $scope.scopeModel.width,
//                        Height: $scope.scopeModel.height,
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }
//        }

//        return directiveDefinitionObject;

//    }
//]);