(function (appControllers) {

    "use strict";

    genericInvoiceEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService', 'VR_Invoice_InvoiceAPIService', 'VRButtonTypeEnum', 'VR_Invoice_InvoiceActionService'];

    function genericInvoiceEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceAPIService, VRButtonTypeEnum, VR_Invoice_InvoiceActionService) {
        var invoiceTypeId;
        $scope.scopeModel = {};

        $scope.scopeModel.invoiceTypeEntity;
        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var invoiceGeneratorActions;
        var validateResult = false;
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }
        function defineScope() {
            $scope.scopeModel.actions = [];
            $scope.scopeModel.issueDate = new Date();
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
                return generateInvoice();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.validateForm = function ()
            {
                var partnerObject;
                if (partnerSelectorAPI != undefined)
                    partnerObject = partnerSelectorAPI.getData();
                if ($scope.scopeModel.issueDate != undefined && partnerObject != undefined && partnerObject.selectedIds != undefined && $scope.scopeModel.fromDate != undefined && $scope.scopeModel.toDate != undefined)
                {
                    buildInvoiceGeneratorActions();
                    validateResult = true;
                    return null;
                }
                validateResult = false;
                return null;
            }
            function generateInvoice() {
                $scope.scopeModel.isLoading = true;

                var incvoiceObject = buildInvoiceObjFromScope();
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
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            getInvoiceType().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });

            function getInvoiceType() {
                return VR_Invoice_InvoiceTypeAPIService.GetGeneratorInvoiceTypeRuntime(invoiceTypeId).then(function (response) {
                    $scope.scopeModel.invoiceTypeEntity = response;
                });
            }

        }
        function loadAllControls() {

            function setTitle() {
                $scope.title = "Generate Invoice";
            }
            function loadPartnerSelectorDirective() {
                var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                partnerSelectorReadyDeferred.promise.then(function () {
                    var partnerSelectorPayload = { context: getContext() };
                    VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
                });
                return partnerSelectorPayloadLoadDeferred.promise;
            }
            function loadStaticData() {
            }
           
            
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPartnerSelectorDirective])
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
                InvoiceTypeId: invoiceTypeId,
                PartnerId: partnerObject != undefined ? partnerObject.selectedIds:undefined,
                FromDate: $scope.scopeModel.fromDate,
                ToDate: $scope.scopeModel.toDate,
                IssueDate: $scope.scopeModel.issueDate
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
                }
            };
            return context;
        }
    }

    appControllers.controller('VR_Invoice_GenericInvoiceEditorController', genericInvoiceEditorController);
})(appControllers);
