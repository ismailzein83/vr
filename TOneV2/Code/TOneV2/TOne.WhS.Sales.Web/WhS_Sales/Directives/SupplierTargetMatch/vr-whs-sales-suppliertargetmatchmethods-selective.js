"use strict";

app.directive("vrWhsSalesSuppliertargetmatchmethodsSelective", ["WhS_Sales_SupplierTargetMatchAPIService", "UtilsService", "VRUIUtilsService",
function (WhS_Sales_SupplierTargetMatchAPIService, UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "=",
            onselectionchanged: '='
        },
        controller: function ($scope, $element, $attrs) {
            var selectiveCtrl = this;
            var stmMethods = new STMMethods(selectiveCtrl, $scope);
            stmMethods.initializeController();
        },
        controllerAs: "selectiveCtrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/SupplierTargetMatch/Templates/SupplierTargetMatchMethodsSelectiveTemplate.html"
    };

    function STMMethods(selectiveCtrl, $scope) {
        this.initializeController = initializeController;

        var directiveAPI;
        var context;

        function initializeController() {
            selectiveCtrl.methods = [];
            selectiveCtrl.selectedMethod;

            selectiveCtrl.onMethodsSelectorReady = function (api) {
                defineAPI();
            };

            selectiveCtrl.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.isLoadingDirective = value;
                };

                var directivePayload = {
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
                    context = payload.context;
                }

                var loadMethodsPromise = loadMethods();
                promises.push(loadMethodsPromise);

                function loadMethods() {
                    return WhS_Sales_SupplierTargetMatchAPIService.GetTargetMatchMethods().then(function (methods) {
                        if (methods != null) {
                            for (var i = 0; i < methods.length; i++) {
                                selectiveCtrl.methods.push(methods[i]);
                            }
                        }
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return directiveAPI.getData()
            };

            if (selectiveCtrl.onReady && typeof selectiveCtrl.onReady == "function")
                selectiveCtrl.onReady(api);
        }
    }
}]);
