﻿(function (appControllers) {

    "use strict";

    FirewallEditorController.$inject = ['$scope', 'NP_IVSwitch_FirewallAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService'];

    function FirewallEditorController($scope, npIvSwitchFirewallApiService, vrNotificationService, utilsService, vrNavigationService) {

        var isEditMode;
        var firewallId;
        var firewallEntity;
        var context;
        var isViewHistoryMode;
        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                firewallId = parameters.Id;
                context = parameters.context;
            }
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
            isEditMode = (firewallId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.hasSaveFirewallPermission = function () {
                if (isEditMode) {
                    return npIvSwitchFirewallApiService.HasEditFirewallPermission();
                }
                else {
                    return npIvSwitchFirewallApiService.HasAddFirewallPermission();
                }
            };


        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getFirewall().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getFirewallHistory().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }

            else {
                loadAllControls();
            }
        }
        function getFirewallHistory() {
            return npIvSwitchFirewallApiService.GetFirewallHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                firewallEntity = response;

            });
        }
        function getFirewall() {
            return npIvSwitchFirewallApiService.GetFirewall(firewallId).then(function (response) {
				firewallEntity = response;
            });
        }

        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
               
            });

            function setTitle() {
                if (isEditMode) {
                    var description = (firewallEntity != undefined) ? firewallEntity.Description : null;
                    $scope.title = utilsService.buildTitleForUpdateEditor(description, 'Firewall');
                }
                else if (isViewHistoryMode && firewallEntity != undefined)
                    $scope.title = "View Firewall: " + firewallEntity.Description;

                else {
                    $scope.title = utilsService.buildTitleForAddEditor('Firewall');
                }
            }
            function loadStaticData() {
                if (firewallEntity == undefined)
                    return;
                $scope.scopeModel.description = firewallEntity.Description;
				$scope.scopeModel.IPaddress = firewallEntity.Host;
				var ipAddressWithMask = firewallEntity.Host.split("/");
				if (ipAddressWithMask != undefined) {
					if (ipAddressWithMask[0] != undefined)
						$scope.scopeModel.IPaddress = ipAddressWithMask[0];
					if (ipAddressWithMask[1] != undefined)
						$scope.scopeModel.subnet= ipAddressWithMask[1];
				}
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return npIvSwitchFirewallApiService.AddFirewall(buildFirewallObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemAdded('Firewall', response, 'Host')) {
                    if ($scope.onFirewallAdded != undefined)
                        $scope.onFirewallAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
		function update() {
            $scope.scopeModel.isLoading = true;
            return npIvSwitchFirewallApiService.UpdateFirewall(buildFirewallObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemUpdated('Firewall', response, 'Host')) {
                    if ($scope.onFirewallUpdated != undefined) {
                        $scope.onFirewallUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
                //firewallEntity = undefined;
            });
        }

        function buildFirewallObjFromScope() {
            return {
				Id: firewallEntity != undefined ? firewallEntity.Id : undefined,
				Host: $scope.scopeModel.subnet != undefined ? $scope.scopeModel.IPaddress + "/" + $scope.scopeModel.subnet : $scope.scopeModel.IPaddress,
                Description: $scope.scopeModel.description
            };
        }
    }
    appControllers.controller('NP_IVSwitch_FirewallEditorController', FirewallEditorController);

})(appControllers);