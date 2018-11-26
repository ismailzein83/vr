'use strict';

app.directive('retailBePackagedefinitionExtendedsettingsVolume', ['UtilsService', 'Retail_BE_VolumePackageDefinitionSettingsService',
    function (UtilsService, Retail_BE_VolumePackageDefinitionSettingsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VolumePackageDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/MainExtensions/ExtendedSettings/Templates/VolumePackageDefinitionSettingsTemplate.html'
        };

        function VolumePackageDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.addVolumePackageDefinitionItem = function () {
                    var onVolumePackageDefinitionItemAdded = function (addedVolumePackageDefinitionItem) {
                        $scope.scopeModel.datasource.push(addedVolumePackageDefinitionItem);
                    };

                    Retail_BE_VolumePackageDefinitionSettingsService.addVolumePackageDefinitionItem(onVolumePackageDefinitionItemAdded);
                };

                $scope.scopeModel.removeVolumePackageDefinitionItem = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.VolumePackageDefinitionItemId, 'VolumePackageDefinitionItemId');
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.datasource.length = 0;

                    if (payload != undefined && payload.extendedSettings != undefined)
                        $scope.scopeModel.datasource = payload.extendedSettings.Items;
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.PackageTypes.VolumePackageDefinitionSettings, Retail.BusinessEntity.MainExtensions',
                        Items: $scope.scopeModel.datasource
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editVolumePackageDefinitionItem
                }];

                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editVolumePackageDefinitionItem(volumePackageDefinitionItem) {
                var onVolumePackageDefinitionItemUpdated = function (updatedVolumePackageDefinitionItem) {
                    var index = $scope.scopeModel.datasource.indexOf(volumePackageDefinitionItem);
                    $scope.scopeModel.datasource[index] = updatedVolumePackageDefinitionItem;
                };
                Retail_BE_VolumePackageDefinitionSettingsService.editVolumePackageDefinitionItem(volumePackageDefinitionItem, onVolumePackageDefinitionItemUpdated);
            }
        }
    }]);