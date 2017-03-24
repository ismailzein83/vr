'use strict';

app.directive('vrInvtoaccbalancerelationDefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrInvToAccBalanceRelationDefinitionSettings = new VRInvToAccBalanceRelationDefinitionSettings($scope, ctrl, $attrs);
                vrInvToAccBalanceRelationDefinitionSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_InvToAccBalanceRelation/Elements/InvToAccBalanceRelationDefinition/Directives/Templates/VRInvToAccBalanceRelationDefinitionSettings.html'
        };

        function VRInvToAccBalanceRelationDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var invToAccBalanceRelationDefinitionEntity;

            var extendedSettingsAPI;
            var extendedSettingsReadyDeferred = UtilsService.createPromiseDeferred();
        
            var fieldsBySourceId;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.extendedSettingsDirectiveReady = function (api) {
                    extendedSettingsAPI = api;
                    extendedSettingsReadyDeferred.resolve();
                };
             
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        invToAccBalanceRelationDefinitionEntity = payload.componentType;
                        if (invToAccBalanceRelationDefinitionEntity != undefined) {
                            $scope.scopeModel.name = invToAccBalanceRelationDefinitionEntity.Name;
                        }
                        
                    }
                    promises.push(loadExtendedSettingsDirective());
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: GetExtendedSettings(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function GetExtendedSettings() {
                return {
                    $type: "Vanrise.InvToAccBalanceRelation.Entities.InvToAccBalanceRelationDefinitionSettings,  Vanrise.InvToAccBalanceRelation.Entities",
                    ExtendedSettings: extendedSettingsAPI.getData(),
                };
            }

            function loadExtendedSettingsDirective() {
                var extendedSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                extendedSettingsReadyDeferred.promise.then(function () {
                    var extendedSettingsPayload;
                    if (invToAccBalanceRelationDefinitionEntity != undefined) {
                        extendedSettingsPayload = { extendedSettingsEntity: invToAccBalanceRelationDefinitionEntity.Settings.ExtendedSettings };
                    }
                    VRUIUtilsService.callDirectiveLoad(extendedSettingsAPI, extendedSettingsPayload, extendedSettingsLoadDeferred);
                });
                return extendedSettingsLoadDeferred.promise;
            }

            function getContext() {
                var context = {
                };
                return context;
            }


        }
    }]);
