"use strict";

app.directive("retailTelesTelesdidsGrid", ["UtilsService", "VRNotificationService", "Retail_Teles_EnterpriseAPIService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, Retail_Teles_EnterpriseAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DIDsGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Teles/Directives/TelesEnterprises/Templates/TelesDIDsGridTemplate.html"
        };

        function DIDsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.dids = [];
            
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};

                        directiveAPI.load = function (query) {
                            return gridAPI.retrieveData(query);
                        };


                        return directiveAPI;
                    }
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_Teles_EnterpriseAPIService.GetFilteredEnterpriseDIDs(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

            }
        
        }

        return directiveDefinitionObject;
    }]);