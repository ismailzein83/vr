StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function StrategyEditorController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.StrategyId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.StrategyId = parameters.StrategyId;

        if ($scope.StrategyId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.SaveStrategy = function () {
            if (editMode) {
                return updateStrategy();
            }
            else {
                return insertStrategy();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        if (editMode) {
            $scope.isGettingData = true;
            getStrategy().finally(function () {
                $scope.isGettingData = false;
            })
        }
    }

    function getStrategy() {
        return StrategysAPIService.GetStrategy($scope.StrategyId)
           .then(function (response) {
               fillScopeFromStrategyObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }



    function buildStrategyObjFromScope() {
        var StrategyObject = {
            StrategyId: ($scope.StrategyId != null) ? $scope.StrategyId : 0,
            name: $scope.name,
            email: $scope.email,
            description: $scope.description,
            Status: $scope.isActive == false ? "0" : "1"
        };
        return StrategyObject;
    }

    function fillScopeFromStrategyObj(StrategyObject) {
        $scope.name = StrategyObject.Name;
        $scope.email = StrategyObject.Email;
        $scope.description = StrategyObject.Description;
        $scope.isActive = StrategyObject.Status;
    }

    function insertStrategy() {
        $scope.issaving = true;
        var StrategyObject = buildStrategyObjFromScope();
        return StrategysAPIService.AddStrategy(StrategyObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Strategy", response)) {
                if ($scope.onStrategyAdded != undefined)
                    $scope.onStrategyAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });

    }

    function updateStrategy() {
        var StrategyObject = buildStrategyObjFromScope();
        StrategyAPIService.UpdateStrategy(StrategyObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Strategy", response)) {
                if ($scope.onStrategyUpdated != undefined)
                    $scope.onStrategyUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }

    //$scope.CheckStrategyName = function (name) {
    //    var bool = false;
    //    if (name == undefined)
    //        return null;

    //    StrategyAPIService.CheckStrategyName(name == undefined ? " " : name).then(function (response) {
    //        bool = response;
    //        //if (response == false)
    //        //return "Invalid";
    //        //else
    //        //  return null;

    //    }).finally(function () {

    //    });

    //    if (!bool)
    //        return "Invalid";
    //}
}
appControllers.controller('StrategyEditorController', StrategyEditorController);
