(function (app) {

    'use strict';

    VRRestAPIDataProviderDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function VRRestAPIDataProviderDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRRestAPIDataProviderCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "restAPICtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AnalyticDataProvider/Templates/VRRestAPIAnalyticDataProviderTemplate.html"
        };

        function VRRestAPIDataProviderCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionId;

            var connectionSelectorAPI;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var connectionSelectorSelectionChangedDeferred;

            var analyticTableDirectiveAPI;
            var analyticTableReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var analyticQueryInterceptorSelectiveAPI;
            var analyticQueryInterceptorSelectivePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAnalyticTableSelectorReady = function (api) {
                    analyticTableDirectiveAPI = api;
                    analyticTableReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAnalyticQueryInterceptorSelectiveReady = function (api) {
                    analyticQueryInterceptorSelectiveAPI = api;
                    analyticQueryInterceptorSelectivePromiseDeferred.resolve();
                };

                $scope.scopeModel.onConnectionSelectionChanged = function (dataItem) {

                    if (dataItem != undefined) {
                        if (connectionSelectorSelectionChangedDeferred != undefined) {
                            connectionSelectorSelectionChangedDeferred.resolve();
                        }
                        else {
                            connectionId = dataItem.VRConnectionId;

                            var directivePayload = {
                                connectionId: connectionId
                            };

                            var setLoader = function (value) {
                                $scope.scopeModel.isRemoteAnalyticTableSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, analyticTableDirectiveAPI, directivePayload, setLoader);
                        }
                    }
                };

                defineAPI();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var isEditMode;
                    var remoteAnalyticTableId;
                    var vrRestAPIAnalyticQueryInterceptor;

                    if (payload != undefined && payload.analyticDataProviderSettings != undefined) {
                        connectionId = payload.analyticDataProviderSettings.VRConnectionId;
                        remoteAnalyticTableId = payload.analyticDataProviderSettings.RemoteAnalyticTableId;
                        vrRestAPIAnalyticQueryInterceptor = payload.analyticDataProviderSettings.VRRestAPIAnalyticQueryInterceptor;

                        isEditMode = true;
                        connectionSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                    }

                    //Loading Connection selector
                    var connectionSelectorLoadPromise = getConnectionSelectorLoadPromise();
                    promises.push(connectionSelectorLoadPromise);

                    var analyticQueryInterceptorSelectiveLoadPromise = getAnalyticQueryInterceptorSelectiveLoadPromise();
                    promises.push(analyticQueryInterceptorSelectiveLoadPromise);

                    if (isEditMode) {
                        var analyticTableSelectorLoadPromise = getAnalyticTableSelectorLoadPromise();
                        promises.push(analyticTableSelectorLoadPromise);
                    }

                    function getConnectionSelectorLoadPromise() {
                        var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        connectionSelectorPromiseDeferred.promise.then(function () {

                            var connectionSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                                    }]
                                }
                            };
                            if (connectionId != undefined) {
                                connectionSelectorPayload.selectedIds = connectionId;
                            };
                            VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, connectionSelectorPayload, connectionSelectorLoadDeferred);
                        });

                        return connectionSelectorLoadDeferred.promise;
                    }
                    function getAnalyticTableSelectorLoadPromise() {
                        var analyticTableReadyLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([analyticTableReadyPromiseDeferred.promise, connectionSelectorSelectionChangedDeferred.promise]).then(function () {
                            connectionSelectorSelectionChangedDeferred = undefined;

                            var directivePayload = {
                                connectionId: connectionId,
                                selectedIds: remoteAnalyticTableId
                            };
                            VRUIUtilsService.callDirectiveLoad(analyticTableDirectiveAPI, directivePayload, analyticTableReadyLoadPromiseDeferred);
                        });

                        return analyticTableReadyLoadPromiseDeferred.promise;
                    }
                    function getAnalyticQueryInterceptorSelectiveLoadPromise() {
                        var analyticQueryInterceptorSelectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        analyticQueryInterceptorSelectivePromiseDeferred.promise.then(function () {

                            var directivePayload = {
                                vrRestAPIAnalyticQueryInterceptor: vrRestAPIAnalyticQueryInterceptor
                            };
                            VRUIUtilsService.callDirectiveLoad(analyticQueryInterceptorSelectiveAPI, directivePayload, analyticQueryInterceptorSelectiveLoadPromiseDeferred);
                        });

                        return analyticQueryInterceptorSelectiveLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Business.VRRestAPIAnalyticDataProvider, Vanrise.Analytic.Business",
                        VRConnectionId: connectionSelectorAPI.getSelectedIds(),
                        RemoteAnalyticTableId: analyticTableDirectiveAPI.getSelectedIds(),
                        VRRestAPIAnalyticQueryInterceptor: analyticQueryInterceptorSelectiveAPI.getData()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            };
        }
    }

    app.directive('vrAnalyticRestapiDataprovider', VRRestAPIDataProviderDirective);

})(app);