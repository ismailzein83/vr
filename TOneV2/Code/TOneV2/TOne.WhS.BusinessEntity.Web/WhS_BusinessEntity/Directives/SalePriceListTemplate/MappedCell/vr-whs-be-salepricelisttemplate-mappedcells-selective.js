'use strict';

app.directive('vrWhsBeSalepricelisttemplateMappedcellsSelective', ['WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var mappedCellsSelective = new MappedCellsSelective($scope, ctrl, $attrs);
            mappedCellsSelective.initializeController();
        },
        controllerAs: "mappedCellCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/MappedCell/Templates/SalePriceListMappedCellsSelectiveTemplate.html'
    };

    function MappedCellsSelective($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.extensionConfigs = [];

            $scope.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingDirective = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
            };
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var promises = [];

                var configId;
                var mappedCell;

                if (payload != undefined) {
                    mappedCell = payload.mappedCell;
                }

                if (mappedCell != undefined) {
                    configId = mappedCell.ConfigId;
                }

                var getExtensionConfigsPromise = getMappedCellsExtensionConfigs(configId);
                promises.push(getExtensionConfigsPromise);

                var loadDirectivePromise = loadDirective(mappedCell);
                promises.push(loadDirectivePromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function getData() {
                return directiveAPI.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getMappedCellsExtensionConfigs(selectedId) {
            return WhS_BE_SalePriceListTemplateAPIService.GetMappedCellsExtensionConfigs().then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++)
                        $scope.extensionConfigs.push(response[i]);
                }
                if (selectedId != undefined) {
                    $scope.selectedExtensionConfig = UtilsService.getItemByVal($scope.extensionConfigs, selectedId, 'ExtensionConfigurationId');
                }
                else if ($scope.extensionConfigs.length > 0) {
                    $scope.selectedExtensionConfig = $scope.extensionConfigs[0];
                }
            });
        }
        function loadDirective(mappedCell) {

            var directiveLoadDeferred = UtilsService.createPromiseDeferred();

            directiveReadyDeferred.promise.then(function () {
                directiveReadyDeferred = undefined;
                var directivePayload = {
                    mappedCell: mappedCell,
                };
                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
            });

            return directiveLoadDeferred.promise;
        }
    }
}]);