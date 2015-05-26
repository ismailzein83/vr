var SwitchEditorController = function ($scope, SwitchManagmentAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {


    loadParameters();
    defineScopeObjects();
    load();

    function defineScopeObjects() {
        // $scope.subViewConnector = {};
        $scope.EnableCDRImport = false;
        $scope.EnableRouting = false;

        $scope.switchManagers = {
            datasource: [],
            selectedvalue: ''
        };
    }


    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.switchId = parameters.switchId;
        $scope.Symbol = parameters.Symbol;
        $scope.Name = parameters.Name;
    }


    $scope.insertSwitch = function (asyncHandle) {
        $scope.issaving = true;
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
        alert('Enter insertSwitch' + switchObject.Description);
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


    $scope.UpdateSwitch = function (asyncHandle) {
        $scope.issaving = true;
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
        SwitchManagmentAPIService.updateSwitch(switchObject)
        .then(function (response) {
            $scope.issaving = false;
            if ($scope.switchId != 'undefined') {
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

    $scope.SaveSwitch = function (asyncHandle) {

        if ($scope.switchId != 'undefined') {
            $scope.UpdateSwitch(asyncHandle);
        }
        else {
            $scope.insertSwitch(asyncHandle);
        }
    }

    $scope.hide = function () {
        $scope.$hide();
    };
    function load() {
        alert($scope.switchId);
        if ($scope.switchId != 'undefined') {

            SwitchManagmentAPIService.getSwitchDetails($scope.switchId)
           .then(function (response) {
               $scope.switchObject = response;
               $scope.lastImport = new Date($scope.switchObject.LastImport);
               $scope.Description = $scope.switchObject.Description;
               $scope.Name = $scope.switchObject.Name;
               $scope.lastCDRImportTag = $scope.switchObject.lastCDRImportTag;
               $scope.Symbol = $scope.switchObject.Symbol;
               $scope.enableCDRImport = $scope.switchObject.EnableCDRImport;
               $scope.enableRouting = $scope.switchObject.EnableRouting;
           })
        }
        else {
            //$scope.optionsRouteType.selectedvalue = null;
            //$scope.optionsEditorType.selectedvalue = $scope.optionsEditorType.datasource[0];
            //$scope.optionsRuleType.selectedvalue = $scope.optionsRuleType.datasource[0];
        }
    }

}
SwitchEditorController.$inject = ['$scope', 'SwitchManagmentAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

appControllers.controller('SwitchEditorController', SwitchEditorController)


