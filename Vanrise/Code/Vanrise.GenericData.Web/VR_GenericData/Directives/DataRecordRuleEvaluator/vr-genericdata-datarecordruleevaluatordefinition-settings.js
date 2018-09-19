(function (app) {

    'use strict';

    DataRecordRuleEvaluatorDefinitionSettings.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DataRecordRuleEvaluatorDefinitionSettings(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordRuleEvaluatorDefinitionSettingsDirective = new DataRecordRuleEvaluatorDefinitionSettingsDirective(ctrl, $scope, $attrs);
                dataRecordRuleEvaluatorDefinitionSettingsDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordRuleEvaluator/Templates/DataRecordRuleEvaluatorDefinitionSettingsTemplate.html'
        };

        function DataRecordRuleEvaluatorDefinitionSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var dataRecordStorageSelectorAPI;
            var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var alertRuleTypeSelectorAPI;
            var alertRuleTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var startInstancePermissionAPI;
            var startInstancePermissionReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageSelectorAPI = api;
                    dataRecordStorageSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAlertRuleTypeSelectorReady = function (api) {
                    alertRuleTypeSelectorAPI = api;
                    alertRuleTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };
                $scope.scopeModel.onStartInstanceRequiredPermissionReady = function (api) {
                    startInstancePermissionAPI = api;
                    startInstancePermissionReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var settings;
                    if (payload != undefined && payload.componentType != undefined) {
                        $scope.scopeModel.name = payload.componentType.Name;
                        settings = payload.componentType.Settings;
                        if (settings != undefined) {
                            $scope.scopeModel.areDatesHardCoded = settings.AreDatesHardCoded;
                        }
                    }

                    promises.push(loadDataRecordStorageSelector());
                    promises.push(loadAlertRuleTypeSelector());

                    function loadDataRecordStorageSelector() {
                        var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordStorageSelectorReadyDeferred.promise.then(function () {
                            var dataRecordStorageSelectorPayload;

                            if (settings != undefined) {
                                dataRecordStorageSelectorPayload = {
                                    selectedIds: settings.DataRecordStorageIds
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, dataRecordStorageSelectorPayload, dataRecordStorageSelectorLoadDeferred);
                        });

                        return dataRecordStorageSelectorLoadDeferred.promise;
                    }

                    function loadAlertRuleTypeSelector() {
                        var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        alertRuleTypeSelectorReadyDeferred.promise.then(function () {
                            var alertRuleTypeSelectorPayload;

                            if (settings != undefined) {
                                alertRuleTypeSelectorPayload = {
                                    selectedIds: settings.AlertRuleTypeId
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, alertRuleTypeSelectorPayload, alertRuleTypeSelectorLoadDeferred);
                        });

                        return alertRuleTypeSelectorLoadDeferred.promise;
                    }

                    var loadViewRequiredPermissionPromise = loadViewRequiredPermission();
                    promises.push(loadViewRequiredPermissionPromise);
                    function loadViewRequiredPermission() {
                        var viewPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                        viewPermissionReadyDeferred.promise.then(function () {
                            var viewpayload;

                            if (settings != undefined && settings.Security != undefined && settings.Security.ViewPermission != undefined) {
                                viewpayload = {
                                    data: settings.Security.ViewPermission
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, viewpayload, viewPermissionLoadDeferred);
                        });

                        return viewPermissionLoadDeferred.promise;
                    }

                    var loadAddRequiredPermissionPromise = loadAddRequiredPermission();
                    promises.push(loadAddRequiredPermissionPromise);
                    function loadAddRequiredPermission() {
                        var startInstancePermissionLoadDeferred = UtilsService.createPromiseDeferred();

                        startInstancePermissionReadyDeferred.promise.then(function () {
                            var startInstancePayload;

                            if (settings != undefined && settings.Security != undefined && settings.Security.StartInstancePermission != undefined) {
                                startInstancePayload = {
                                    data: settings.Security.StartInstancePermission
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(startInstancePermissionAPI, startInstancePayload, startInstancePermissionLoadDeferred);
                        });

                        return startInstancePermissionLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.GenericData.Entities.DataRecordRuleEvaluatorDefinitionSettings, Vanrise.GenericData.Entities",
                            DataRecordStorageIds: dataRecordStorageSelectorAPI.getSelectedIds(),
                            AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds(),
                            AreDatesHardCoded: $scope.scopeModel.areDatesHardCoded,
                            Security: {
                                ViewPermission: viewPermissionAPI.getData(),
                                StartInstancePermission: startInstancePermissionAPI.getData(),
                            }
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }
        }
        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataDatarecordruleevaluatordefinitionSettings', DataRecordRuleEvaluatorDefinitionSettings);

})(app);
