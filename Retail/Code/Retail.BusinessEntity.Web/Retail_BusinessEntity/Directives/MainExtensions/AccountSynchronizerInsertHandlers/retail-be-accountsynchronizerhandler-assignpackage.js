'use strict';

app.directive('retailBeAccountsynchronizerhandlerAssignpackage', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var assignProductAndPackagesAccountInsertHandler = new AssignProductAndPackagesAccountInsertHandler($scope, ctrl, $attrs);
                assignProductAndPackagesAccountInsertHandler.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountSynchronizerInsertHandlers/Templates/AssignPackageAccountInsertHandlerTemplate.html'
        };

        function AssignProductAndPackagesAccountInsertHandler($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var customFieldTypeSelectorAPI;
            var customFieldTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var packageSelectorAPI;
            var packageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.assignementDaysOffsetFromToday = 0;

                $scope.scopeModel.onPackageSelectorReady = function (api) {
                    packageSelectorAPI = api;
                    packageSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onCustomFieldTypeSelectorReady = function (api) {
                    customFieldTypeSelectorAPI = api;
                    customFieldTypeSelectorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var handlerSettings;

                    if (payload != undefined) {

                        handlerSettings = payload.Settings;

                        if (handlerSettings != undefined) {
                            $scope.scopeModel.assignementDate = handlerSettings.AssignementDate;
                            $scope.scopeModel.assignementDaysOffsetFromToday = handlerSettings.AssignementDaysOffsetFromToday;
                            $scope.scopeModel.customFieldName = handlerSettings.CustomFieldName;
                        }
                    }

                    promises.push(loadPackageSelector());
                    promises.push(loadCustomFieldTypeSelector());

                    function loadPackageSelector() {
                        var packageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                         packageSelectorReadyDeferred.promise.then(function () {
                            var packageSelectorPayload;
                          
                            if (handlerSettings != undefined) {
                                packageSelectorPayload= {
                                    selectedIds: handlerSettings.DefaultPackageId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(packageSelectorAPI, packageSelectorPayload, packageSelectorLoadDeferred);
                        });

                        return packageSelectorLoadDeferred.promise;
                    };
                    function loadCustomFieldTypeSelector() {
                        var customFieldTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        customFieldTypeSelectorReadyPromiseDeferred.promise.then(function () {
                            var customFieldTypeSelectorPayload;

                            if (handlerSettings != undefined) {
                                customFieldTypeSelectorPayload = {
                                    selectedIds: handlerSettings.CustomFieldType
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(customFieldTypeSelectorAPI, customFieldTypeSelectorPayload, customFieldTypeSelectorLoadDeferred);
                        });

                        return customFieldTypeSelectorLoadDeferred.promise;
                    };

                    var rootPromiseNode = { promises: promises };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountSynchronizerInsertHandlers.AssignPackageAccountInsertHandler, Retail.BusinessEntity.MainExtensions',
                        AssignementDate: $scope.scopeModel.assignementDate,
                        AssignementDaysOffsetFromToday: $scope.scopeModel.assignementDaysOffsetFromToday,
                        DefaultPackageId: packageSelectorAPI.getSelectedIds(),
                        CustomFieldName: $scope.scopeModel.customFieldName,
                        CustomFieldType: customFieldTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
    }]);
