'use strict';

app.directive('retailBeAccountactiondefinitionsettingsExportrates', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ChangeStatusActionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/ExportRatesAction/Templates/ExportRatesActionSettingsTemplate.html'
        };

        function ChangeStatusActionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var viewLogPermissionAPI;
            var viewLogPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onViewLogRequiredPermissionReady = function (api) {
                    viewLogPermissionAPI = api;
                    viewLogPermissionReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var accountActionDefinitionSettings;

                    if (payload != undefined) {
                        accountActionDefinitionSettings = payload.accountActionDefinitionSettings;
                    }

                    var promises = [];
                    promises.push(loadViewLogRequiredPermission());

                    function loadViewLogRequiredPermission() {
                        var viewLogSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewLogPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: accountActionDefinitionSettings && accountActionDefinitionSettings.Security && accountActionDefinitionSettings.Security.ViewLogPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewLogPermissionAPI, dataPayload, viewLogSettingPermissionLoadDeferred);
                        });
                        return viewLogSettingPermissionLoadDeferred.promise;
                    }


                    return UtilsService.waitMultiplePromises(promises)
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountBEActionTypes.ExportRatesActionSettings, Retail.BusinessEntity.MainExtensions',
                        Security: {
                            ViewLogPermission: viewLogPermissionAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);