RepeatedNumberController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', '$q', 'VRNotificationService', 'WhS_Analytics_BillingCDROptionMeasureEnum', 'WhS_Analytics_PhoneNumberEnum', 'VRUIUtilsService', 'VRValidationService'];

function RepeatedNumberController($scope, UtilsService, VRNavigationService, $q, VRNotificationService, BillingCDROptionMeasureEnum, PhoneNumberEnum, VRUIUtilsService, VRValidationService) {

    var receivedSwitchIds;
    var mainGridAPI;

    var switchDirectiveAPI;
    var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    loadParameters();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            $scope.fromDate = parameters.fromDate;
            $scope.toDate = parameters.toDate;
            receivedSwitchIds = parameters.switchIds;
            $scope.advancedSelected = true;
        }
    }

    function defineScope() {

        $scope.validateDateTime = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

        $scope.onSwitchDirectiveReady = function (api) {
            switchDirectiveAPI = api;
            switchReadyPromiseDeferred.resolve();
        }

        $scope.useOneTime = true;

        $scope.nRecords = '10';

        var date = new Date();
        $scope.fromDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 00, 00, 00, 00);

        $scope.CDROption = [];
        $scope.PhoneNumber = [];
        $scope.selectedCDROption = BillingCDROptionMeasureEnum.All.value;
        $scope.selectedPhoneNumberOption = PhoneNumberEnum.CDPN.value;

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            if (!$scope.isLoading) {
                mainGridAPI.loadGrid(getQuery());
            }
        }

        $scope.getData = function () {
            return mainGridAPI.loadGrid(getQuery());
        };

    }

    function getQuery() {
        var filter = buildFilter();
        var query = {
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
            NRecords: $scope.nRecords,
            CDRType: $scope.selectedCDROption.value,
            PhoneNumber: $scope.selectedPhoneNumberOption.propertyName
        }
        return query;
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadCDROption, loadSwitches, loadPhoneNumber])
            .then(function () {

                if (mainGridAPI != undefined)
                    mainGridAPI.loadGrid(getQuery());
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = switchDirectiveAPI.getSelectedIds();
        return filter;
    }

    function loadSwitches() {
        var loadSwitchPromiseDeferred = UtilsService.createPromiseDeferred();
        switchReadyPromiseDeferred.promise.then(function () {
            var payload = {
                selectedIds: receivedSwitchIds
            };
            VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, payload, loadSwitchPromiseDeferred);
        });
        return loadSwitchPromiseDeferred.promise;
    }

    function loadCDROption() {
        for (var prop in BillingCDROptionMeasureEnum) {
            $scope.CDROption.push(BillingCDROptionMeasureEnum[prop]);
        }
        $scope.selectedCDROption = BillingCDROptionMeasureEnum.All;
    }

    function loadPhoneNumber() {
        for (var prop in PhoneNumberEnum) {
            $scope.PhoneNumber.push(PhoneNumberEnum[prop]);
        }
        $scope.selectedPhoneNumberOption = PhoneNumberEnum.CDPN;
    }
};

appControllers.controller('WhS_Analytics_RepeatedNumberController', RepeatedNumberController);