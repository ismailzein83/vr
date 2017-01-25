"use strict";

app.directive("vrWhsSalesTqiSelective", ["WhS_Sales_RatePlanAPIService", "UtilsService", "VRUIUtilsService",
function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var selectiveCtrl = this;
            var tqiMethods = new TQIMethods(selectiveCtrl, $scope);
            tqiMethods.initializeController();
        },
        controllerAs: "selectiveCtrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/TQI/Templates/TQISelectiveTemplate.html"
    };

    function TQIMethods(selectiveCtrl, $scope) {
        this.initializeController = initializeController;

        var directiveAPI;
        var rpRouteDetail;
        var context;
       
        function initializeController() {
            selectiveCtrl.tqiMethods = [];
            selectiveCtrl.selectedTQIMethod;

            selectiveCtrl.onTQIMethodsSelectorReady = function (api) {
                defineAPI();
            };

            selectiveCtrl.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.isLoadingDirective = value;
                };

                var directivePayload = {
                    rpRouteDetail: rpRouteDetail,
                    context: context
                };

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, undefined);
            }

        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    rpRouteDetail = payload.rpRouteDetail;
                    context = payload.context;
                }

                var loadTQIMethodsPromise = loadTQIMethods();
                promises.push(loadTQIMethodsPromise);

                function loadTQIMethods() {
                    return WhS_Sales_RatePlanAPIService.GetTQIMethods().then(function (tqiMethods) {
                        if (tqiMethods != null) {
                            for (var i = 0; i < tqiMethods.length; i++) {
                                selectiveCtrl.tqiMethods.push(tqiMethods[i]);
                            }
                        }
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
               return  directiveAPI.getData()
            };

            if (selectiveCtrl.onReady && typeof selectiveCtrl.onReady == "function")
                selectiveCtrl.onReady(api);
        }
    }
}]);
