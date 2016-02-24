"use strict";

app.directive("demoGrouptypeZone", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var groupTypeZone = new GroupTypeZone(ctrl, $scope);
            groupTypeZone.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/MainExtensions/GroupTypes/Templates/GroupTypeZone.html"
    };

    function GroupTypeZone(ctrl, $scope) {
        this.initCtrl = initCtrl;

        var zoneDirectiveApi;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initCtrl() {

            ctrl.onZoneDirectiveReady = function (zoneAPI) {
                zoneDirectiveApi = zoneAPI;
                zoneReadyPromiseDeferred.resolve();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var zoneLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    zoneReadyPromiseDeferred.promise
                        .then(function () {
                            var zonePayload = {
                                selectedIds: payload != undefined ? payload.selectedIds : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(zoneDirectiveApi, zonePayload, zoneLoadPromiseDeferred);
                        });
                    return zoneLoadPromiseDeferred.promise;
                };


                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.GroupTypeZone, Demo.Module.MainExtension",
                        SelectedIds: zoneDirectiveApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }
}]);
