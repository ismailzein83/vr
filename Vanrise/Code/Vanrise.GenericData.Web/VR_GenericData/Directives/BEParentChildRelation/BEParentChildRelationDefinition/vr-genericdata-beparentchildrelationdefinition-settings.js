'use strict';

app.directive('vrGenericdataBeparentchildrelationdefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var beParentChildRelationDefinitionSettings = new BEParentChildRelationDefinitionSettings($scope, ctrl, $attrs);
                beParentChildRelationDefinitionSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BEParentChildRelation/BEParentChildRelationDefinition/Templates/BEParentChildRelationDefinitionSettingsTemplate.html'
        };

        function BEParentChildRelationDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var parentBEDefinitionSelectorAPI;
            var parentBEDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var childBEDefinitionSelectorAPI;
            var childBEDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onParentBEDefinitionSelectorReady = function (api) {
                    parentBEDefinitionSelectorAPI = api;
                    parentBEDefinitionSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onChildBEDefinitionSelectorReady = function (api) {
                    childBEDefinitionSelectorAPI = api;
                    childBEDefinitionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var beParentChildRelationDefinitionSettings;

                    if (payload != undefined) {
                        var beParentChildRelationDefinitionEntity = payload.componentType;

                        if (beParentChildRelationDefinitionEntity != undefined) {
                            $scope.scopeModel.name = beParentChildRelationDefinitionEntity.Name;
                            beParentChildRelationDefinitionSettings = beParentChildRelationDefinitionEntity.Settings;
                        }
                    }

                    $scope.scopeModel.childFilterFQTN = beParentChildRelationDefinitionSettings.ChildFilterFQTN;

                    //Loading ParentBEDefinition selector
                    var parentBEDefinitionSelectorLoadPromise = getParentBEDefinitionSelectorLoadPromise();
                    promises.push(parentBEDefinitionSelectorLoadPromise);

                    //Loading ChildBEDefinition selector
                    var childBEDefinitionSelectorLoadPromise = getChildBEDefinitionSelectorLoadPromise();
                    promises.push(childBEDefinitionSelectorLoadPromise);


                    function getParentBEDefinitionSelectorLoadPromise() {
                        var parentBEDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        parentBEDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                selectedIds: beParentChildRelationDefinitionSettings != undefined ? beParentChildRelationDefinitionSettings.ParentBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(parentBEDefinitionSelectorAPI, payload, parentBEDefinitionSelectorLoadDeferred);
                        });

                        return parentBEDefinitionSelectorLoadDeferred.promise;
                    }
                    function getChildBEDefinitionSelectorLoadPromise() {
                        var childBEDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        childBEDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                selectedIds: beParentChildRelationDefinitionSettings != undefined ? beParentChildRelationDefinitionSettings.ChildBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(childBEDefinitionSelectorAPI, payload, childBEDefinitionSelectorLoadDeferred);
                        });

                        return childBEDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.GenericData.Entities.BEParentChildRelationDefinitionSettings, Vanrise.GenericData.Entities",
                            ParentBEDefinitionId: parentBEDefinitionSelectorAPI.getSelectedIds(),
                            ChildBEDefinitionId: childBEDefinitionSelectorAPI.getSelectedIds(),
                            ChildFilterFQTN: $scope.scopeModel.childFilterFQTN
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
