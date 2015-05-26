SwitchEditorController.$inject = ['$scope', 'SwitchManagmentAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
function SwitchEditorController($scope, SwitchManagmentAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {


    var editMode = false;
    loadParameters();
    defineScope();
    load();

    function defineScope() {
        $scope.enableCDRImport = false;
        $scope.enableRouting = false;

        $scope.switchManagers = {
            datasource: [],
            selectedvalue: ''
        };
    }

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        editMode = false;
        $scope.switchId = 'undefined';
        if (parameters != 'undefined' && parameters != null) {
            editMode = true;
            $scope.switchId = parameters.switchId;
            $scope.Symbol = parameters.Symbol;
            $scope.Name = parameters.Name;
        }
    }

    function load() {
        if (editMode) {
            SwitchManagmentAPIService.getSwitchDetails($scope.switchId)
           .then(function (response) {
               getScopeDataFromSwitch(response);
           })
        }
    }

    function getSwitchFromScope() {
        var switchObject = {
            SwitchId: ($scope.switchId != null) ? $scope.switchId : 0,
            Name: $scope.Name,
            Symbol: $scope.Symbol,
            Description: $scope.Description,
            LastImport: $scope.lastImport,
            LastCDRImportTag: $scope.lastCDRImportTag,
            EnableCDRImport: $scope.enableCDRImport,
            EnableRouting: $scope.enableRouting
        };
        return switchObject;
    }

    function getScopeDataFromSwitch(switchObject) {
        $scope.lastImport = new Date(switchObject.LastImport);
        $scope.Description = switchObject.Description;
        $scope.Name = switchObject.Name;
        $scope.lastCDRImportTag = switchObject.lastCDRImportTag;
        $scope.Symbol = switchObject.Symbol;
        $scope.enableCDRImport = switchObject.EnableCDRImport;
        $scope.enableRouting = switchObject.EnableRouting;
    }

    function insertSwitch(asyncHandle) {
        $scope.issaving = true;
        var switchObject = getSwitchFromScope();
        SwitchManagmentAPIService.insertSwitch(switchObject)
        .then(function (response) {
            $scope.issaving = false;

            if (angular.isNumber(response) && response > 0) {
                alert(response);
                //$scope.switchId != 'undefined'
                $scope.refreshRowData(response, $scope.index);
            }
            //var newdata = response;
            //newdata.Time = new Date();
            //newdata.Action = ($scope.switchId != 'undefined') ? "Updated" : "Added";
            //$scope.callBackHistory(newdata);
            notify({ message: 'Switch has been saved successfully.', classes: "alert  alert-success" });
            $scope.$hide();
        }).finally(function () {
            if (asyncHandle)
                asyncHandle.operationDone();
        });

    }

    function updateSwitch(asyncHandle) {
        $scope.issaving = true;
        var switchObject = getSwitchFromScope();
        SwitchManagmentAPIService.updateSwitch(switchObject)
        .then(function (response) {
            $scope.issaving = false;
            if (angular.isNumber(response) && response > 0) {//if ($scope.switchId != 'undefined') {
                $scope.refreshRowData(response, $scope.index);
            }
            var newdata = response;
            newdata.Time = new Date();
            newdata.Action = ($scope.switchId != 'undefined') ? "Updated" : "Added";
            $scope.callBackHistory(newdata);
            notify({ message: 'Switch has been saved successfully.', classes: "alert  alert-success" });
            $scope.$hide();
        }).finally(function () {
            if (asyncHandle)
                asyncHandle.operationDone();
        });

    }

    $scope.saveSwitch = function (asyncHandle) {
        if ($scope.switchId != 'undefined') {
            updateSwitch(asyncHandle);
        }
        else {
            insertSwitch(asyncHandle);
        }
    }

    $scope.close = function () {
        $scope.modalContext.closeModal()
    };

}
appControllers.controller('SwitchEditorController', SwitchEditorController);


