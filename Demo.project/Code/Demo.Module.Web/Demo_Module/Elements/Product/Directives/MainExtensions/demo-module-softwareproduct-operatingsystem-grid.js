(function (app) {

    'use strict';

    operatingSystemGrid.$inject = ['UtilsService', 'Demo_Module_ProductAPIService', 'VRUIUtilsService'];

    function operatingSystemGrid(UtilsService, Demo_Module_ProductAPIService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var ctor = new operatingSystemGridCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Product/Directives/MainExtensions/Templates/OperatingSystemGridTemplate.html'
        };

        function operatingSystemGridCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var operatingSystemGridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.operatingSystemsGridItems = [];

                $scope.scopeModel.addOperatingSystemClicked = function () {
                    extendOperatingSystemRow();
                };

                $scope.scopeModel.onRemoveIconClicked = function (operatingSystem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.operatingSystemsGridItems, operatingSystem.rowIndex, 'rowIndex');
                    if (index > -1) {
                        $scope.scopeModel.operatingSystemsGridItems.splice(index, 1);
                    }
                };

                $scope.scopeModel.onOperatingSystemGridReady = function (api) {
                    operatingSystemGridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.validateOperatingSystem = function () {
                    var errorMessage = "At least 1 operating system must be selected";
                    if ($scope.scopeModel.operatingSystemsGridItems.length == 0)
                        return errorMessage;

                    return null;
                };
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var softwareOperatingSystems;

                    if (payload != undefined) {
                        softwareOperatingSystems = payload.softwareOperatingSystems;
                    }

                    var loadOperatingSystemSelectorPromise = loadOperatingSystemSelector();
                    var rootPromiseNode = {
                        promises: [loadOperatingSystemSelectorPromise],
                        getChildNode: function () {
                            var childPromises = [];
                            if (softwareOperatingSystems != null && softwareOperatingSystems != undefined) {
                                for (var i = 0; i < softwareOperatingSystems.length; i++) {
                                    var extendOperatingSystemRowPromise = extendOperatingSystemRow(softwareOperatingSystems[i]);
                                    childPromises.push(extendOperatingSystemRowPromise);
                                }
                            }
                            return { promises: childPromises };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var softwareOperatingSystems = [];
                    for (var i = 0; i < $scope.scopeModel.operatingSystemsGridItems.length; i++) {
                        var currentGridItem = $scope.scopeModel.operatingSystemsGridItems[i];

                        var softwareOperatingSystem;
                        if (currentGridItem.operatingSystemDirectiveAPI != undefined) {
                            softwareOperatingSystem = currentGridItem.operatingSystemDirectiveAPI.getData();
                        }
                        else {
                            softwareOperatingSystem = currentGridItem.softwareOperatingSystem;
                        }

                        if (softwareOperatingSystem != undefined) {
                            softwareOperatingSystem.ConfigId = $scope.scopeModel.operatingSystemsGridItems[i].selectedOperatingSystem.ExtensionConfigurationId;
                            softwareOperatingSystems.push(softwareOperatingSystem);
                        }
                    }
                    return softwareOperatingSystems;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadOperatingSystemSelector() {
                $scope.scopeModel.operatingSystemsConfigs = [];
                return Demo_Module_ProductAPIService.GetOperatingSystemConfigs().then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.operatingSystemsConfigs.push(response[i]);
                        }
                    }
                });
            }

            function extendOperatingSystemRow(softwareOperatingSystem) {

                var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                var directiveLoadDeferred;

                var gridItem = {
                    isFirstLoad: true,
                    isDirectiveLoading: true,
                    selectedOperatingSystem: undefined,
                    operatingSystemSelectorAPI: undefined,
                    operatingSystemDirectiveAPI: undefined,
                    softwareOperatingSystem: softwareOperatingSystem
                };

                gridItem.onOperatingSystemSelectorReady = function (api) {
                    gridItem.operatingSystemSelectorAPI = api;
                    if (softwareOperatingSystem != undefined) {
                        var selectedOperatingSystemValue = UtilsService.getItemByVal($scope.scopeModel.operatingSystemsConfigs, softwareOperatingSystem.ConfigId, 'ExtensionConfigurationId');
                        if (selectedOperatingSystemValue != null) {
                            gridItem.selectedOperatingSystem = selectedOperatingSystemValue;
                        }
                    }
                    selectorLoadDeferred.resolve();
                };

                gridItem.onDirectiveReady = function (api) {
                    gridItem.operatingSystemDirectiveAPI = api;

                    var payload = {
                        softwareOperatingSystem: softwareOperatingSystem
                    };
                    softwareOperatingSystem = undefined;

                    var setLoader = function (value) {
                        gridItem.isDirectiveLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.operatingSystemDirectiveAPI, payload, setLoader, directiveLoadDeferred);
                };

                gridItem.onSelectionChanged = function () {
                    if (gridItem.isFirstLoad) {
                        gridItem.isFirstLoad = false;
                        return;
                    }

                    softwareOperatingSystem = undefined;
                    if (gridItem.selectedOperatingSystem == undefined) {
                        operatingSystemGridAPI.collapseRow(gridItem);
                    }
                    else {
                        operatingSystemGridAPI.expandRow(gridItem);
                    }
                };

                $scope.scopeModel.operatingSystemsGridItems.push(gridItem);

                return UtilsService.waitMultiplePromises([selectorLoadDeferred.promise]);
            }
        }
    }

    app.directive('demoModuleSoftwareproductOperatingsystemGrid', operatingSystemGrid);

})(app);