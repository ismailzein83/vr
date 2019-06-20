"use strict";
app.directive("vrAccountbalanceGenericFinancialaccountbalanceSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericFinancialAccountBalanceSettings = new GenericFinancialAccountBalanceSettings($scope, ctrl, $attrs);
                genericFinancialAccountBalanceSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/MainExtensions/GenericData/Templates/GenericFinancialAccountBalanceSettingsTemplate.html"
        };


        function GenericFinancialAccountBalanceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var configuration;

            var genericFinancialAccountDirectiveAPI;
            var genericFinancialAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenricFinancialAccountReady = function (api) {
                    genericFinancialAccountDirectiveAPI = api;
                    genericFinancialAccountReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    var promises = [];
                    if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                        configuration = payload.extendedSettingsEntity.Configuration;
                    }

                    promises.push(loadGenericFinancialAccountDirective());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.AccountBalance.MainExtensions.GenericFinancialAccountBalanceSetting,Vanrise.AccountBalance.MainExtensions",
                        Configuration: genericFinancialAccountDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                function loadGenericFinancialAccountDirective() {
                    var genericFinancialAccountDirectiverLoadDeferred = UtilsService.createPromiseDeferred();

                    genericFinancialAccountReadyPromiseDeferred.promise.then(function () {
                        var genericFinancialAccountPayload;

                        if (configuration != undefined) {
                            genericFinancialAccountPayload = configuration;
                        }
                        VRUIUtilsService.callDirectiveLoad(genericFinancialAccountDirectiveAPI, genericFinancialAccountPayload, genericFinancialAccountDirectiverLoadDeferred);
                    });

                    return genericFinancialAccountDirectiverLoadDeferred.promise;
                }
            }
        }

        return directiveDefinitionObject;
    }
]);