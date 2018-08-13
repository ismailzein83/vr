'use strict';

app.directive('retailMultinetFaultticketStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailMultinetFaultticketStaticeditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/Retail_MultiNet/Directives/FaultTicket/Templates/FaultTicketStaticEditor.html"
        };

        function retailMultinetFaultticketStaticeditor(ctrl, $scope, $attrs) {


            var selectedValues = {};
            var accountSelectorAPI;
            var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var attachmentGridAPI;
            var attachmentGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();



            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountSelectorReady = function (api) {
                    accountSelectorAPI = api;
                    accountSelectorReadyDeferred.resolve();
                };
                $scope.onAttachmentGridReady = function (api) {
                    attachmentGridAPI = api;
                    attachmentGridReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onStatusSelectorReady = function (api) {
                    statusSelectorAPI = api;
                    statusSelectorReadyPromiseDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([accountSelectorReadyDeferred.promise, attachmentGridReadyPromiseDeferred.promise]).then(function () {

                    defineApi();
                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            $scope.scopeModel.fromDate = selectedValues.FromDate;
                            $scope.scopeModel.toDate = selectedValues.ToDate;
                            $scope.scopeModel.notes = selectedValues.Notes;
                            $scope.scopeModel.sendEmail = selectedValues.SendEmail;
                            $scope.scopeModel.phoneNumber = selectedValues.PhoneNumber;
                            $scope.scopeModel.email = selectedValues.ContactEmails;
                        }
                    }

                    promises.push(loadAccountSelector());
                    promises.push(loadAttachmentGrid());
                   // promises.push(loadStatusSelector());


                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                    });

                };

                api.setData = function (faultTicketObject) {
                    faultTicketObject.SubscriberId = accountSelectorAPI != undefined ? accountSelectorAPI.getSelectedIds() : undefined;
                    faultTicketObject.FromDate = $scope.scopeModel.fromDate;
                    faultTicketObject.ToDate = $scope.scopeModel.toDate;
                    faultTicketObject.Notes = $scope.scopeModel.notes;
                    faultTicketObject.SendEmail = $scope.scopeModel.sendEmail;
                    faultTicketObject.PhoneNumber = $scope.scopeModel.phoneNumber;
                    faultTicketObject.ContactEmails = $scope.scopeModel.email;
                    faultTicketObject.Attachments = attachmentGridAPI != undefined ? attachmentGridAPI.getData() : undefined;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadAttachmentGrid() {

                var attachmentGridPayload = {
                    attachementFieldTypes: selectedValues != undefined ? selectedValues.Attachments : undefined
                };
                return attachmentGridAPI.loadGrid(attachmentGridPayload);
            }

            function loadAccountSelector() {
                var accountSelectorPayload = {
                    businessEntityDefinitionId: "9a427357-cf55-4f33-99f7-745206dee7cd",
                    selectedIds: selectedValues != undefined ? selectedValues.SubscriberId : undefined
                };
                return accountSelectorAPI.load(accountSelectorPayload);
            }
            function loadStatusSelector() {
                var selectorPayload;

                selectorPayload = {
                    businessEntityDefinitionId: "c63202a1-438f-428e-b0fb-3a9bad708e9b",
                    filter: {
                        Filters: []
                    }
                };
                selectorPayload.filter.Filters.push(getStatusSelectorFiter());
                if (selectedValues != undefined)
                    selectorPayload.selectedIds = selectedValues.StatusId;
                selectorPayload.selectfirstitem = !$scope.scopeModel.isEditMode;

                return statusSelectorAPI.load(selectorPayload);
            }

        }
        return directiveDefinitionObject;
    }]);