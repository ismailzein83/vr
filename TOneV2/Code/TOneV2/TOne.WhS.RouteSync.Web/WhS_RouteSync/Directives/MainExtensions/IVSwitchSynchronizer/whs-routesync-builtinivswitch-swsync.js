(function (app) {

    'use strict';

    ivswitchSwSync.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService'];

    function ivswitchSwSync(utilsService, vruiUtilsService, whSBeCarrierAccountApiService, vrNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ivSWSyncronizer = new IVSWSyncronizer($scope, ctrl, $attrs);
                ivSWSyncronizer.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/IVSwitchSynchronizer/Templates/BuiltInIVSwitchSWSyncTemplate.html"

        };
        function IVSWSyncronizer($scope, ctrl, $attrs) {

            var gridAPI;

            function defineApi() {
                var api = {};
                api.load = function (payload) {

                    var ivSwSynSettings;

                    if (payload != undefined) {
                        ivSwSynSettings = payload.switchSynchronizerSettings;
                    }
                    else {
                        $scope.scopeModel.Uid = utilsService.guid();
                    }
                    if (ivSwSynSettings) {
                        $scope.scopeModel.MasterConnectionString = ivSwSynSettings.MasterConnectionString;
                        $scope.scopeModel.RouteConnectionString = ivSwSynSettings.RouteConnectionString;
                        $scope.scopeModel.TariffConnectionString = ivSwSynSettings.TariffConnectionString;
                        $scope.scopeModel.OwnerName = ivSwSynSettings.OwnerName;
                        $scope.scopeModel.NumberOfOptions = ivSwSynSettings.NumberOfOptions;
                        $scope.scopeModel.BlockedAccountMapping = ivSwSynSettings.BlockedAccountMapping;
                        $scope.scopeModel.Uid = ivSwSynSettings.Uid;
                    }
                };

                function getData() {
                    var data = {
                        $type: "TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync,TOne.WhS.RouteSync.IVSwitch",
                        MasterConnectionString: $scope.scopeModel.MasterConnectionString,
                        RouteConnectionString: $scope.scopeModel.RouteConnectionString,
                        TariffConnectionString: $scope.scopeModel.TariffConnectionString,
                        OwnerName: $scope.scopeModel.OwnerName,
                        NumberOfOptions: $scope.scopeModel.NumberOfOptions,
                        BlockedAccountMapping: $scope.scopeModel.BlockedAccountMapping,
                        Uid: $scope.scopeModel.Uid
                    };
                    return data;
                }

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isLoading = false;
                defineApi();
            }

            this.initializeController = initializeController;
        }
    }

    app.directive('whsRoutesyncBuiltinivswitchSwsync', ivswitchSwSync);

})(app);