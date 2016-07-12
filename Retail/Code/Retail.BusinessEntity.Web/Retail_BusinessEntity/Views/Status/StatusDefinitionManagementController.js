(function (appControllers) {

    "use strict";

    StatusDefinitionManagementController.$inject = ['$scope', 'Retail_BE_StatusDefinitionService', 'Retail_BE_StatusDefinitionAPIService'];

    //function StatusDefinitionManagementController($scope, WhS_BE_SellingNumberPlanService, WhS_BE_SellingNumberPlanAPIService)
    function StatusDefinitionManagementController($scope, Retail_BE_StatusDefinitionService, Retail_BE_StatusDefinitionAPIService) {

        $scope.name = "Status Definition";
        $scope.id = "Vanrise";

        function Get() {
            $.ajax({
                type: "GET",
                url: _url, //URI  

                dataType: "json",
                success: function (data) {
                    debugger;

                    var myJsonObject = data;
                    contentType: "application/json";
                } ,
                error: function (xhr) {
                    alert('Problem in Get car: ' + xhr.responseText);
                }
            });
        }
        function Post() {

            $.ajax({
                type: "POST",
                data: null,
                url: "http://localhost:6060/api/Retail_BE/StatusDefinition/GetFilteredStatusDefinition",
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    debugger;
                    $scope.statusDefinition = data;
                    contentType: "application/json";
                },
                error: function (xhr) {
                    alert('Problem in Post car: ' + xhr.responseText);
                }
            });
        }
        //Post();


        //var response = { "$type": "Vanrise.Entities.BigResult`1[[Retail.BusinessEntity.Entities.StatusDefinitionDetail, Retail.BusinessEntity.Entities]], Vanrise.Entities", "ResultKey": null, "Data": [{ "$type": "Retail.BusinessEntity.Entities.StatusDefinitionDetail, Retail.BusinessEntity.Entities", "Entity": { "$type": "Retail.BusinessEntity.Entities.StatusDefinition, Retail.BusinessEntity.Entities", "StatusDefinitionId": "ddb6a5b8-b9e5-4050-bee8-0f030e801b8b", "Name": "Active", "Settings": null } }, { "$type": "Retail.BusinessEntity.Entities.StatusDefinitionDetail, Retail.BusinessEntity.Entities", "Entity": { "$type": "Retail.BusinessEntity.Entities.StatusDefinition, Retail.BusinessEntity.Entities", "StatusDefinitionId": "1ab487d6-4560-4ac9-80a0-b870e18df273", "Name": "Blocked", "Settings": null } }, { "$type": "Retail.BusinessEntity.Entities.StatusDefinitionDetail, Retail.BusinessEntity.Entities", "Entity": { "$type": "Retail.BusinessEntity.Entities.StatusDefinition, Retail.BusinessEntity.Entities", "StatusDefinitionId": "007869d9-6dc2-4f56-88a4-18c8c442e49e", "Name": "Suspended", "Settings": null } }, { "$type": "Retail.BusinessEntity.Entities.StatusDefinitionDetail, Retail.BusinessEntity.Entities", "Entity": { "$type": "Retail.BusinessEntity.Entities.StatusDefinition, Retail.BusinessEntity.Entities", "StatusDefinitionId": "8a359658-75d4-47fb-8b3b-1f940c3faa58", "Name": "Terminated", "Settings": null } }], "TotalCount": 4 };
        //$scope.statusData = response.Data;

        //console.log(response);
        //Retail_BE_StatusDefinitionAPIService.GetFilteredStatusDefinition({ SortByColumnName: "Entity.Name" })
        //                                    .then(function (response) {
        //                                            $scope.statusData = response.Data;
        //                                    });

        var gridAPI;

        $scope.scopeModel = {};
        $scope.scopeModel.statusDefinition = [];
        $scope.scopeModel.menuActions = [];
        $scope.scopeModel.onGridReady = function (api) {
            gridAPI = api;
            //defineAPI();
            retrieveData(null);
        };

        $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return Retail_BE_StatusDefinitionAPIService.GetFilteredStatusDefinition(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        };


        function retrieveData(query) {
            return gridAPI.retrieveData(query);
        };



    }

    appControllers.controller('Retail_BE_StatusDefinitionManagementController', StatusDefinitionManagementController);

})(appControllers);