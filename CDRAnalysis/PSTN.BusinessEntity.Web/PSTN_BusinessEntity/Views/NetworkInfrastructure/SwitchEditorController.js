(function (appControllers) {

    SwitchEditorController.$inject = ["$scope", "CDRAnalysis_PSTN_SwitchAPIService", "UtilsService", "VRNavigationService", "VRNotificationService", 'VRUIUtilsService'];

    function SwitchEditorController($scope, CDRAnalysis_PSTN_SwitchAPIService, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService) {

        var switchId;
        var isEditMode;
        var switchEntity;
        var dataSourceDirectiveAPI;
        var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var brandDirectiveAPI;
        var brandReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                switchId = parameters.SwitchId;

            isEditMode = (switchId != undefined);
        }

        function defineScope() {
            $scope.hasSaveSwitchPermission = function () {
                if (isEditMode)
                    return CDRAnalysis_PSTN_SwitchAPIService.HasUpdateSwitchPermission();
                else
                    return CDRAnalysis_PSTN_SwitchAPIService.HasAddSwitchPermission();
            };

            $scope.timeOffset = (!isEditMode) ? "00.00:00:00" : null;

            $scope.onDataSourceSelectorReady = function (api) {
                dataSourceDirectiveAPI = api;
                dataSourceReadyPromiseDeferred.resolve();
            };

            $scope.onSwicthBrandSelectorReady = function (api) {
                brandDirectiveAPI = api;
                brandReadyPromiseDeferred.resolve();
            };

            $scope.saveSwitch = function () {
                $scope.isLoading = true;
                if (isEditMode)
                    return updateSwitch();
                else
                    return insertSwitch();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.validateTimeOffset = function (value) {

                if (value == undefined) return null;

                var offset = value.split(".");

                if (offset.length == 1) {
                    var time = offset[0].split("-");

                    if (time.length == 1 && validateTime(time[0]))
                        return null;

                    else if (time.length == 2 && time[0].length == 0 && validateTime(time[1]))
                        return null;
                }
                else if (offset.length == 2) {
                    var days = offset[0].split("-");

                    if (days.length == 1 && validateInteger(days[0], 99) && validateTime(offset[1]))
                        return null;

                    else if (days.length == 2 && days[0].length == 0 && validateInteger(days[1], 99) && validateTime(offset[1]))
                        return null;
                }

                return "Format: DD.HH:MM:SS";
            };

        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                GetSwitch().then(function () {
                    loadAllControls().finally(function () {
                        switchEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                })
            }
            else {
                loadAllControls();
            }

        }

        function GetSwitch() {
            return CDRAnalysis_PSTN_SwitchAPIService.GetSwitchById(switchId).then(function (response) {
                switchEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSwitchBrandSelector, loadDataSourceSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function (error) {
                switchEntity = undefined;
                $scope.isLoading = false;
            });

        }

        function setTitle() {
            if (isEditMode && switchEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(switchEntity.Name, "Switch");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Switch");
        }

        function loadStaticData() {

            if (switchEntity == undefined)
                return;

            $scope.name = switchEntity.Name;
            $scope.areaCode = switchEntity.AreaCode;
            $scope.timeOffset = switchEntity.TimeOffset;
        }

        function loadSwitchBrandSelector() {
            var brandLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            brandReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: switchEntity != undefined ? switchEntity.BrandId : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(brandDirectiveAPI, directivePayload, brandLoadPromiseDeferred);
                });
            return brandLoadPromiseDeferred.promise;
        }

        function loadDataSourceSelector() {
            var dataSourceLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            dataSourceReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: switchEntity != undefined ? switchEntity.DataSourceId : undefined,
                        filter: null
                    };
                    VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, directivePayload, dataSourceLoadPromiseDeferred);
                });
            return dataSourceLoadPromiseDeferred.promise;
        }

        function updateSwitch() {
            var switchObj = buildSwitchObjFromScope();
            $scope.isLoading = true;
            return CDRAnalysis_PSTN_SwitchAPIService.UpdateSwitch(switchObj)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Switch", response, "Name")) {
                        if ($scope.onSwitchUpdated != undefined)
                            $scope.onSwitchUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                })
                .finally(function (error) {
                    $scope.isLoading = false;
                });
        }

        function insertSwitch() {
            var switchObj = buildSwitchObjFromScope();
            $scope.isLoading = true;
            return CDRAnalysis_PSTN_SwitchAPIService.AddSwitch(switchObj)
                .then(function (response) {
                    $scope.isLoading = false;
                    if (VRNotificationService.notifyOnItemAdded("Switch", response, "Name")) {
                        if ($scope.onSwitchAdded != undefined)
                            $scope.onSwitchAdded(response.InsertedObject);

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                })
                .finally(function (error) {
                    $scope.isLoading = false;
                });;
        }

        function buildSwitchObjFromScope() {
            return {
                SwitchId: switchId,
                Name: $scope.name,
                BrandId: brandDirectiveAPI.getSelectedIds(),
                AreaCode: $scope.areaCode,
                TimeOffset: $scope.timeOffset,
                DataSourceId: dataSourceDirectiveAPI.getSelectedIds()
            };
        }

        function validateInteger(integer, maxValue) {
            var parsedInt = parseInt(integer);

            if (isNaN(parsedInt) || parsedInt < 0 || parsedInt > maxValue) return false;

            return true;
        }

        function validateTime(time) { // the valid time format is HH:MM:SS

            if (time.length != 8) return false;

            var timeArray = time.split(":");

            if (timeArray.length != 3 || timeArray[0].length != 2 || timeArray[1].length != 2 || timeArray[2].length != 2)
                return false;

            if (validateInteger(timeArray[0], 23) && validateInteger(timeArray[1], 59) && validateInteger(timeArray[2], 59))
                return true;

            return false;
        }

    }

    appControllers.controller("PSTN_BusinessEntity_SwitchEditorController", SwitchEditorController);

})(appControllers);