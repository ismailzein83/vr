WorkflowManagementController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'UtilsService'];

function WorkflowManagementController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, UtilsService) {
    defineScope();
    load();



    function defineScope() {

        $scope.customvalidateFrom = function (fromDate) {
            return validateDates(fromDate, $scope.toDate);
        };
        $scope.customvalidateTo = function (toDate) {
            return validateDates($scope.fromDate, toDate);
        };
        function validateDates(fromDate, toDate) {
            if (fromDate == undefined || toDate == undefined)
                return null;
            var from = new Date(fromDate);
            var to = new Date(toDate);
            if (from.getTime() > to.getTime())
                return "Start should be before end";
            else
                return null;
        }

        $scope.strategies = [];
        $scope.periods = [];
        $scope.selectedPeriod = "";
        $scope.selectedStrategies = [];

      
    }

    function load() {
        loadPeriods();
        loadStrategies();
    }


    //function buildStrategyObjFromScope() {
        
    //    var strategyObject = {
    //        Id: ($scope.strategyId != null) ? $scope.strategyId : 0,
    //        Name: $scope.name,
    //        Description: $scope.description,
    //        IsDefault: $scope.isDefault,
    //        GapBetweenConsecutiveCalls:$scope.gapbetweenconsecutivecalls,
    //        MaxLowDurationCall:$scope.maxLowDurationCall,
    //        MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour,
    //        PeakHours: $scope.selectedPeakHours,
    //        StrategyFilters: [],
    //        StrategyLevels: []
    //    };




    //    angular.forEach($scope.strategyFilters, function (filter) {
    //        if (filter.isSelected)
    //        {
    //            var filterItem = {
    //                FilterId: filter.filterId,
    //                Description: filter.description,
    //                Threshold: filter.threshold


    //            };

    //            if (filter.period != undefined)
    //                filterItem.PeriodId = filter.period.Id;


    //            strategyObject.StrategyFilters.push(filterItem);
    //        }
           
    //    });





    //    angular.forEach($scope.strategyLevels, function (level) {
           
    //        var strategyLevelItem = {
    //            SuspicionLevelId: level.suspicionLevel.id,
    //            StrategyLevelCriterias: []
    //        };



           

    //        var index = 0;
    //        angular.forEach(level.StrategyLevelCriterias, function (levelCriteria) {

    //            if ($scope.strategyFilters[index].isSelected && levelCriteria.isSelected) {

                   

    //                var levelCriteriaItem = {
    //                    FilterId: $scope.strategyFilters[index].filterId
    //                };

    //                if (levelCriteria.percentage != undefined) {
    //                    levelCriteriaItem.Percentage = levelCriteria.percentage.value;
    //                }


    //                strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
    //            }

    //            index++;

    //        });

    //        strategyObject.StrategyLevels.push(strategyLevelItem);


    //    });

       
    //    console.log(strategyObject)

    //    return strategyObject;
    //}

    //function StartWorkflow() {
    //    $scope.issaving = true;
    //    var strategyObject = buildStrategyObjFromScope();

    //    return StrategyAPIService.AddStrategy(strategyObject)
    //      .then(function (response) {
    //          if (VRNotificationService.notifyOnItemAdded("Strategy", response)) {
    //              if ($scope.onStrategyAdded != undefined)
    //                  $scope.onStrategyAdded(response.InsertedObject);
    //              $scope.modalContext.closeModal();
    //          }
    //      }).catch(function (error) {
    //          VRNotificationService.notifyException(error, $scope);
    //      });
    //}
    


    function loadStrategies() {
        return StrategyAPIService.GetAllStrategies().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name });
            });
        });
    }

    
    function loadPeriods() {
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
                
            });
        });
    }





    WorkflowManagementController.isFilterTabShown = true;
    WorkflowManagementController.isLevelsTabShow = false;

    WorkflowManagementController.viewVisibilityChanged = function () {
      
        isFilterTabShown = !isFilterTabShown;
        isLevelsTabShow = !isLevelsTabShow;
    };







}
appControllers.controller('WorkflowManagementController', WorkflowManagementController);
