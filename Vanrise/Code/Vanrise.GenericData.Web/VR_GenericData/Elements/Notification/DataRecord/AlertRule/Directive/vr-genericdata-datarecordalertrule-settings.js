'use strict';

app.directive('vrGenericdataDatarecordalertruleSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataRecordAlertRule($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/AlertRule/Directive/Templates/DataRecordAlertRuleSettingsTemplate.html'
        };

        function DataRecordAlertRule($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordAlertRuleConfigSelectorAPI;
            var dataRecordAlertRuleConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var dataRecordAlertRuleConfigSelectionChanged;

            var directiveAPI;
            var directiveReadyDeferred;

            var settings;
            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordAlertRuleConfigSelectorReady = function (api) {
                    dataRecordAlertRuleConfigSelectorAPI = api;
                    dataRecordAlertRuleConfigSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = {
                        context: context
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isDirectiveLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    directiveAPI = undefined;

                    var promises = [];

                    if (payload != undefined) {
                        settings = payload.settings;
                        context = payload.context;
                    }

                    var dataRecordAlertRuleConfigSelectorLoadPromise = getDataRecordAlertRuleConfigSelectorLoadPromise();
                    promises.push(dataRecordAlertRuleConfigSelectorLoadPromise);

                    if (settings != undefined) {
                        var directiveLoadPromise = getDirectiveLoadPromise();
                        promises.push(directiveLoadPromise);
                    }

                    function getDataRecordAlertRuleConfigSelectorLoadPromise() {
                        if (settings != undefined)
                            dataRecordAlertRuleConfigSelectionChanged = UtilsService.createPromiseDeferred();

                        var dataRecordAlertRuleConfigSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordAlertRuleConfigSelectorReadyDeferred.promise.then(function () {
                            var dataRecordAlertRuleConfigSelectorPayload = { selectIfSingleItem: true };
                            if (settings != undefined) {
                                dataRecordAlertRuleConfigSelectorPayload.selectedIds = settings.ConfigId;
                            }
                            else {
                                $scope.scopeModel.selectedDataRecordAlertRuleConfig = undefined;
                            }

                            VRUIUtilsService.callDirectiveLoad(dataRecordAlertRuleConfigSelectorAPI, dataRecordAlertRuleConfigSelectorPayload, dataRecordAlertRuleConfigSelectorLoadPromiseDeferred);
                        });

                        return dataRecordAlertRuleConfigSelectorLoadPromiseDeferred.promise;
                    }
                    function getDirectiveLoadPromise() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                settings: settings,
                                context: context
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadPromiseDeferred);
                        });

                        return directiveLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return directiveAPI.getData();
                };

                api.hasData = function () {
                    var hasData = false;

                    if (directiveAPI != undefined) {
                        hasData = directiveAPI.hasData();
                    }
                    return hasData;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);