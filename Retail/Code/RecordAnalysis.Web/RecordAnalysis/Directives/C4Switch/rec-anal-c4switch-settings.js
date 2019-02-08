(function (app) {

    'use strict';

    RecordAnalysisC4SwitchSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'RecAnal_C4SwitchAPIService'];

    function RecordAnalysisC4SwitchSettingsDirective(UtilsService, VRUIUtilsService, RecAnal_C4SwitchAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecordAnalysisC4SwitchSettings(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/C4Switch/Templates/C4SwitchSettingsTemplate.html"
        };

        function RecordAnalysisC4SwitchSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.c4SwitchTemplates = [];
                $scope.selectedC4Switch;

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.c4SwitchTemplates.length = 0;

                    var promises = [];
                    var settings;

                    if (payload != undefined && payload.selectedValues != undefined) {
                        settings = payload.selectedValues.Settings;
                    }

                    var loadTemplatesPromise = loadTemplates();
                    promises.push(loadTemplatesPromise);

                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);

                    loadTemplatesPromise.then(function () {
                        if (settings != undefined) {
                            $scope.selectedC4Switch = UtilsService.getItemByVal($scope.c4SwitchTemplates, settings.ConfigId, 'ExtensionConfigurationId');
                        }
                        else if ($scope.c4SwitchTemplates.length > 0)
                            $scope.selectedC4Switch = $scope.c4SwitchTemplates[0];
                    });

                    function loadTemplates() {
                        return RecAnal_C4SwitchAPIService.GetC4SwitchTemplates().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.c4SwitchTemplates.push(response[i]);
                                }
                            }
                        });
                    };

                    function loadDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { settings: settings };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = directiveAPI.getData();
                    obj.ConfigId = $scope.selectedC4Switch.ExtensionConfigurationId;
                    return obj;
                };

                api.setData = function (dicData) {
                    var obj = directiveAPI.getData();
                    obj.ConfigId = $scope.selectedC4Switch.ExtensionConfigurationId;

                    dicData["Settings"] = obj;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('recAnalC4switchSettings', RecordAnalysisC4SwitchSettingsDirective);

})(app);