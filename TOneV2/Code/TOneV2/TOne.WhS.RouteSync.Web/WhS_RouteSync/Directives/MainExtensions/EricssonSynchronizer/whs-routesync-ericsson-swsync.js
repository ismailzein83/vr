(function (app) {

    'use strict';

    EricssonSWSync.$inject = ["UtilsService", 'VRUIUtilsService'];

    function EricssonSWSync(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EricssonSWSyncronizerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonSWSyncTemplate.html"
        };

        function EricssonSWSyncronizerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var switchCommunicationAPI;
            var switchCommunicationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountMappingGridAPI;
            var carrierAccountMappingGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var branchRouteSettingsAPI;
            var branchRouteSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var manualRouteSettingsAPI;
            var manualRouteSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
            
                $scope.scopeModel.onManualRoutesReady = function (api) {
                    manualRouteSettingsAPI = api;
                    manualRouteSettingsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onEricssonSwitchCommunicationReady = function (api) {
                    switchCommunicationAPI = api;
                    switchCommunicationReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onBranchRouteSettingsReady = function (api) {
                    branchRouteSettingsAPI = api;
                    branchRouteSettingsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCarrierAccountMappingGridReady = function (api) {
                    carrierAccountMappingGridAPI = api;
                    carrierAccountMappingGridReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var ericssonSWSync;
                    var carrierMappings;
                    var sshCommunicationList;
                    var switchLoggerList;
                    var branchRouteSettings;

                    if (payload != undefined) {
                        ericssonSWSync = payload.switchSynchronizerSettings;

                        if (ericssonSWSync != undefined) {
                            $scope.scopeModel.isEditMode = true;
                            $scope.scopeModel.firstRCNumber = ericssonSWSync.FirstRCNumber;
                            $scope.scopeModel.numberOfMappings = ericssonSWSync.NumberOfMappings;
                            $scope.scopeModel.minCodeLength = ericssonSWSync.MinCodeLength;
                            $scope.scopeModel.maxCodeLength = ericssonSWSync.MaxCodeLength;
                            $scope.scopeModel.interconnectGeneralPrefix = ericssonSWSync.InterconnectGeneralPrefix;
                            $scope.scopeModel.esr = ericssonSWSync.ESR;
                            $scope.scopeModel.cc = ericssonSWSync.CC;
                            $scope.scopeModel.percentagePrefix = ericssonSWSync.PercentagePrefix;
                            branchRouteSettings = ericssonSWSync.BranchRouteSettings;
                            sshCommunicationList = ericssonSWSync.SwitchCommunicationList;
                            switchLoggerList = ericssonSWSync.SwitchLoggerList;
                            carrierMappings = ericssonSWSync.CarrierMappings;
                        }
                    }
                    //Loading Switch Communication
                    var switchCommunicationLoadPromise = getSwitchCommunicationLoadPromise();
                    promises.push(switchCommunicationLoadPromise);
                    
                    //Loading CarrierAccountMapping Grid
                    var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise();
                    promises.push(carrierAccountMappingGridLoadPromise);

                    //LoadingBranchRouteSettings
                    var branchRouteSettingsLoadPromise = getBranchRouteSettingsLoadPromise();
                    promises.push(branchRouteSettingsLoadPromise);

                    var manualRouteSettingsLoadPromise = getManualRouteSettingsLoadPromise();
                    promises.push(manualRouteSettingsLoadPromise);

                    function getManualRouteSettingsLoadPromise() {
                        var manualRouteSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        manualRouteSettingsReadyPromiseDeferred.promise.then(function () {
                            var manualRouteSettingsPayload;
                            if (ericssonSWSync != undefined) {
                                manualRouteSettingsPayload = ericssonSWSync.ManualRouteSettings;
                            }
                            VRUIUtilsService.callDirectiveLoad(manualRouteSettingsAPI, manualRouteSettingsPayload, manualRouteSettingsLoadPromiseDeferred);
                        });

                        return manualRouteSettingsLoadPromiseDeferred.promise;
                    }

                    function getSwitchCommunicationLoadPromise() {
                        var switchCommunicationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        switchCommunicationReadyPromiseDeferred.promise.then(function () {
                            var switchCommunicationPayload;
                            if (ericssonSWSync != undefined) {
                                switchCommunicationPayload = { sshCommunicationList: sshCommunicationList, switchLoggerList: switchLoggerList };
                            }
                            VRUIUtilsService.callDirectiveLoad(switchCommunicationAPI, switchCommunicationPayload, switchCommunicationLoadPromiseDeferred);
                        });

                        return switchCommunicationLoadPromiseDeferred.promise;
                    }

                    function getCarrierAccountMappingGridLoadPromise() {
                        var carrierAccountMappingGridLoadDeferred = UtilsService.createPromiseDeferred();

                        carrierAccountMappingGridReadyPromiseDeferred.promise.then(function () {
                            var carrierAccountMappingGridPayload = { carrierMappings: carrierMappings };
                            VRUIUtilsService.callDirectiveLoad(carrierAccountMappingGridAPI, carrierAccountMappingGridPayload, carrierAccountMappingGridLoadDeferred);
                        });

                        return carrierAccountMappingGridLoadDeferred.promise;
                    }

                    function getBranchRouteSettingsLoadPromise() {
                        var branchRouteSettingsLoadPromise = UtilsService.createPromiseDeferred();
                        branchRouteSettingsReadyPromiseDeferred.promise.then(function () {
                            var branchRouteSettingsPayload = { branchRouteSettings: branchRouteSettings };
                            VRUIUtilsService.callDirectiveLoad(branchRouteSettingsAPI, branchRouteSettingsPayload, branchRouteSettingsLoadPromise);
                        });

                        return branchRouteSettingsLoadPromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    
                    var switchCommunicationData = switchCommunicationAPI.getData();
                    var data = {
                        $type: "TOne.WhS.RouteSync.Ericsson.EricssonSWSync, TOne.WhS.RouteSync.Ericsson",
                        NumberOfMappings: $scope.scopeModel.numberOfMappings,
                        MinCodeLength: $scope.scopeModel.minCodeLength,
                        MaxCodeLength: $scope.scopeModel.maxCodeLength,
                        InterconnectGeneralPrefix: $scope.scopeModel.interconnectGeneralPrefix,
                       CarrierMappings: carrierAccountMappingGridAPI.getData(),
                        SwitchCommunicationList: switchCommunicationData != undefined ? switchCommunicationData.sshCommunicationList : undefined,
                        SwitchLoggerList: switchCommunicationData != undefined ? switchCommunicationData.switchLoggerList : undefined,
                        FirstRCNumber: $scope.scopeModel.firstRCNumber,
                        BranchRouteSettings: (branchRouteSettingsAPI != undefined) ? branchRouteSettingsAPI.getData() : null,
                        ManualRouteSettings: (manualRouteSettingsAPI != undefined) ? manualRouteSettingsAPI.getData() : null,
                        ESR: $scope.scopeModel.esr,
                        CC: $scope.scopeModel.cc,
                        PercentagePrefix: $scope.scopeModel.percentagePrefix
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonSwsync', EricssonSWSync);

})(app);