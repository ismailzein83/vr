(function (appControllers) {

    "use strict";

    teamEditorController.$inject = ['$scope', 'Demo_Module_TeamAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function teamEditorController($scope, Demo_Module_TeamAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var teamID;
        var teamEntity;
        var leagueIDItem;

        var leagueSelectorAPI;
        var leagueSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var teamSettingsDirectiveAPI;
        var teamSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                teamID = parameters.teamID;
                leagueIDItem = parameters.leagueIDItem;
            }

            isEditMode = (teamID != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onLeagueSelectorReady = function (api) {
                leagueSelectorAPI = api;
                leagueSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onTeamSettingsDirectiveReady = function (api) {
                teamSettingsDirectiveAPI = api;
                teamSettingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.saveTeam = function () {
                if (isEditMode)
                    return updateTeam();
                else
                    return insertTeam();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            $scope.scopeModel.disableLeague = leagueIDItem != undefined;

            if (isEditMode) {
                getTeam().then(function () {
                    loadAllControls().finally(function () {
                        teamEntity = undefined;
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

        function getTeam() {
            return Demo_Module_TeamAPIService.GetTeamByID(teamID).then(function (response) {
                teamEntity = response;
            });
        }

        function loadAllControls() {
            function setTitle() {
                if (isEditMode && teamEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(teamEntity.Name, "Team");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Team");
            }

            function loadStaticData() {
                if (teamEntity != undefined) {
                    $scope.scopeModel.name = teamEntity.Name;
                }
            }

            function loadLeagueSelector() {
                var leagueSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                leagueSelectorReadyDeferred.promise.then(function () {

                    var leagueSelectorPayload = {};

                    if (teamEntity != undefined) {
                        leagueSelectorPayload.selectedIds = teamEntity.LeagueID;
                    } else if (leagueIDItem != undefined) {
                        leagueSelectorPayload.selectedIds = leagueIDItem.LeagueID;
                    }
                    VRUIUtilsService.callDirectiveLoad(leagueSelectorAPI, leagueSelectorPayload, leagueSelectorLoadPromiseDeferred);
                });

                return leagueSelectorLoadPromiseDeferred.promise;
            }

            function loadTeamSettingsDirective() {
                var teamSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                teamSettingsDirectiveReadyDeferred.promise.then(function () {

                    var teamSettingsPayload;
                    if (teamEntity != undefined) {
                        teamSettingsPayload = {
                            teamSettingsEntity: teamEntity.Settings
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(teamSettingsDirectiveAPI, teamSettingsPayload, teamSettingsLoadPromiseDeferred);
                });

                return teamSettingsLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadLeagueSelector, loadTeamSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertTeam() {
            $scope.scopeModel.isLoading = true;

            var teamObject = buildTeamObjectFromScope();
            Demo_Module_TeamAPIService.AddTeam(teamObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Team", response, "Name")) {
                    if ($scope.onTeamAdded != undefined) {
                        $scope.onTeamAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateTeam() {
            $scope.scopeModel.isLoading = true;

            var teamObject = buildTeamObjectFromScope();
            Demo_Module_TeamAPIService.UpdateTeam(teamObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Team", response, "Name")) {
                    if ($scope.onTeamUpdated != undefined) {
                        $scope.onTeamUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildTeamObjectFromScope() {
            var object = {
                TeamID: teamID != undefined ? teamID : undefined,
                Name: $scope.scopeModel.name,
                LeagueId: leagueSelectorAPI.getSelectedIds(),
                Settings: teamSettingsDirectiveAPI.getData()
            };
            return object;
        }
    }

    appControllers.controller('Demo_Module_TeamEditorController', teamEditorController);
})(appControllers);