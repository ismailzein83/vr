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
            $scope.StrategyId = parameters.strategyId;
        
        if ($scope.StrategyId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.SaveStrategy = function () {
            if (editMode) {
                
                return UpdateStrategy();
            }
            else {
                return AddStrategy();
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
        
        return StrategyAPIService.GetStrategy($scope.StrategyId)
           .then(function (response) {
               console.log(response);
               fillScopeFromStrategyObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }



    function buildStrategyObjFromScope() {
        var StrategyObject = {
            Id: ($scope.StrategyId != null) ? $scope.StrategyId : 0,
            Name: $scope.name,
            Description: $scope.description,
            IsDefault: $scope.isDefault == false ? "0" : "1"
        };
        return StrategyObject;
    }

    function fillScopeFromStrategyObj(strategyObject) {
        
        //alert(response);

        $scope.name = strategyObject.Name;
        $scope.description = strategyObject.Description;
        $scope.isDefault = strategyObject.IsDefault;
    }

    function AddStrategy() {
        $scope.issaving = true;
        var StrategyObject = buildStrategyObjFromScope();
        return StrategyAPIService.AddStrategy(StrategyObject)
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

    function UpdateStrategy() {
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
