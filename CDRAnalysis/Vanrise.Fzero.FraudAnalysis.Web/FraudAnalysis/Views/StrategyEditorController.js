StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function StrategyEditorController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    
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
        $scope.strategyFilters = [];
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

        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadFilters, loadPeriods])
        .then(function () {
            if (editMode) {
                getStrategy().finally(function () {
                    $scope.isGettingData = false;
                });
            }
            else {
                loadFiltersForAddMode();
                $scope.isGettingData = false;
            }
            
        })
        .catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error);
        });
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
            StrategyFilters : $scope.strategyFilters,
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
        console.log("strategyObject.StrategyFilters: " + StrategyObject.StrategyFilters[0]);
        return StrategyObject;
    }

    function buildStrategyFilterObjFromScope() {
        var StrategyFilterObject = {
            PeriodId: ($scope.selectedPeriod != null) ? $scope.selectedPeriod : 0,
            Threshold: $scope.threshold,
            MinimumValue: $scope.minimumValue
        };
        console.log(StrategyFilterObject);
        return StrategyFilterObject;
    }

    function SaveStrategyFilters() {
        $scope.issaving = true;
        var StrategyObject = buildStrategyFilterObjFromScope();
       
    }

    function loadFiltersForAddMode() {
        angular.forEach($scope.filterDefinitions, function (filterDef) {
            var filterItem = {
                filterId: filterDef.filterId,
                filterDescription: filterDef.description
            };
            $scope.strategyFilters.push(filterItem);
        });
    }

    function fillScopeFromStrategyObj(strategyObject) {
        $scope.name = strategyObject.Name;
        $scope.description = strategyObject.Description;
        $scope.isDefault = strategyObject.IsDefault;        
        angular.forEach($scope.filterDefinitions, function (filterDef) {
            var filterItem = {
                filterId: filterDef.filterId,
                filterDescription: filterDef.description
            };
            var existingItem = UtilsService.getItemByVal(strategyObject.StrategyFilters, filterDef.filterId, "CriteriaId");
            if (existingItem != undefined && existingItem != null) {
                filterItem.isSelected = true;
                filterItem.threshold = existingItem.Threshold;
                filterItem.minimumValue = existingItem.MinimumValue;
                if (existingItem.Period != undefined)
                    filterItem.period = UtilsService.getItemByVal($scope.periods, existingItem.Period, "Id");
            }
            $scope.strategyFilters.push(filterItem);
        });
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


    $scope.filterDefinitions =  [];


    function loadFilters() {
        return StrategyAPIService.GetFilters().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.filterDefinitions.push({ filterId: itm.CriteriaId, description: itm.Description });
            });
        });
    }



    $scope.periods = [];
    $scope.selectedPeriod = "";

    function loadPeriods() {
        console.log("Periods");
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });
        });
    }





    StrategyEditorController.isFilterTabShown = true;
    StrategyEditorController.isLevelsTabShow = false;

    StrategyEditorController.viewVisibilityChanged = function () {
        isFilterTabShown = !isFilterTabShown;
        isLevelsTabShow = !isLevelsTabShow;
    };






    
}
appControllers.controller('StrategyEditorController', StrategyEditorController);
