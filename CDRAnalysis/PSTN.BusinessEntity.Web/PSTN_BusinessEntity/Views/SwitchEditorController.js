SwitchEditorController.$inject = ["$scope", "SwitchAPIService", "DataSourceAPIService", "UtilsService", "VRNavigationService", "VRNotificationService", "VRModalService"];

function SwitchEditorController($scope, SwitchAPIService, DataSourceAPIService, UtilsService, VRNavigationService, VRNotificationService, VRModalService) {

    var switchID = undefined;

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
        $scope.timeOffset = undefined;
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

            if (value.length != 8) return "Format: HH:MM:SS";

            for (var i = 0; i < value.length; i++) {

                if ((i == 2 || i == 5) && value.charAt(i) != ":")
                    return "Format: HH:MM:SS";
                else if (value.charAt(i) != ":" && isNaN(parseInt(value.charAt(i))))
                    return "Format: HH:MM:SS";
            }
        }

        $scope.addDataSource = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add a Data Source";

                modalScope.onDataSourceAdded = function (dataSource) {
                    $scope.dataSources.push(dataSource);
                    $scope.selectedDataSource = dataSource;
                };
            };

            VRModalService.showModal("/Client/Modules/Integration/Views/DataSourceEditor.html", null, settings);
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
        return SwitchAPIService.GetSwitchTypes()
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
            DataSourceID: $scope.selectedDataSource.DataSourceId
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchEditorController", SwitchEditorController);
