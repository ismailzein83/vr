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

            var specialRoutingSettingsAPI;
            var specialRoutingSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.reservedBTables = [];
                $scope.scopeModel.reservedBTableRanges = [];

                $scope.scopeModel.onManualRoutesReady = function (api) {
                    manualRouteSettingsAPI = api;
                    manualRouteSettingsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSpecialRoutingReady = function (api) {
                    specialRoutingSettingsAPI = api;
                    specialRoutingSettingsReadyPromiseDeferred.resolve();
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

                $scope.scopeModel.isCodeLengthValid = function () {
                    if ($scope.scopeModel.minCodeLength != undefined && $scope.scopeModel.maxCodeLength != undefined && parseInt($scope.scopeModel.minCodeLength) > parseInt($scope.scopeModel.maxCodeLength))
                        return 'Maximum Code Length should be greater than Minimum Code Length.';
                    return null;
                };


                $scope.scopeModel.addReservedBTable = function () {
                    $scope.scopeModel.reservedBTables.push($scope.scopeModel.reservedBTable);
                    $scope.scopeModel.reservedBTable = undefined;
                };

                $scope.scopeModel.isReservedBTableValid = function () {
                    if (!isBetweenZeroAndOneHundred($scope.scopeModel.reservedBTable))
                        return false;

                    return ($scope.scopeModel.validateReservedBTables() == undefined);
                };

                $scope.scopeModel.validateReservedBTables = function () {

                    if (isNewBTableExists())
                        return "BTable already exists";

                    if (isNewBTableBelongsToExistingBTableRange())
                        return "BTable belongs to an existing BTable range";

                    function isNewBTableExists() {
                        if ($scope.scopeModel.reservedBTables != undefined) {
                            for (var i = 0; i < $scope.scopeModel.reservedBTables.length; i++) {

                                if ($scope.scopeModel.reservedBTable == $scope.scopeModel.reservedBTables[i])
                                    return true;
                            }
                        }
                        return false;
                    }

                    function isNewBTableBelongsToExistingBTableRange() {
                        if ($scope.scopeModel.reservedBTableRanges != undefined) {
                            for (var i = 0; i < $scope.scopeModel.reservedBTableRanges.length; i++) {
                                var item = $scope.scopeModel.reservedBTableRanges[i];

                                if (parseInt($scope.scopeModel.reservedBTable) >= parseInt(item.From) && parseInt($scope.scopeModel.reservedBTable) <= parseInt(item.To))
                                    return true;
                            }
                        }
                        return false;
                    }
                };


                $scope.scopeModel.addReservedBTableRange = function () {
                    var reservedBTableRange = {
                        From: $scope.scopeModel.reservedBTableRangeFrom,
                        To: $scope.scopeModel.reservedBTableRangeTo
                    };

                    $scope.scopeModel.reservedBTableRanges.push(reservedBTableRange);
                    $scope.scopeModel.reservedBTableRangeFrom = undefined;
                    $scope.scopeModel.reservedBTableRangeTo = undefined;
                };

                $scope.scopeModel.isReservedBTableRangeValid = function () {
                    if (!isBetweenZeroAndOneHundred($scope.scopeModel.reservedBTableRangeFrom) || !isBetweenZeroAndOneHundred($scope.scopeModel.reservedBTableRangeTo))
                        return false;

                    return ($scope.scopeModel.validateReservedBTableRanges() == undefined);
                };

                $scope.scopeModel.validateReservedBTableRanges = function () {

                    if (parseInt($scope.scopeModel.reservedBTableRangeFrom) >= parseInt($scope.scopeModel.reservedBTableRangeTo))
                        return "To value should be greater than From value.";

                    if (isNewRangeOverlappeWithExistingRange())
                        return "BTable range overlapped with existing one.";

                    if (isNewBTableRangeContainsExistingBTable())
                        return "BTable range contains an existing BTable.";

                    function isNewRangeOverlappeWithExistingRange() {
                        if ($scope.scopeModel.reservedBTableRanges != undefined) {
                            for (var i = 0; i < $scope.scopeModel.reservedBTableRanges.length; i++) {
                                var item = $scope.scopeModel.reservedBTableRanges[i];

                                if (parseInt($scope.scopeModel.reservedBTableRangeFrom) >= parseInt(item.From) && parseInt($scope.scopeModel.reservedBTableRangeFrom) <= parseInt(item.To)) //From In
                                    return true;

                                if (parseInt($scope.scopeModel.reservedBTableRangeTo) >= parseInt(item.From) && parseInt($scope.scopeModel.reservedBTableRangeTo) <= parseInt(item.To)) //To In
                                    return true;

                                if (parseInt($scope.scopeModel.reservedBTableRangeFrom) <= parseInt(item.From) && parseInt($scope.scopeModel.reservedBTableRangeTo) >= parseInt(item.To)) //Range Inside
                                    return true;
                            }
                        }
                        return false;
                    }

                    function isNewBTableRangeContainsExistingBTable() {
                        if ($scope.scopeModel.reservedBTables != undefined) {
                            for (var i = 0; i < $scope.scopeModel.reservedBTables.length; i++) {
                                var item = $scope.scopeModel.reservedBTables[i];

                                if ($scope.scopeModel.reservedBTableRangeFrom == item)
                                    return true;

                                if ($scope.scopeModel.reservedBTableRangeTo == item)
                                    return true;

                                if (parseInt($scope.scopeModel.reservedBTableRangeFrom) <= parseInt(item) && parseInt($scope.scopeModel.reservedBTableRangeTo) >= parseInt(item))
                                    return true;
                            }
                        }
                        return false;
                    }
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
                            if (ericssonSWSync.ReservedBTables != undefined)
                                $scope.scopeModel.reservedBTables = ericssonSWSync.ReservedBTables;
                            if (ericssonSWSync.ReservedBTableRanges != undefined)
                                $scope.scopeModel.reservedBTableRanges = ericssonSWSync.ReservedBTableRanges;
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

                    var manualRoutesLoadPromise = getManualRouteSettingsLoadPromise();
                    promises.push(manualRoutesLoadPromise);

                    var specialRoutesLoadPromise = getSpecialRoutingSettingsLoadPromise();
                    promises.push(specialRoutesLoadPromise);

                    function getManualRouteSettingsLoadPromise() {
                        var manualRouteSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        manualRouteSettingsReadyPromiseDeferred.promise.then(function () {
                            var manualRouteSettingsPayload;
                            if (ericssonSWSync != undefined && ericssonSWSync.ManualRouteSettings != undefined) {
                                manualRouteSettingsPayload = {
                                    manualRoutes: ericssonSWSync.ManualRouteSettings.EricssonManualRoutes
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(manualRouteSettingsAPI, manualRouteSettingsPayload, manualRouteSettingsLoadPromiseDeferred);
                        });

                        return manualRouteSettingsLoadPromiseDeferred.promise;
                    }

                    function getSpecialRoutingSettingsLoadPromise() {
                        var specialRoutingSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        specialRoutingSettingsReadyPromiseDeferred.promise.then(function () {
                            var specialRoutingSettingsPayload;
                            if (ericssonSWSync != undefined && ericssonSWSync.ManualRouteSettings != undefined) {
                                specialRoutingSettingsPayload = {
                                    specialRoutes: ericssonSWSync.ManualRouteSettings.EricssonSpecialRoutes
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(specialRoutingSettingsAPI, specialRoutingSettingsPayload, specialRoutingSettingsLoadPromiseDeferred);
                        });

                        return specialRoutingSettingsLoadPromiseDeferred.promise;
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

                    function getManualRoutesSettings() {
                        var manualRouteSettings = {
                            EricssonManualRoutes: manualRouteSettingsAPI != undefined ? manualRouteSettingsAPI.getData() : null,
                            EricssonSpecialRoutes: specialRoutingSettingsAPI != undefined ? specialRoutingSettingsAPI.getData() : null
                        };
                        return manualRouteSettings;
                    }

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
                        ManualRouteSettings: getManualRoutesSettings(),
                        ReservedBTables: $scope.scopeModel.reservedBTables,
                        ReservedBTableRanges: $scope.scopeModel.reservedBTableRanges,
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

            function isBetweenZeroAndOneHundred(value) {
                return (value >= 0 && value <= 100);
            }
        }
    }

    app.directive('whsRoutesyncEricssonSwsync', EricssonSWSync);

})(app);