(function (appControllers) {

    "use strict";

    genericInvoiceEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService', 'VR_Invoice_InvoiceAPIService', 'VRButtonTypeEnum', 'VR_Invoice_InvoiceActionService'];

    function genericInvoiceEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceAPIService, VRButtonTypeEnum, VR_Invoice_InvoiceActionService) {
        var invoiceTypeId;
        $scope.scopeModel = {};
        var invoiceId;
        $scope.scopeModel.invoiceTypeEntity;
        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var timeZoneSelectorAPI;
        var timeZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceGeneratorActions;
        var invoiceEntity;
        var validateResult = false;
        $scope.scopeModel.isEditMode;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
                invoiceId = parameters.invoiceId;
            }
            $scope.scopeModel.isEditMode = (invoiceId != undefined);
        }

        function defineScope() {
            $scope.scopeModel.actions = [];
            $scope.scopeModel.issueDate = new Date();

            $scope.scopeModel.onTimeZoneSelectorReady = function(api)
            {
                timeZoneSelectorAPI = api;
                timeZoneSelectorReadyDeferred.resolve();
            }
            $scope.scopeModel.onPartnerSelectorReady = function (api) {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.validateToDate = function () {
                var date = new Date();
                if ($scope.scopeModel.toDate >= new Date(date.getFullYear(), date.getMonth(), date.getDate()))
                    return "Date must be less than date of today.";
                return null;
            };
            $scope.scopeModel.validateFromDate = function () {
                if ($scope.scopeModel.toDate < $scope.scopeModel.fromDate)
                    return "From date must be less than to date.";
                return null;
            };
            $scope.scopeModel.generateInvoice = function () {
                if ($scope.scopeModel.isEditMode) {
                    return regenerateInvoice();
                }
                else {
                    return tryGenerateInvoice();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.validateForm = function () {
                var partnerObject;
                if (partnerSelectorAPI != undefined)
                    partnerObject = partnerSelectorAPI.getData();
                if ($scope.scopeModel.issueDate != undefined && partnerObject != undefined && partnerObject.selectedIds != undefined && $scope.scopeModel.fromDate != undefined && $scope.scopeModel.toDate != undefined) {
                    buildInvoiceGeneratorActions();
                    validateResult = true;
                    return null;
                }
                validateResult = false;
                return null;
            };
            function tryGenerateInvoice() {
                var incvoiceObject = buildInvoiceObjFromScope();
                $scope.scopeModel.isLoading = true;
                VR_Invoice_InvoiceAPIService.CheckGeneratedInvoicePeriodGaP(incvoiceObject.FromDate, incvoiceObject.InvoiceTypeId, incvoiceObject.PartnerId).then(function (response) {
                    if(response == undefined)
                    {
                        generateInvoice(incvoiceObject);
                    }else
                    {
                        $scope.scopeModel.isLoading = false;
                        var date = UtilsService.createDateFromString(response);
                        VRNotificationService.showConfirmation("Period skipped , invoice must be generated from  date " + UtilsService.getShortDate(date)+ " of next invoice, are you sure you want to continue ?").then(function (response) {
                            if (response)
                            {
                                generateInvoice(incvoiceObject);
                            }
                        });
                    }
                }).catch(function(error){
                    $scope.scopeModel.isLoading = false;
                }).finally(function(){
                });
            }
            function generateInvoice(incvoiceObject) {
                $scope.scopeModel.isLoading = true;
                return VR_Invoice_InvoiceAPIService.GenerateInvoice(incvoiceObject)
               .then(function (response) {
                   if (VRNotificationService.notifyOnItemAdded("Invoice", response)) {
                       if ($scope.onGenerateInvoice != undefined)
                           $scope.onGenerateInvoice(response.InsertedObject);
                       $scope.modalContext.closeModal();
                   }
               })
               .catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               }).finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
            }
            function regenerateInvoice() {
                $scope.scopeModel.isLoading = true;

                var incvoiceObject = buildInvoiceObjFromScope();
                return VR_Invoice_InvoiceAPIService.ReGenerateInvoice(incvoiceObject)
               .then(function (response) {
                   if (VRNotificationService.notifyOnItemAdded("Invoice", response)) {
                       if ($scope.onGenerateInvoice != undefined)
                           $scope.onGenerateInvoice(response.UpdatedObject);
                       $scope.modalContext.closeModal();
                   }
               })
               .catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               }).finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if ($scope.scopeModel.isEditMode) {
                UtilsService.waitMultipleAsyncOperations([getInvoiceType, getInvoice]).then(function () {
                    loadAllControls();
                }).catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
                  $scope.scopeModel.isLoading = false;
              });
            }
            else {
                getInvoiceType().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            function getInvoice()
            {
              return  VR_Invoice_InvoiceAPIService.GetInvoice(invoiceId).then(function (response) {
                  invoiceEntity = response;
                });
            }
            function getInvoiceType() {
                return VR_Invoice_InvoiceTypeAPIService.GetGeneratorInvoiceTypeRuntime(invoiceTypeId).then(function (response) {
                    $scope.scopeModel.invoiceTypeEntity = response;
                });
            }

        }

        function loadAllControls() {

            function setTitle() {
                if ($scope.scopeModel.isEditMode)
                    $scope.title = "Regenerate Invoice";
                else
                $scope.title = "Generate Invoice";
            }
            function loadPartnerSelectorDirective() {
                var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                partnerSelectorReadyDeferred.promise.then(function () {
                    var partnerSelectorPayload = { context: getContext(), extendedSettings: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.ExtendedSettings };
                    if (invoiceEntity != undefined) {
                        partnerSelectorPayload.selectedIds = invoiceEntity.PartnerId;
                    }
                    VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
                });
                return partnerSelectorPayloadLoadDeferred.promise;
            }
            function loadStaticData() {
                if(invoiceEntity != undefined)
                {
                    $scope.scopeModel.fromDate = invoiceEntity.FromDate;
                    if (invoiceEntity.ToDate != undefined)
                    {
                        var toDate = UtilsService.createDateFromString(invoiceEntity.ToDate);
                        $scope.scopeModel.toDate = new Date(toDate.setHours(23, 59, 59, 998));
                    }
                    $scope.scopeModel.issueDate = invoiceEntity.IssueDate;
                }
            }
            function loadTimeZoneSelector() {
                var timeZoneSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                timeZoneSelectorReadyDeferred.promise.then(function () {
                    var timeZoneSelectorPayload;
                    if (invoiceEntity != undefined) {
                        timeZoneSelectorPayload = {
                            selectedIds: invoiceEntity.TimeZoneId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(timeZoneSelectorAPI, timeZoneSelectorPayload, timeZoneSelectorPayloadLoadDeferred);
                });
                return timeZoneSelectorPayloadLoadDeferred.promise;
            }
            
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPartnerSelectorDirective, loadTimeZoneSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }
  
        function buildInvoiceObjFromScope() {
            var partnerObject = partnerSelectorAPI.getData();
            var obj = {
                InvoiceId:invoiceId,
                InvoiceTypeId: invoiceTypeId,
                PartnerId: partnerObject != undefined ? partnerObject.selectedIds:undefined,
                FromDate: $scope.scopeModel.fromDate,
                ToDate: $scope.scopeModel.toDate,
                IssueDate: $scope.scopeModel.issueDate,
                TimeZoneId: timeZoneSelectorAPI.getSelectedIds()
            };
            return obj;
        }

        function buildInvoiceGeneratorActions() {
            if (!validateResult) {
                $scope.scopeModel.isLoading = true;
                $scope.scopeModel.actions.length = 0;
                function getGeneratorActions() {
                    return VR_Invoice_InvoiceTypeAPIService.GetInvoiceGeneratorActions(buildInvoiceObjFromScope()).then(function (response) {
                        invoiceGeneratorActions = response;
                    });
                }

                return getGeneratorActions().then(function () {
                    if ($scope.scopeModel.invoiceTypeEntity != undefined) {
                        if ($scope.scopeModel.invoiceTypeEntity.InvoiceType != undefined && $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings != undefined && $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.InvoiceGeneratorActions != undefined) {
                            var actionsDictionary = getActionsDictionary();
                            buildActionsFromDictionary(actionsDictionary);
                        }
                    }
                    function getActionsDictionary() {
                        var dictionay = {};

                        for (var i = 0; i < invoiceGeneratorActions.length ; i++) {
                            var invoiceGeneratorAction = invoiceGeneratorActions[i];
                            var buttonType = UtilsService.getEnum(VRButtonTypeEnum, "value", invoiceGeneratorAction.ButtonType);
                            var invoiceAction = UtilsService.getItemByVal($scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.InvoiceActions, invoiceGeneratorAction.InvoiceGeneratorActionId, "InvoiceActionId");

                            if (dictionay[buttonType.value] == undefined) {
                                dictionay[buttonType.value] = [];
                            }
                            dictionay[buttonType.value].push({
                                buttonType: buttonType,
                                invoiceAction: invoiceAction,
                                invoiceGeneratorAction: invoiceGeneratorAction
                            });
                        }
                        return dictionay;
                    }
                    function buildActionsFromDictionary(actionsDictionary) {
                        if (actionsDictionary != undefined) {
                            for (var prop in actionsDictionary) {

                                if (actionsDictionary[prop].length > 1) {
                                    var menuActions = [];
                                    for (var i = 0; i < actionsDictionary[prop].length; i++) {
                                        if (menuActions == undefined)
                                            menuActions = [];
                                        var object = actionsDictionary[prop][i];
                                        addMenuAction(object.invoiceGeneratorAction, object.invoiceAction);
                                    }
                                    function addMenuAction(invoiceGeneratorAction, invoiceAction) {
                                        menuActions.push({
                                            name: invoiceGeneratorAction.Title,
                                            clicked: function () {
                                                return callActionMethod(invoiceAction);
                                            },
                                        });
                                    }

                                    addActionToList(object.buttonType, undefined, menuActions);
                                } else {
                                    var object = actionsDictionary[prop][0];
                                    var clickFunc = function () {
                                        return callActionMethod(object.invoiceAction);
                                    };
                                    addActionToList(object.buttonType, clickFunc, undefined);
                                }
                            }
                        }
                        function addActionToList(buttonType, clickEvent, menuActions) {
                            var type = buttonType != undefined ? buttonType.type : undefined;
                            $scope.scopeModel.actions.push({
                                type: type,
                                onclick: clickEvent,
                                menuActions: menuActions
                            });
                        }

                    }
                    function callActionMethod(invoiceAction) {
                        var partnerObject = partnerSelectorAPI.getData();
                        var payload = {
                            generatorEntity: {
                                invoiceTypeId: invoiceTypeId,
                                partnerId: partnerObject != undefined ? partnerObject.selectedIds : undefined,
                                fromDate: $scope.scopeModel.fromDate,
                                toDate: $scope.scopeModel.toDate,
                                issueDate: $scope.scopeModel.issueDate
                            },
                            invoiceAction: invoiceAction,
                            isPreGenerateAction: true
                        };
                        var actionType = VR_Invoice_InvoiceActionService.getActionTypeIfExist(invoiceAction.Settings.ActionTypeName);

                        var promise = actionType.actionMethod(payload);

                        return promise;
                    }
                    $scope.scopeModel.isLoading = false;
                });
            }
        }

        function getContext()
        {
            var context = {
                reloadPregeneratorActions:function()
                {
                    validateResult = false;
                },
                reloadBillingPeriod:function()
                {
                    if (!$scope.scopeModel.isEditMode)
                    {
                        var partnerObject = partnerSelectorAPI.getData();
                        if (partnerObject != undefined && partnerObject.selectedIds != undefined) {
                            $scope.scopeModel.isLoading = true;
                            VR_Invoice_InvoiceAPIService.GetBillingInterval(invoiceTypeId, partnerObject.selectedIds, $scope.scopeModel.issueDate).then(function (response) {
                                $scope.scopeModel.isLoading = false;
                                if (response) {
                                    $scope.scopeModel.isLoading = true;
                                    VR_Invoice_InvoiceAPIService.CheckInvoiceFollowBillingPeriod(invoiceTypeId, partnerObject.selectedIds).then(function (response) {
                                        if (response) {
                                            $scope.scopeModel.isDateDisabled = true;
                                        }else
                                        {
                                            $scope.scopeModel.isDateDisabled = false;
                                        }
                                        $scope.scopeModel.isLoading = false;
                                    }).catch(function (error) {
                                        $scope.scopeModel.isLoading = false;
                                    });
                                    $scope.scopeModel.fromDate = response.FromDate;
                                    $scope.scopeModel.toDate = response.ToDate;
                                } else {
                                    $scope.scopeModel.fromDate = undefined;
                                    $scope.scopeModel.toDate = undefined;
                                }
                            }).catch(function (error) {
                                $scope.scopeModel.isLoading = false;
                            });;
                        }
                          
                    }
                }
            };
            return context;
        }
    }

    appControllers.controller('VR_Invoice_GenericInvoiceEditorController', genericInvoiceEditorController);
})(appControllers);
