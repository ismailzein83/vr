(function (appControllers) {

    "use strict";

    parserTypeManagementController.$inject = ['$scope', 'VR_DataParser_ParserTypeService', 'UtilsService','VRUIUtilsService'];

    function parserTypeManagementController($scope, VR_DataParser_ParserTypeService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        var filter = {};

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
            function getFilterObject() {
                filter = {
                    Name: $scope.name,
                    DevProjectIds: devProjectDirectiveApi != undefined ? devProjectDirectiveApi.getSelectedIds() : undefined
                };
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };

            $scope.addNewParserType = addNewParserType;
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitPromiseNode({ promises: [loadDevProjectSelector()] })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }



        function addNewParserType() {
            var onParserTypeAdded = function (parserTypeObj) {
                gridAPI.onParserTypeAdded(parserTypeObj);
            };
            VR_DataParser_ParserTypeService.addParserType(onParserTypeAdded);
        }


    }

    appControllers.controller('VR_DataParser_ParserTypeManagementController', parserTypeManagementController);
})(appControllers);