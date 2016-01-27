SwitchEditorController.$inject = ["$scope", "SwitchAPIService", "SwitchBrandAPIService", "VR_Integration_DataSourceAPIService", "UtilsService", "VRNavigationService", "VRNotificationService", "VRModalService", 'VRUIUtilsService'];

function SwitchEditorController($scope, SwitchAPIService, SwitchBrandAPIService, VR_Integration_DataSourceAPIService, UtilsService, VRNavigationService, VRNotificationService, VRModalService, VRUIUtilsService) {

    var switchId;
    var isEditMode;
    var switchEntity;
    var allExceptDataSources;
    var dataSourceDirectiveAPI;
    var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();


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

        $scope.brands = [];
        $scope.timeOffset = (!isEditMode) ? "00.00:00:00" : null;
        $scope.dataSources = [];

        $scope.onDataSourceSelectorReady =function(api)
        {
            dataSourceDirectiveAPI = api;
            dataSourceReadyPromiseDeferred.resolve();
        }

        $scope.saveSwitch = function () {
            $scope.isLoading = true;
            if (isEditMode)
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

        $scope.addBrand = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Switch Brand");;


                modalScope.onBrandAdded = function (brandObj) {
                    $scope.brands.push(brandObj);
                    $scope.selectedBrand = brandObj;
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchBrandEditor.html", null, settings);
        }

        $scope.onDataSourceChanged = function () {
        }
    }

    function load() {
        $scope.isLoading = true;
        if (isEditMode) {
            GetSwitch().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            })
        }
        else {
            loadSwitchAssignedDataSources().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            })
        }
       
    }

    function loadAllControls() {
      return  UtilsService.waitMultipleAsyncOperations([loadBrands, loadDataSourceSelector]).then(function () {
                      loadFilterBySection();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            }).finally(function (error) {
                switchEntity = undefined;
                $scope.isLoading = false;
            });
      
    }

    function GetSwitch()
    {
        return SwitchAPIService.GetSwitchById(switchId).then(function (response) {
            switchEntity = response;
        });
    }

    function loadBrands() {
        return SwitchBrandAPIService.GetBrands()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.brands.push(item);
                });
            });
    }

    function loadDataSourceSelector()
    {
        var dataSourceLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        dataSourceReadyPromiseDeferred.promise
            .then(function () {

                var directivePayload = {
                    selectedIds: switchEntity != undefined ? switchEntity.DataSourceId : undefined,
                    filter: allExceptDataSources != undefined ? { AllExcept: allExceptDataSources } : null
                };
                periodReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, directivePayload, dataSourceLoadPromiseDeferred);
            });
        return dataSourceLoadPromiseDeferred.promise;
    }

    function loadSwitchAssignedDataSources() {

        return SwitchAPIService.GetSwitchAssignedDataSources()
            .then(function (response) {
                console.log(response);
                allExceptDataSources=response;
            });
    }

    function loadFilterBySection() {
        if(switchEntity !=undefined)
        {
            $scope.name = switchEntity.Name;
            $scope.selectedBrand = UtilsService.getItemByVal($scope.brands, switchEntity.BrandId, "BrandId");
            $scope.areaCode = switchEntity.AreaCode;
            $scope.timeOffset = switchEntity.TimeOffset;
            $scope.selectedDataSource = UtilsService.getItemByVal($scope.dataSources, switchEntity.DataSourceId, "DataSourceID");
        }
       
    }

    function updateSwitch() {
        var switchObj = buildSwitchObjFromScope();
        return SwitchAPIService.UpdateSwitch(switchObj)
            .then(function (response) {
                $scope.isLoading = false;
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
                $scope.isLoading = false;
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
            DataSourceId: ($scope.selectedDataSource != undefined) ? $scope.selectedDataSource.DataSourceID : null
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
