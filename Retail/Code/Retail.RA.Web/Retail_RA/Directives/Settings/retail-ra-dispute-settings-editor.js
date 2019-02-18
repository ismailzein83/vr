'use strict';

app.directive('retailRaDisputeSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/Settings/Templates/DisputeSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;
            var disputeSettings;

            var serialNumberEditorAPI;
            var serialNumberEditorReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};
            $scope.scopeModel.initialSequence = 0;

            $scope.scopeModel.onSerialNumberEditorReady = function (api) {
                serialNumberEditorAPI = api;
                serialNumberEditorReadyDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var data;

                    if (payload != undefined) {
                        data = payload.data;
                    }
                    if (data != undefined) {
                        $scope.scopeModel.initialSequence = data.InitialSequence;
                    }

                    var loadSerialNumberEditorPromise = loadSerialNumberEditor(data);

                    promises.push(loadSerialNumberEditorPromise);
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Entities.DisputeSettingsData, Retail.RA.Entities",
                        SerialNumberPattern: serialNumberEditorAPI.getData().serialNumberPattern,
                        InitialSequence: $scope.scopeModel.initialSequence
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadSerialNumberEditor(disputeSettings) {
                var serialNumberEditorEditorLoadDeferred = UtilsService.createPromiseDeferred();
                serialNumberEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: disputeSettings,
                        businessEntityDefinitionId: "45ea42b6-05ec-462d-8a38-c66a08aa28bd"
                    };
                    VRUIUtilsService.callDirectiveLoad(serialNumberEditorAPI, payload, serialNumberEditorEditorLoadDeferred);
                });
                return serialNumberEditorEditorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);