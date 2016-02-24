"use strict";

app.directive("demoGrouptypeCountry", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var groupTypeCountry = new GroupTypeCountry(ctrl, $scope);
            groupTypeCountry.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/MainExtensions/GroupTypes/Templates/GroupTypeCountry.html"
    };

    function GroupTypeCountry(ctrl, $scope) {
        this.initCtrl = initCtrl;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initCtrl() {

            ctrl.onCountryDirectiveReady = function (countryAPI) {
                countryDirectiveApi = countryAPI;
                countryReadyPromiseDeferred.resolve();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    countryReadyPromiseDeferred.promise
                        .then(function () {
                            var countryPayload = {
                                selectedIds: payload != undefined ? payload.selectedIds : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, countryPayload, countryLoadPromiseDeferred);
                        });
                    return countryLoadPromiseDeferred.promise;
                };


                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.GroupTypeCountry, Demo.Module.MainExtension",
                        SelectedIds: countryDirectiveApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }
}]);
