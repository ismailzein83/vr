//"use strict";

//app.directive("vrWhsBeCodegroupSubview", ["UtilsService", "WhS_BE_CodeGroupService",
//    function (UtilsService, WhS_BE_CodeGroupService) {

//        var directiveDefinitionObject = {

//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var codeGroupGrid = CodeGroupGrid($scope, ctrl, $attrs);
//                codeGroupGrid.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CodeGroup/Templates/CodeGroupSubviewTemplate.html"

//        };

//        function CodeGroupGrid($scope, ctrl, $attrs) {

//            var codeGroupGridAPI;
//            var codeGroupGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//            var countryItem;
//            return { initializeController: initializeController };
//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.onCodeGroupGridDirectiveReady = function (api) {
//                    codeGroupGridAPI = api;
//                    codeGroupGridReadyPromiseDeferred.resolve();
//                };
//                $scope.scopeModel.addCodeGroup = function () {

//                    var onCodeGroupAdded = function (codeGroupObj) {
//                        if (codeGroupGridAPI != undefined) {
//                            codeGroupGridAPI.onCodeGroupAdded(codeGroupObj);
//                        }
//                    };
//                    WhS_BE_CodeGroupService.addCodeGroup(onCodeGroupAdded, countryItem.Entity.CountryId);
//                }
//                defineAPI()
//            }

//            function defineAPI() {

//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    if (payload != undefined) {
//                        countryItem = payload.countryItem;
//                        var loadPromiseDeferred = UtilsService.createPromiseDeferred();
//                        promises.push(loadPromiseDeferred.promise);
//                        codeGroupGridReadyPromiseDeferred.promise.then(function () {
//                            codeGroupGridAPI.loadGrid(payload.query).then(function () {
//                                loadPromiseDeferred.resolve();

//                            });
//                        });
//                    }
//                    var rootPromiseNode = { promises: promises };
//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };


//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//                return api;
//            }

//        }

//        return directiveDefinitionObject;

//    }]);