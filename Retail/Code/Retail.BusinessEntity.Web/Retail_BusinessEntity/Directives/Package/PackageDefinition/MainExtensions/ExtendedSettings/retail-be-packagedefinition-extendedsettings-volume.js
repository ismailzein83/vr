'use strict';

app.directive('retailBePackagedefinitionExtendedsettingsVolume', ['UtilsService', 'Retail_BE_VolumePackageDefinitionService',
    function (UtilsService, Retail_BE_VolumePackageDefinitionService) {
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

            var volumePackageDefinitionItemsNames = [];

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.isGridValid = function () {
                    if ($scope.scopeModel.datasource.length < 1)
                        return 'Grid should contains at least one item';
                    return null;
                };

                $scope.scopeModel.addVolumePackageDefinitionItem = function () {
                    var onVolumePackageDefinitionItemAdded = function (addedVolumePackageDefinitionItem) {
                        $scope.scopeModel.datasource.push({ Entity: addedVolumePackageDefinitionItem });
                    };

                    getVolumePackageDefinitionItemsNames();

                    Retail_BE_VolumePackageDefinitionService.addVolumePackageDefinitionItem(onVolumePackageDefinitionItemAdded, volumePackageDefinitionItemsNames);
                };

                $scope.scopeModel.removeVolumePackageDefinitionItem = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity.VolumePackageDefinitionItemId, 'Entity.VolumePackageDefinitionItemId');
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.datasource.length = 0;

                    if (payload != undefined && payload.extendedSettings != undefined) {
                        for (var i = 0; i < payload.extendedSettings.Items.length; i++) {
                            $scope.scopeModel.datasource.push({ Entity: payload.extendedSettings.Items[i] });
                        }
                    }
                };

                api.getData = function () {

                    function getItems() {
                        var items = [];
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var currentItem = $scope.scopeModel.datasource[i].Entity;
                            items.push({
                                VolumePackageDefinitionItemId: currentItem.VolumePackageDefinitionItemId,
                                Name: currentItem.Name,
                                CompositeRecordConditionDefinitionGroup: currentItem.CompositeRecordConditionDefinitionGroup,
                                ServiceTypeIds: currentItem.ServiceTypeIds
                            });
                        }
                        return items;
                    }

                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.PackageTypes.VolumePackageDefinitionSettings, Retail.BusinessEntity.MainExtensions',
                        Items: getItems()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editVolumePackageDefinitionItem
                }];
            }

            function editVolumePackageDefinitionItem(volumePackageDefinitionItem) {
                var onVolumePackageDefinitionItemUpdated = function (updatedVolumePackageDefinitionItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, volumePackageDefinitionItem.Entity.VolumePackageDefinitionItemId, 'Entity.VolumePackageDefinitionItemId');
                    $scope.scopeModel.datasource[index] = { Entity: updatedVolumePackageDefinitionItem };
                };

                getVolumePackageDefinitionItemsNames();

                Retail_BE_VolumePackageDefinitionService.editVolumePackageDefinitionItem(onVolumePackageDefinitionItemUpdated, volumePackageDefinitionItem.Entity, volumePackageDefinitionItemsNames);
            }

            function getVolumePackageDefinitionItemsNames() {
                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                    var currentEntity = $scope.scopeModel.datasource[i].Entity;
                    volumePackageDefinitionItemsNames.push(currentEntity.Name);
                }
            }
        }
    }]);