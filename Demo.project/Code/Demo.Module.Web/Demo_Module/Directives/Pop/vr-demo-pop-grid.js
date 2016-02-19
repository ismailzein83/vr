"use strict";

app.directive("vrDemoPopGrid", ["UtilsService", "VRNotificationService", "Demo_PopAPIService", "Demo_PopService",
function (UtilsService, VRNotificationService, Demo_PopAPIService, Demo_PopService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var popGrid = new PopGrid($scope, ctrl, $attrs);
            popGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/Pop/Templates/PopGridTemplate.html"

    };

    function PopGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.pops = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                       
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onPopAdded = function (popObject) {
                        gridAPI.itemAdded(popObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_PopAPIService.GetFilteredPops(dataRetrievalInput)
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
            $scope.gridMenuActions = [{
                name: "Edit",
               clicked: editPop
            }];
        }

        function editPop(popObj) {
            var onPopUpdated = function (popObj) {
                gridAPI.itemUpdated(popObj);
            }
            Demo_PopService.editPop(popObj.PopId, onPopUpdated);
        }
        
    }

    return directiveDefinitionObject;

}]);
