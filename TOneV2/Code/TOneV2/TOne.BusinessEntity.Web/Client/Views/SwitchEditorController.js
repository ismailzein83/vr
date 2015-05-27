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
        return SwitchManagmentAPIService.getSwitchDetails($scope.switchId)
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

    function fillScopeFromSwitchObj(switchObject) {
        $scope.lastImport = new Date(switchObject.LastImport);
        $scope.Description = switchObject.Description;
        $scope.Name = switchObject.Name;
        $scope.lastCDRImportTag = switchObject.lastCDRImportTag;
        $scope.Symbol = switchObject.Symbol;
        $scope.enableCDRImport = switchObject.EnableCDRImport;
        $scope.enableRouting = switchObject.EnableRouting;
    }

    function insertSwitch() {
        $scope.issaving = true;
        var switchObject = buildSwitchObjFromScope();
        return SwitchManagmentAPIService.insertSwitch(switchObject)
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
        SwitchManagmentAPIService.updateSwitch(switchObject)
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


