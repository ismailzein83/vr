"use strict";

app.directive("cdranalysisPstnSwitchbrandGrid", ["CDRAnalysis_PSTN_SwitchBrandService", "CDRAnalysis_PSTN_SwitchBrandAPIService", "VRNotificationService",
    function (CDRAnalysis_PSTN_SwitchBrandService, CDRAnalysis_PSTN_SwitchBrandAPIService,  VRNotificationService) {
    
    var directiveDefinitionObj = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var switchGrid = new SwitchGrid($scope, ctrl);
            switchGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
           
        },
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/SwitchBrand/Templates/SwitchBrandGridTemplate.html"

    };

    function SwitchGrid($scope, ctrl) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController()
        {
            $scope.brands = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
             
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.retrieveData = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onSwitchBrandAdded = function (switchBrandObj) {
                        gridAPI.itemAdded(switchBrandObj);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CDRAnalysis_PSTN_SwitchBrandAPIService.GetFilteredBrands(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
            
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [
               {
                   name: "Edit",
                   clicked: editSwitchBrand,
                   haspermission: hasUpdateSwitchBrandPermission
               },
               {
                   name: "Delete",
                   clicked: deleteSwitchBrand,
                   haspermission: hasDeleteSwitchBrandPermission
               }
            ];

            function hasUpdateSwitchBrandPermission() {
                return CDRAnalysis_PSTN_SwitchBrandAPIService.HasUpdateSwitchBrandPermission();
            }
            function hasDeleteSwitchBrandPermission() {
                return CDRAnalysis_PSTN_SwitchBrandAPIService.HasDeleteSwitchBrandPermission();
            }

        }


        function editSwitchBrand(switchBrandObj) {
            var onSwitchBrandUpdated = function (switchBrandObj) {
                gridAPI.itemUpdated(switchBrandObj);
            };
            CDRAnalysis_PSTN_SwitchBrandService.editSwitchBrand(switchBrandObj.BrandId, onSwitchBrandUpdated);
        }

        function deleteSwitchBrand(switchBrandObj) {
            var onSwitchBrandDeleted = function (deletedSwitchBrandObj) {
                gridAPI.itemDeleted(deletedSwitchBrandObj);
            };
            CDRAnalysis_PSTN_SwitchBrandService.deleteSwitchBrand(switchBrandObj, onSwitchBrandDeleted);
        }
    }

    return directiveDefinitionObj;

}]);
