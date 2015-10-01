SwitchEditorController.$inject = ["$scope", "SwitchAPIService", "SwitchTypeAPIService", "DataSourceAPIService", "DataSourceService", "UtilsService", "VRNavigationService", "VRNotificationService", "VRModalService"];

function SwitchEditorController($scope, SwitchAPIService, SwitchTypeAPIService, DataSourceAPIService, DataSourceService, UtilsService, VRNavigationService, VRNotificationService, VRModalService) {

    var switchID;
    var editMode;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            switchID = parameters.SwitchID;

        editMode = (switchID != undefined);
    }

    function defineScope() {

        $scope.name = undefined;
        $scope.types = [];
        $scope.selectedType = undefined;
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

            if (value.length == 0) return null;

            var offset = value.split(".");

            if (offset.length == 1 && validateTime(offset[0]))
                return null;

            else if (offset.length == 2 && validateInteger(offset[0], 99) && validateTime(offset[1]))
                return null;

            return "Format: DD.HH:MM:SS";
        }

        $scope.addDataSource = function () {

            var eventHandler = function (dataSourceObject) {
                $scope.dataSources.push(dataSourceObject);
                $scope.selectedDataSource = dataSourceObject;
            }

            DataSourceService.addDataSource(eventHandler);
        }

        $scope.addSwitchType = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add a Switch Type";

                modalScope.onSwitchTypeAdded = function (switchTypeObject) {
                    $scope.types.push(switchTypeObject);
                    $scope.selectedType = switchTypeObject;
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Type/SwitchTypeEditor.html", null, settings);
        }
    }

    function load() {
        $scope.isGettingData = true;

        return UtilsService.waitMultipleAsyncOperations([loadSwitchTypes, loadDataSources])
            .then(function () {

                if (editMode) {
                    return SwitchAPIService.GetSwitchByID(switchID)
                        .then(function (response) {
                            fillScopeFromSwitchObject(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        })
                        .finally(function () {
                            $scope.isGettingData = false;
                        });
                }
                else
                    $scope.isGettingData = false;
            })
            .catch(function (error) {
                $scope.isGettingData = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function loadSwitchTypes() {
        return SwitchTypeAPIService.GetSwitchTypes()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.types.push(item);
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

    function fillScopeFromSwitchObject(switchObject) {
        $scope.name = switchObject.Name;
        $scope.selectedType = UtilsService.getItemByVal($scope.types, switchObject.TypeID, "ID");
        $scope.areaCode = switchObject.AreaCode;
        $scope.timeOffset = switchObject.TimeOffset;
        $scope.selectedDataSource = UtilsService.getItemByVal($scope.dataSources, switchObject.DataSourceID, "DataSourceId");
    }

    function updateSwitch() {
        var switchObject = buildSwitchObjectFromScope();

        return SwitchAPIService.UpdateSwitch(switchObject)
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
        var switchObject = buildSwitchObjectFromScope();

        return SwitchAPIService.AddSwitch(switchObject)
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

    function buildSwitchObjectFromScope() {
        return {
            ID: switchID,
            Name: $scope.name,
            TypeID: ($scope.selectedType != undefined) ? $scope.selectedType.ID : null,
            AreaCode: $scope.areaCode,
            TimeOffset: $scope.timeOffset,
            DataSourceID: ($scope.selectedDataSource != undefined) ? $scope.selectedDataSource.DataSourceId : null
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
