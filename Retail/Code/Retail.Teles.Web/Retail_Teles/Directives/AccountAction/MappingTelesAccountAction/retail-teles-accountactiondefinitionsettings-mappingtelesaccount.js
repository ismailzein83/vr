﻿'use strict';

app.directive('retailTelesAccountactiondefinitionsettingsMappingtelesaccount', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new MappingTelesAccountActionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/AccountAction/MappingTelesAccountAction/Templates/MappingTelesAccountActionSettingsTemplate.html'
        };

        function MappingTelesAccountActionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var conectionTypeAPI;
            var conectionTypeReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onConectionTypeReady = function (api) {
                    conectionTypeAPI = api;
                    conectionTypeReadyDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined && payload.accountActionDefinitionSettings != undefined)
                    {
                    }
                    var promises = [];

                    promises.push(loadConectionTypes());
                    function loadConectionTypes() {
                        var conectionTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        conectionTypeReadyDeferred.promise.then(function () {
                            var conectionTypePayload;
                            if (payload != undefined && payload.accountActionDefinitionSettings != undefined) {
                                conectionTypePayload = { selectedIds: payload.accountActionDefinitionSettings.VRConnectionId };
                            }
                            VRUIUtilsService.callDirectiveLoad(conectionTypeAPI, conectionTypePayload, conectionTypeLoadDeferred);
                        });
                        return conectionTypeLoadDeferred.promise
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: 'Retail.Teles.Business.AccountBEActionTypes.MappingTelesAccountActionSettings, Retail.Teles.Business',
                        VRConnectionId: conectionTypeAPI.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);