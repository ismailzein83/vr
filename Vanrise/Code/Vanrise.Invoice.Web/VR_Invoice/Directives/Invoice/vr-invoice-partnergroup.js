﻿'use strict';

app.directive('vrInvoicePartnergroup', ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Invoice_InvoiceAPIService',
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new partnerGroupCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'partnerGroupCtrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/PartnerGroupTemplate.html"

    };


    function partnerGroupCtor(ctrl, $scope, $attrs) {
        var invoiceTypeId;
        var accountStatus;
        var partnerGroupSelectorAPI;

        var partnerGroupDirectiveAPI;
        var partnerGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {


            $scope.partnerGroupTemplates = [];

            $scope.onPartnerGroupSelectorReady = function (api) {
                partnerGroupSelectorAPI = api;
                defineAPI();
            };

            $scope.onPartnerGroupDirectiveReady = function (api) {
                partnerGroupDirectiveAPI = api;
                var partnerGroupDirectivePayload = { invoiceTypeId: invoiceTypeId, accountStatus: accountStatus };
                var setLoader = function (value) {
                    $scope.isLoadingPartnerGroupTemplateDirective = value;
                };

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerGroupDirectiveAPI, partnerGroupDirectivePayload, setLoader);

            };
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {

                var partnerGroupSettings;
                if ($scope.selectedPartnerGroupTemplate != undefined) {
                    if (partnerGroupDirectiveAPI != undefined) {
                        partnerGroupSettings = partnerGroupDirectiveAPI.getData();
                        partnerGroupSettings.ConfigId = $scope.selectedPartnerGroupTemplate.ExtensionConfigurationId;
                    }
                }
                return partnerGroupSettings;
            };

            api.load = function (payload) {
                partnerGroupSelectorAPI.clearDataSource();
                if (payload != undefined) {
                    invoiceTypeId = payload.invoiceTypeId;
                    accountStatus = payload.accountStatus;
                }

                var promises = [];

                var loadPartnerGroupTemplatesPromise = loadPartnerGroupTemplates();
                promises.push(loadPartnerGroupTemplatesPromise);

                function loadPartnerGroupTemplates() {
                    return VR_Invoice_InvoiceAPIService.GetPartnerGroupTemplates().then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.partnerGroupTemplates.push(item);
                        });
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.accountStatusChanged = function (newAccountStatus) {
                accountStatus = newAccountStatus;
                if (partnerGroupDirectiveAPI != undefined) {
                    partnerGroupDirectiveAPI.accountStatusChanged(accountStatus);
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);