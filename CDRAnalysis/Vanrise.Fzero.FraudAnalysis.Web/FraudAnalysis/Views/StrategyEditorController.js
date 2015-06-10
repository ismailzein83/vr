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
        $scope.percentages = [
                         { description: '-75%', value: 0.25 }, { description: '-50%', value: 0.5 }, { description: '-25%', value: 0.75 }, { description: '0%', value: 1.00 }, { description: '25%', value: 1.25 }, { description: '50%', value: 1.50 }, { description: '75%', value: 1.75 }

        ];

        $scope.suspectionLevels = [
                         { id: 2, name: 'Suspicious' }, { id: 3, name: 'Highly Suspicious' }, { id: 4, name: 'Fraud' }

        ];

        $scope.selectedSuspectionLevel = $scope.suspectionLevels[0];

        $scope.strategyLevels = [];


        $scope.AddSuspectionLevel = function () {

            var indexSuspectionLevel = UtilsService.getItemIndexByVal($scope.suspectionLevels, $scope.selectedSuspectionLevel.id, "id");

            $scope.strategyLevels.push({ suspectionLevel: $scope.suspectionLevels[indexSuspectionLevel], StrategyLevelCriterias: [{ filterId: 1, percentage: $scope.percentages[3] }, { filterId: 2, percentage: $scope.percentages[3] }, { filterId: 3, percentage: $scope.percentages[3] }, { filterId: 1, percentage: $scope.percentages[3] }, { filterId: 2, percentage: $scope.percentages[3] }, { filterId: 3, percentage: $scope.percentages[3] }, { filterId: 1, percentage: $scope.percentages[3] }, { filterId: 2, percentage: $scope.percentages[3] }, { filterId: 3, percentage: $scope.percentages[3] }, { filterId: 1, percentage: $scope.percentages[3] }, { filterId: 2, percentage: $scope.percentages[3] }, { filterId: 3, percentage: $scope.percentages[3] }, { filterId: 1, percentage: $scope.percentages[3] }, { filterId: 2, percentage: $scope.percentages[3] }, { filterId: 3, percentage: $scope.percentages[3] }, { filterId: 1, percentage: $scope.percentages[3] }, { filterId: 2, percentage: $scope.percentages[3] }, { filterId: 3, percentage: $scope.percentages[3] }, { filterId: 1, percentage: $scope.percentages[3] }] });
        };


        

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
            Id: ($scope.strategyId != null) ? $scope.strategyId : 0,
            Name: $scope.name,
            Description: $scope.description,
            IsDefault: $scope.isDefault,
            StrategyFilters: [],
            StrategyLevels: []
        };




        angular.forEach($scope.strategyFilters, function (filter) {
            var filterItem = {
                FilterId: filter.filterId,
                Description: filter.description,
                Threshold: filter.threshold,
                MinimumValue: filter.minimumValue,
                IsSelected: filter.isSelected


            };

            console.log(filter.period)
            if (filter.period != undefined)
                filterItem.PeriodId = filter.period.Id;

            strategyObject.StrategyFilters.push(filterItem);
        });


             


        angular.forEach($scope.strategyLevels, function (level) {

            var strategyLevelItem = {
                SuspectionLevelId: level.suspectionLevel.id,
                StrategyLevelCriterias: []
            };


            angular.forEach(level.StrategyLevelCriterias, function (levelCriteria) {
                
                if (levelCriteria.isSelected && levelCriteria.isSelected != undefined) {

                    var levelCriteriaItem = {
                        FilterId: levelCriteria.filterId,
                        Percentage: levelCriteria.percentage.value
                    };

                    console.log("levelCriteriaItem.FilterId" + levelCriteriaItem.FilterId)
                    console.log("levelCriteriaItem.Percentage" + levelCriteriaItem.Percentage)
                    console.log("levelCriteriaItem.FilterId" + levelCriteriaItem.FilterId)

                    strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem)  ;

                }

               

            });

            strategyObject.StrategyLevels.push(strategyLevelItem);


        });



        console.log(strategyObject)

        return strategyObject;
    }



    function loadFiltersForAddMode() {
        angular.forEach($scope.filterDefinitions, function (filterDef) {
            var filterItem = {
                filterId: filterDef.filterId,
                description: filterDef.description
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
                description: filterDef.description
            };



            var existingItem = UtilsService.getItemByVal(strategyObject.StrategyFilters, filterDef.filterId, "FilterId");
            if (existingItem != undefined && existingItem != null) {
                filterItem.isSelected = existingItem.IsSelected;
                filterItem.threshold = existingItem.Threshold;
                filterItem.minimumValue = existingItem.MinimumValue;
                console.log(existingItem.PeriodId);

                if (existingItem.PeriodId != undefined)
                    filterItem.period = UtilsService.getItemByVal($scope.periods, existingItem.PeriodId, "Id");



            }
            $scope.strategyFilters.push(filterItem);
        });


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


    $scope.filterDefinitions = [];


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
