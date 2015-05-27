SwitchEditorController.$inject = ['$scope', 'SwitchManagmentAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
function SwitchEditorController($scope, SwitchManagmentAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        
        $scope.switchId = undefined;
        if (parameters != undefined && parameters != null)            
            $scope.switchId = parameters.switchId;

        if ($scope.switchId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.saveSwitch = function () {
            if (editMode) {
                return updateSwitch();
            }
            else {
                return insertSwitch();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        if (editMode) {
            $scope.isGettingData = true;
            getSwitch().finally(function () {
                $scope.isGettingData = false;
            })
        }
    }

    function getSwitch() {
        return SwitchManagmentAPIService.GetSwitchDetails($scope.switchId)
           .then(function (response) {
               fillScopeFromSwitchObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }

    function buildSwitchObjFromScope() {
        var switchObject = {
            SwitchId: ($scope.switchId != null) ? $scope.switchId : 0,
            Name: $scope.name,
            Symbol: $scope.symbol,
            Description: $scope.description,
            LastImport: $scope.lastImport,
            LastCDRImportTag: $scope.lastCDRImportTag,
            EnableCDRImport: $scope.enableCDRImport,
            EnableRouting: $scope.enableRouting
        };

        return switchObject;
    }

    function fillScopeFromSwitchObj(switchObject) {
        $scope.lastImport = new Date(switchObject.LastImport);
        $scope.description = switchObject.Description;
        $scope.name = switchObject.Name;
        $scope.lastCDRImportTag = switchObject.lastCDRImportTag;
        $scope.symbol = switchObject.Symbol;
        $scope.enableCDRImport = switchObject.EnableCDRImport;
        $scope.enableRouting = switchObject.EnableRouting;
    }

    function insertSwitch() {
        $scope.issaving = true;
        var switchObject = buildSwitchObjFromScope();
        return SwitchManagmentAPIService.InsertSwitch(switchObject)
        .then(function (response) {  
            if (VRNotificationService.notifyOnItemAdded("Switch", response))
            {
                if ($scope.onSwitchAdded != undefined)
                    $scope.onSwitchAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });

    }

    function updateSwitch() {
        var switchObject = buildSwitchObjFromScope();
        SwitchManagmentAPIService.UpdateSwitch(switchObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Switch", response)) {
                if ($scope.onSwitchUpdated != undefined)
                    $scope.onSwitchUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }

}
appControllers.controller('SwitchEditorController', SwitchEditorController);


