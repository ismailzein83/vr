SwitchEditorController.$inject = ["$scope", "SwitchAPIService", "BrandAPIService", "DataSourceAPIService", "DataSourceService", "UtilsService", "VRNavigationService", "VRNotificationService", "VRModalService"];

function SwitchEditorController($scope, SwitchAPIService, BrandAPIService, DataSourceAPIService, DataSourceService, UtilsService, VRNavigationService, VRNotificationService, VRModalService) {

    var switchId;
    var editMode;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            switchId = parameters.SwitchId;

        editMode = (switchId != undefined);
    }

    function defineScope() {

        $scope.name = undefined;
        $scope.brands = [];
        $scope.selectedBrand = undefined;
        $scope.areaCode = undefined;
        $scope.timeOffset = (!editMode) ? "00.00:00:00" : null;
        $scope.dataSources = [];
        $scope.selectedDataSource = undefined;

        $scope.saveSwitch = function () {
            if (editMode)
                return updateSwitch();
            else
                return insertSwitch();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }

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
        }

        $scope.addDataSource = function () {

            var onDataSourceAdded = function (dataSourceObj) {
                $scope.dataSources.push(dataSourceObj);
                $scope.selectedDataSource = dataSourceObj;
            }

            DataSourceService.addDataSource(onDataSourceAdded);
        }

        $scope.addBrand = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add Brand";

                modalScope.onBrandAdded = function (brandObj) {
                    $scope.brands.push(brandObj);
                    $scope.selectedBrand = brandObj;
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Brand/BrandEditor.html", null, settings);
        }

        $scope.onDataSourceChanged = function () {
        }
    }

    function load() {
        $scope.isGettingData = true;

        UtilsService.waitMultipleAsyncOperations([loadBrands, loadDataSources])
            .then(function () {

                if (editMode) {
                    return SwitchAPIService.GetSwitchById(switchId)
                        .then(function (response) {

                            fillScopeFromSwitchObj(response);

                            loadSwitchAssignedDataSources();
                        })
                        .catch(function (error) {
                            $scope.isGettingData = false;
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                }
                else {
                    loadSwitchAssignedDataSources();
                }
            })
            .catch(function (error) {
                $scope.isGettingData = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function loadBrands() {
        return BrandAPIService.GetBrands()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.brands.push(item);
                });
            });
    }

    function loadDataSources() {
        return DataSourceAPIService.GetDataSources()
            .then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.dataSources.push(item);
                });
            });
    }

    function loadSwitchAssignedDataSources() {

        return SwitchAPIService.GetSwitchAssignedDataSources()
            .then(function (response) {

                var selectedDataSourceIndex = ($scope.selectedDataSource != undefined) ?
                    UtilsService.getItemIndexByVal($scope.dataSources, $scope.selectedDataSource.DataSourceId, "DataSourceId") : -1;

                angular.forEach(response, function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.dataSources, item.DataSourceId, "DataSourceId");

                    if (index > -1 && index != selectedDataSourceIndex) {
                        $scope.dataSources.splice(index, 1);
                    }
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

    function fillScopeFromSwitchObj(switchObj) {
        $scope.name = switchObj.Name;
        $scope.selectedBrand = UtilsService.getItemByVal($scope.brands, switchObj.BrandId, "BrandId");
        $scope.areaCode = switchObj.AreaCode;
        $scope.timeOffset = switchObj.TimeOffset;

        var dataSourceIndex = UtilsService.getItemIndexByVal($scope.dataSources, switchObj.DataSourceId, "DataSourceId");
        $scope.selectedDataSource = (dataSourceIndex > 0) ? $scope.dataSources[dataSourceIndex] : undefined;
    }

    function updateSwitch() {
        var switchObj = buildSwitchObjFromScope();

        return SwitchAPIService.UpdateSwitch(switchObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch", response, "Name")) {
                    if ($scope.onSwitchUpdated != undefined)
                        $scope.onSwitchUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function insertSwitch() {
        var switchObj = buildSwitchObjFromScope();

        return SwitchAPIService.AddSwitch(switchObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch", response, "Name")) {
                    if ($scope.onSwitchAdded != undefined)
                        $scope.onSwitchAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function buildSwitchObjFromScope() {

        return {
            SwitchId: switchId,
            Name: $scope.name,
            BrandId: ($scope.selectedBrand != undefined) ? $scope.selectedBrand.BrandId : null,
            AreaCode: $scope.areaCode,
            TimeOffset: $scope.timeOffset,
            DataSourceId: ($scope.selectedDataSource != undefined) ? $scope.selectedDataSource.DataSourceId : null
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

        /*
        for (var i = 0; i < value.length; i++) {

            if ((i == 2 || i == 5) && value.charAt(i) != ":")
                return "Format: DD.HH:MM:SS";
            else if (value.charAt(i) != ":" && isNaN(parseInt(value.charAt(i))))
                return "Format: DD.HH:MM:SS";
        }
        */
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchEditorController", SwitchEditorController);
