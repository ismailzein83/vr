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

            var reasonSelectorAPI;
            var reasonSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var typeSelectorAPI;
            var typeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();



            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.descriptionSettings = [];

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
                $scope.scopeModel.onReasonSelectorReady = function (api) {
                    reasonSelectorAPI = api;
                    reasonSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onTypeSelectorReady = function (api) {
                    typeSelectorAPI = api;
                    typeSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.disableAddReason = function () {
                    return !($scope.scopeModel.isEditMode == false && typeSelectorAPI.getSelectedIds() != undefined && reasonSelectorAPI.getSelectedIds() != undefined);
                };
                $scope.scopeModel.validateDate = function (date) {
                    return UtilsService.validateDates($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
                };
                $scope.scopeModel.descriptionSettingsHasItem = function () {
                    if ($scope.scopeModel.descriptionSettings.length != 0)
                        return null;
                    return "You must add at least one description";
                };
                $scope.scopeModel.addReason = function () {

                    if (doesReasonAlreadyExist())
                        $scope.scopeModel.errorMessage = "*Reason already exists";
                    else {
                        var faultTicketDescriptionSettingDetails = {
                            TicketReasonId: $scope.scopeModel.reason.GenericBusinessEntityId,
                            TicketReasonDescription: $scope.scopeModel.reason.Name,
                            Type: $scope.scopeModel.type.Name
                        };
                        $scope.scopeModel.descriptionSettings.push(faultTicketDescriptionSettingDetails);
                        $scope.scopeModel.errorMessage = undefined;
                    }
                };
                $scope.removerow = function (dataItem) {
                    if (!$scope.scopeModel.isEditMode) {
                        var index = $scope.scopeModel.descriptionSettings.indexOf(dataItem);
                        $scope.scopeModel.descriptionSettings.splice(index, 1);
                    }

                };

                UtilsService.waitMultiplePromises([accountSelectorReadyDeferred.promise, attachmentGridReadyPromiseDeferred.promise, reasonSelectorReadyPromiseDeferred.promise, typeSelectorReadyPromiseDeferred.promise]).then(function () {

                    defineApi();
                });
            }

            function defineApi() {
                var api = {};
                $scope.scopeModel.isEditMode = false;
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            $scope.scopeModel.isEditMode = true;
                            $scope.scopeModel.fromDate = selectedValues.FromDate;
                            $scope.scopeModel.toDate = selectedValues.ToDate;
                            $scope.scopeModel.notes = selectedValues.Notes;
                            $scope.scopeModel.sendEmail = selectedValues.SendEmail;
                            $scope.scopeModel.phoneNumber = selectedValues.PhoneNumber;
                            $scope.scopeModel.email = selectedValues.ContactEmails;
                            $scope.scopeModel.descriptionSettings = selectedValues.TicketDetails != undefined ? selectedValues.TicketDetails : [];
                        }
                    }

                    if (!$scope.scopeModel.isEditMode) {
                        promises.push(loadReasonSelector());
                        promises.push(loadTypeSelector());
                    }
                    promises.push(loadAccountSelector());
                    promises.push(loadAttachmentGrid());
                    promises.push(loadStatusSelector());



                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                    });

                };

                api.setData = function (faultTicketObject) {
                    if (!$scope.scopeModel.isEditMode) {
                        faultTicketObject.SubscriberId = accountSelectorAPI != undefined ? accountSelectorAPI.getSelectedIds() : undefined;
                        faultTicketObject.FromDate = $scope.scopeModel.fromDate;
                        faultTicketObject.ToDate = $scope.scopeModel.toDate;
                        faultTicketObject.TicketDetails = {
                            $type: "Retail.MultiNet.Entities.FaultTicketSettingsDetailsCollection,Retail.MultiNet.Entities",
                            $values: getDescriptionSettingsListData()
                        };
                    }
                    faultTicketObject.ContactEmails = $scope.scopeModel.email;
                    faultTicketObject.Notes = $scope.scopeModel.notes;
                    faultTicketObject.PhoneNumber = $scope.scopeModel.phoneNumber;
                    faultTicketObject.SendEmail = $scope.scopeModel.sendEmail;
                    faultTicketObject.StatusId = statusSelectorAPI != undefined ? statusSelectorAPI.getSelectedIds() : undefined;
                    faultTicketObject.Attachments = attachmentGridAPI != undefined ? attachmentGridAPI.getData() : undefined;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getDescriptionSettingsListData() {
                var descriptionSettings;
                if ($scope.scopeModel.descriptionSettings.length > 0)
                    descriptionSettings = [];
                for (var i = 0; i < $scope.scopeModel.descriptionSettings.length; i++) {
                    var descriptionSettingObject = $scope.scopeModel.descriptionSettings[i];
                    var faultTicketDescriptionSetting =
                        {
                            $type: "Retail.MultiNet.Entities.FaultTicketDescriptionSettingDetails,Retail.MultiNet.Entities",
                            TicketReasonId: descriptionSettingObject.TicketReasonId,
                            TicketReasonDescription: descriptionSettingObject.TicketReasonDescription,
                            Type: descriptionSettingObject.Type,
                            Note: descriptionSettingObject.Note

                        };
                    descriptionSettings.push(faultTicketDescriptionSetting);
                }
                return descriptionSettings;
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
                    businessEntityDefinitionId: "c458d884-29b5-4f82-b528-3457222a94a6"
                    , filter: {
                        Filters: []
                    }
                };
                selectorPayload.filter.Filters.push(getStatusSelectorFiter());
                if (selectedValues != undefined)
                    selectorPayload.selectedIds = selectedValues.StatusId;
                selectorPayload.selectfirstitem = !$scope.scopeModel.isEditMode;

                return statusSelectorAPI.load(selectorPayload);
            }
            function loadTypeSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "f5649bf1-8cab-42ef-a4af-a61af4da81b6"
                };
                return typeSelectorAPI.load(selectorPayload);
            }
            function loadReasonSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "d3336bdd-13fd-46e2-8615-6852782f59f2"
                };
                return reasonSelectorAPI.load(selectorPayload);
            }

            function getStatusSelectorFiter() {
                return {
                    $type: "Retail.MultiNet.Entities.FaultTicketStatusDefinitionFilter,Retail.MultiNet.Entities",
                    BusinessEntityDefinitionId: "0d7dd0d6-ab3c-4e58-bd5f-926a260f1891",
                    StatusId: selectedValues != undefined ? selectedValues.StatusId : undefined
                };
            }

            function doesReasonAlreadyExist() {
                if ($scope.scopeModel.descriptionSettings != undefined) {
                    for (var i = 0; i < $scope.scopeModel.descriptionSettings.length; i++) {
                        var descriptionSetting = $scope.scopeModel.descriptionSettings[i];
                        if (descriptionSetting.TicketReasonId == reasonSelectorAPI.getSelectedIds() && descriptionSetting.Type == $scope.scopeModel.type.Name)
                            return true;
                    }
                }
                return false;
            }

        }
        return directiveDefinitionObject;
    }]);