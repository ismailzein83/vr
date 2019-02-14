'use strict';

app.directive('partnerportalCustomeraccessRetailFaultticketStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService','CP_MultiNet_FaultTicketSourceEnum',
    function (UtilsService, VRUIUtilsService, VRDateTimeService, CP_MultiNet_FaultTicketSourceEnum) {
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
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/FaultTicket/Templates/FaultTicketStaticEditor.html"
        };

        function retailMultinetFaultticketStaticeditor(ctrl, $scope, $attrs) {


            var selectedValues = {};
            var attachmentGridAPI;
            var attachmentGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var reasonSelectorAPI;
            var reasonSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var typeSelectorAPI;
            var typeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var retailSubAccountsSelectorAPI;
            var retailSubAccountsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.descriptionSettings = [];
                $scope.scopeModel.showAccountSelector = false;

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

                $scope.scopeModel.onRetailSubAccountsSelectorReady = function (api) {
                    retailSubAccountsSelectorAPI = api;
                    retailSubAccountsReadyPromiseDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([attachmentGridReadyPromiseDeferred.promise, reasonSelectorReadyPromiseDeferred.promise, typeSelectorReadyPromiseDeferred.promise, retailSubAccountsReadyPromiseDeferred.promise]).then(function () {
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
                            $scope.scopeModel.descriptionSettings = selectedValues.TicketDetails != undefined ? selectedValues.TicketDetails : [];
                        }
                    }

                    if (!$scope.scopeModel.isEditMode) {
                        promises.push(loadReasonSelector());
                        promises.push(loadTypeSelector());
                    }
                    promises.push(loadAttachmentGrid());
                    promises.push(loadStatusSelector());
                    promises.push(loadRetailSubAccountsSelector());


                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                    });

                };

                api.setData = function (faultTicketObject) {
                    if (!$scope.scopeModel.isEditMode) {
                        faultTicketObject.FromDate = $scope.scopeModel.fromDate;
                        faultTicketObject.ToDate = $scope.scopeModel.toDate;
                        faultTicketObject.TicketDetails = UtilsService.serializetoJson({
                            $type: "Retail.BusinessEntity.Business.FaultTicketSettingsDetailsCollection,Retail.BusinessEntity.Business",
                            $values: getDescriptionSettingsListData()
                        });
                    }
                    faultTicketObject.Notes = $scope.scopeModel.notes;
                    faultTicketObject.StatusId = statusSelectorAPI != undefined ? statusSelectorAPI.getSelectedIds() : undefined;
                    faultTicketObject.Attachments = attachmentGridAPI != undefined ? attachmentGridAPI.getData() : undefined;
                    faultTicketObject.SubscriberId = $scope.scopeModel.showAccountSelector && retailSubAccountsSelectorAPI != undefined ? retailSubAccountsSelectorAPI.getSelectedIds() : undefined;
                    faultTicketObject.SourceId = CP_MultiNet_FaultTicketSourceEnum.Portal.value;
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
                        $type: "Retail.BusinessEntity.Business.FaultTicketDescriptionSettingDetails,Retail.BusinessEntity.Business",
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

            function loadStatusSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "c458d884-29b5-4f82-b528-3457222a94a6",
                    filter: {
                        Filters: [getStatusSelectorFilter()]
                    },
                    selectedIds: selectedValues != undefined ? selectedValues.StatusId : undefined,
                    selectfirstitem: selectedValues==undefined,
                    connectionId: "7A2044F4-B42C-44AA-BFB8-6538904E8E4C"
                };
                return statusSelectorAPI.load(selectorPayload);
            }
            function loadTypeSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "d53efed2-eeda-4c94-8d9d-a080428cd616"
                };
                return typeSelectorAPI.load(selectorPayload);
            }
            function loadReasonSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "1f08c17b-24fd-4829-a7bf-94d542ca8fef"
                };
                return reasonSelectorAPI.load(selectorPayload);
            }

            function loadRetailSubAccountsSelector() {
                var retailSubAccountsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                retailSubAccountsReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: selectedValues != undefined ? selectedValues.SubscriberId : undefined,
                        businessEntityDefinitionId: "f5093298-6216-4728-b462-74c5a8ccd2fb"
                    };
                    VRUIUtilsService.callDirectiveLoad(retailSubAccountsSelectorAPI, payload, retailSubAccountsSelectorLoadDeferred);
                });
                retailSubAccountsSelectorLoadDeferred.promise.then(function () {
                    retailSubAccountsSelectorAPI.selectIfSingleItem();
                    $scope.scopeModel.isSingleAccount = retailSubAccountsSelectorAPI.isSingleItem();
                    $scope.scopeModel.showAccountSelector = !retailSubAccountsSelectorAPI.isDataSourceEmpty();
                });
                return retailSubAccountsSelectorLoadDeferred.promise;
            }

            function getStatusSelectorFilter() {
                return {
                    $type: "Retail.BusinessEntity.Business.FaultTicketStatusDefinitionFilter,Retail.BusinessEntity.Business",
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