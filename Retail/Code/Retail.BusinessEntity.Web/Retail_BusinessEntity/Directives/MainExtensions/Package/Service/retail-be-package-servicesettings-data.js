(function (app) {

    'use strict';

    DataServiceDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'Retail_BE_ConnectionTypeEnum'];

    function DataServiceDirective(UtilsService, VRUIUtilsService, Retail_BE_ConnectionTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataService = new DataService($scope, ctrl, $attrs);
                dataService.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Package/Service/Templates/DataServiceTemplate.html"

        };
        function DataService($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.connectionTypes = UtilsService.getArrayEnum(Retail_BE_ConnectionTypeEnum);
                $scope.scopeModel.selectedConnectionType = Retail_BE_ConnectionTypeEnum.WIFI;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                        if (payload.serviceSettings != undefined)
                        {
                            $scope.scopeModel.selectedConnectionType = UtilsService.getItemByVal($scope.scopeModel.connectionTypes, payload.serviceSettings.ConnectionType, "value");
                            $scope.scopeModel.downloadSpeed = payload.serviceSettings.DownloadSpeed;
                            $scope.scopeModel.uploadSpeed = payload.serviceSettings.UploadSpeed;
                        }

                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.Package.DataService,Retail.BusinessEntity.MainExtensions",
                        ConnectionType: $scope.scopeModel.selectedConnectionType.value,
                        DownloadSpeed: $scope.scopeModel.downloadSpeed,
                        UploadSpeed: $scope.scopeModel.uploadSpeed
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBePackageServiceData', DataServiceDirective);

})(app);