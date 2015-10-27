"use strict";

app.directive("vrWhsBeCodegroupGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CodeGroupAPIService", "WhS_BE_MainService",
function (UtilsService, VRNotificationService, WhS_BE_CodeGroupAPIService, WhS_BE_MainService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var codeGroupGrid = new CodeGroupGrid($scope, ctrl, $attrs);
            codeGroupGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CodeGroup/Templates/CodeGroupGridTemplate.html"

    };

    function CodeGroupGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.codegroups = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                       
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onCodeGroupAdded = function (codeGroupObject) {
                        gridAPI.itemAdded(codeGroupObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CodeGroupAPIService.GetFilteredCodeGroups(dataRetrievalInput)
                    .then(function (response) {
                        //if (response.Data != undefined) {
                        //    for (var i = 0; i < response.Data.length; i++) {
                        //        setDataItemExtension(response.Data[i]);
                        //    }
                        //}
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        //function setDataItemExtension(dataItem) {

        //    var extensionObject = {};
        //    var query = {
        //        CarrierProfilesIds: [dataItem.CarrierProfileId],
        //    }
        //    extensionObject.onGridReady = function (api) {
        //        extensionObject.carrierAccountGridAPI = api;
        //        extensionObject.carrierAccountGridAPI.loadGrid(query);
        //        extensionObject.onGridReady = undefined;
        //    };
        //    dataItem.extensionObject = extensionObject;

        //}

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
               clicked: editCodeGroupe
            }
           // ,
           //{
           //    name: "New Carrier Account",
           //    clicked: addCarrierAccount
           //}
            ];
        }

        function editCodeGroupe(codeGroupObj) {
            var onCodeGroupUpdated = function (codeGroupObj) {
                gridAPI.itemUpdated(codeGroupObj);
            }

            WhS_BE_MainService.editCodeGroup(codeGroupObj, onCodeGroupUpdated);
        }
        //function addCarrierAccount(dataItem) {
        //    gridAPI.expandRow(dataItem);
        //    var query = {
        //        CarrierProfilesIds: [dataItem.CarrierProfileId],
        //    }
        //    if (dataItem.extensionObject.carrierAccountGridAPI != undefined)
        //        dataItem.extensionObject.carrierAccountGridAPI.loadGrid(query);
        //    var onCarrierAccountAdded = function (carrierAccountObj) {
        //        if (dataItem.extensionObject.carrierAccountGridAPI != undefined)
        //            dataItem.extensionObject.carrierAccountGridAPI.onCarrierAccountAdded(carrierAccountObj);
        //    };
        //    WhS_BE_MainService.addCarrierAccount(onCarrierAccountAdded, dataItem);
        //}
        //function deleteCarrierProfile(carrierProfileObj) {
        //    var onCarrierProfileDeleted = function () {
        //        //TODO: This is to refresh the Grid after delete, should be removed when centralized
        //        retrieveData();
        //    };

        //    WhS_BE_MainService.deleteCarrierAccount(carrierProfileObj, onCarrierProfileDeleted);
        //}
    }

    return directiveDefinitionObject;

}]);
