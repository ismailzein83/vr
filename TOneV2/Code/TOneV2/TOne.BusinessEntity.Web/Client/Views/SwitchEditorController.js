SwitchEditorController.$inject = ['$scope', 'SwitchManagmentAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
function SwitchEditorController($scope, SwitchManagmentAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {


    var editMode = false;
    loadParameters();
    defineScope();
    load();

    function defineScope() {
        $scope.enableCDRImport = false;
        $scope.enableRouting = false;
        $scope.isGettingData = false;
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
            $scope.isGettingData = true;
            SwitchManagmentAPIService.getSwitchDetails($scope.switchId)
           .then(function (response) {
               getScopeDataFromSwitch(response);
               $scope.isGettingData = false;
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

            var insertionActionSucc = VRNotificationService.notifyOnItemAdded("Insert Switch", response);

            if (insertionActionSucc) {
                VRNotificationService.showSuccess('Switch will Inserted into parent Grid');
                $scope.modalContext.onSwitchAdded();
            }
            //notify({ message: 'Switch has been saved successfully.', classes: "alert  alert-success" });
            $scope.close();
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
            var updateActionSucc = VRNotificationService.notifyOnItemUpdated("Update Switch", response);

            if (updateActionSucc) {
                VRNotificationService.showSuccess('Switch will Updated into parent Grid');
                $scope.modalContext.onSwitchUpdated();
            }
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


