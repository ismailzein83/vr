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
            IsDefault: $scope.isDefault,
            StrategyCriterias: [
                                { CriteriaId: 1, Threshold: 1 },
                                { CriteriaId: 2, Threshold: 2 },
                                { CriteriaId: 3, Threshold: 1 },
                                { CriteriaId: 4, Threshold: 2 },
                                { CriteriaId: 5, Threshold: 3 },
                                { CriteriaId: 6, Threshold: 1 }
                               ],

            StrategyPeriods:  [
                                { CriteriaId: 1, Value: 1, Period: 1 },
                                { CriteriaId: 2, Value: 1, Period: 6 },
                                { CriteriaId: 3, Value: 1, Period: 1 },
                                { CriteriaId: 4, Value: 1, Period: 6 },
                                { CriteriaId: 5, Value: 1, Period: 6 },
                                { CriteriaId: 6, Value: 1, Period: 1 }
                              ],

            StrategyLevels: [
                                { SuspectionLevelId: 2, StrategyLevelCriterias: [{ CriteriaId: 1, Percentage: 1.0 }, { CriteriaId: 2, Percentage: 1.0 }, { CriteriaId: 3, Percentage: 1.0 }] },
                                { SuspectionLevelId: 3, StrategyLevelCriterias: [{ CriteriaId: 1, Percentage: 1.25 }, { CriteriaId: 2, Percentage: 0.75 }, { CriteriaId: 3, Percentage: 1.0 }] }
                             ]


        };
        console.log(StrategyObject);
        return StrategyObject;
    }

    function fillScopeFromStrategyObj(strategyObject) {
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

    
}
appControllers.controller('StrategyEditorController', StrategyEditorController);
