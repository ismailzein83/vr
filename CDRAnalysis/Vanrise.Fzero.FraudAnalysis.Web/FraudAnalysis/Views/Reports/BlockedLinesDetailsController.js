﻿"use strict";

BlockedLinesDetailsController.$inject = ['$scope', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function BlockedLinesDetailsController($scope,  $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    loadParameters();
    defineScope();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.accountNumbers = undefined;

        if (parameters != undefined && parameters != null)
            $scope.accountNumbers = parameters.accountNumbers;
    }

    function defineScope() {
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }
   
}
appControllers.controller('BlockedLinesDetailsController', BlockedLinesDetailsController);
