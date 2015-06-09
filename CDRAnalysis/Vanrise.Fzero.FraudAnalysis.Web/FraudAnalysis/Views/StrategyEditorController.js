StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function StrategyEditorController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    
    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.strategyId = undefined;

        if (parameters != undefined && parameters != null)
            $scope.strategyId = parameters.strategyId;
        
        if ($scope.strategyId != undefined)
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
        
        return StrategyAPIService.GetStrategy($scope.strategyId)
           .then(function (response) {
               fillScopeFromStrategyObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }


    function buildStrategyObjFromScope() {
       
        var strategyObject = {
            id: ($scope.strategyId != null) ? $scope.strategyId : 0,
            name: $scope.name,
            description: $scope.description,
            isDefault: $scope.isDefault,

            strategyFilters: []

            //,StrategyLevels: [
            //                    { SuspectionLevelId: 2, StrategyLevelCriterias: [{ CriteriaId: 1, Percentage: 1.0 }, { CriteriaId: 2, Percentage: 1.0 }, { CriteriaId: 3, Percentage: 1.0 }] },
            //                    { SuspectionLevelId: 3, StrategyLevelCriterias: [{ CriteriaId: 1, Percentage: 1.25 }, { CriteriaId: 2, Percentage: 0.75 }, { CriteriaId: 3, Percentage: 1.0 }] }
            //                 ]

            
        };



        

        angular.forEach($scope.strategyFilters, function (filter) {
            var filterItem = {
                filterId: filter.filterId,
                description: filter.description,
                threshold: filter.threshold,
                minimumValue: filter.minimumValue,
                period: filter.period,
                isSelected: filter.isSelected

            
            };

            strategyObject.strategyFilters.push(filterItem);
        });


        


        console.log(strategyObject);
        
        return strategyObject;
    }

  

    function loadFiltersForAddMode() {
        angular.forEach($scope.filterDefinitions, function (filterDef) {
            var filterItem = {
                filterId: filterDef.filterId,
                description: filterDef.description
            };
            //console(filterItem);
            $scope.strategyFilters.push(filterItem);
        });

        
    }

    function fillScopeFromStrategyObj(strategyObject) {
        $scope.name = strategyObject.name;
        $scope.description = strategyObject.description;
        $scope.isDefault = strategyObject.isDefault;

        
        angular.forEach($scope.filterDefinitions, function (filterDef) {

            var filterItem = {
                filterId: filterDef.filterId,
                description: filterDef.description
            };
           
            var existingItem = UtilsService.getItemByVal(strategyObject.strategyFilters, filterDef.filterId, "filterId");
            if (existingItem != undefined && existingItem != null) {

                filterItem.isSelected = true;
                filterItem.threshold = existingItem.threshold;
                filterItem.minimumValue = existingItem.minimumValue;
                if (existingItem.period != undefined)
                    filterItem.period = UtilsService.getItemByVal($scope.periods, existingItem.period, "Id");

               

            }
            $scope.strategyFilters.push(filterItem);
        });

        console.log(strategyObject);

    }

    function AddStrategy() {
        $scope.issaving = true;
        var strategyObject = buildStrategyObjFromScope();

        


        return StrategyAPIService.AddStrategy(strategyObject)
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
        var strategyObject = buildStrategyObjFromScope();
        StrategyAPIService.UpdateStrategy(strategyObject)
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
                $scope.filterDefinitions.push({ filterId: itm.FilterId, description: itm.Description });
            });
        });
    }



    $scope.periods = [];
    $scope.selectedPeriod = "";

    function loadPeriods() {
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
