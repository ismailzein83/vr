(function (appControllers) {

    "use strict";

    leagueEditorController.$inject = ['$scope', 'Demo_Module_LeagueAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function leagueEditorController($scope, Demo_Module_LeagueAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var isEditMode;
        var leagueID;
        var leagueEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                leagueID = parameters.leagueID;
            }

            isEditMode = (leagueID != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveLeague = function () {
                if (isEditMode)
                    return updateLeague();
                else
                    return insertLeague();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
                $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getLeague().then(function () {
                    loadAllControls().finally(function () {
                        leagueEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }
        }

        function getLeague() {
            return Demo_Module_LeagueAPIService.GetLeagueByID(leagueID).then(function (response) {
                leagueEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && leagueEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(leagueEntity.Name, "League");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("League");
            }

            function loadStaticData() {
                if (leagueEntity != undefined) {
                    $scope.scopeModel.name = leagueEntity.Name;
                    $scope.scopeModel.country = leagueEntity.Settings.Country;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertLeague() {
            $scope.scopeModel.isLoading = true;

            var leagueObject = buildLeagueObjectFromScope();
            return Demo_Module_LeagueAPIService.AddLeague(leagueObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("League", response, "Name")) {
                    if ($scope.onLeagueAdded != undefined) {
                        $scope.onLeagueAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateLeague() {
            $scope.scopeModel.isLoading = true;

            var leagueObject = buildLeagueObjectFromScope();

            return Demo_Module_LeagueAPIService.UpdateLeague(leagueObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("League", response, "Name")) {
                    if ($scope.onLeagueUpdated != undefined) {
                        $scope.onLeagueUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildLeagueObjectFromScope() {
            var object = {
                LeagueID: leagueID != undefined ? leagueID : undefined,
                Name: $scope.scopeModel.name,
                Settings: {
                    Country: $scope.scopeModel.country
                }
            };
            return object;
        }
    }

    appControllers.controller('Demo_Module_LeagueEditorController', leagueEditorController);

})(appControllers);