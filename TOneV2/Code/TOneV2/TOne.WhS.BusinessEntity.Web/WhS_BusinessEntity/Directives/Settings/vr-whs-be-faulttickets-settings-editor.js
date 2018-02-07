'use strict';

app.directive('vrWhsBeFaultticketsSettingsEditor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/FaultTicketsSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

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
                    var customerSetting;
                    if (payload != undefined) {
                        data = payload.data;
                    }
                    if (data != undefined)
                    {
                        customerSetting = data.CustomerSetting;
                        if (customerSetting != undefined)
                            $scope.scopeModel.initialSequence = customerSetting.InitialSequence;
                    }

                    var loadSerialNumberEditorPromise = loadSerialNumberEditor(customerSetting);
                    promises.push(loadSerialNumberEditorPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.FaultTicketsSettingsData, TOne.WhS.BusinessEntity.Entities",
                        CustomerSetting: {
                            SerialNumberPattern: serialNumberEditorAPI.getData().serialNumberPattern,
                            InitialSequence: $scope.scopeModel.initialSequence
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function loadSerialNumberEditor(customerSetting) {
                var serialNumberEditorEditorLoadDeferred = UtilsService.createPromiseDeferred();
                serialNumberEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: customerSetting,
                        businessEntityDefinitionId: "e4053d52-8a52-438e-b353-37acf059a938"
                    };
                    VRUIUtilsService.callDirectiveLoad(serialNumberEditorAPI, payload, serialNumberEditorEditorLoadDeferred);
                });
                return serialNumberEditorEditorLoadDeferred.promise;
            }


        }

        return directiveDefinitionObject;
    }]);