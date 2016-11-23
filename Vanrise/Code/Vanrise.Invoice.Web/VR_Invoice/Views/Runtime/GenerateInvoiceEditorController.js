(function (appControllers) {

    "use strict";

    genericInvoiceEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService', 'VR_Invoice_InvoiceAPIService', 'VRButtonTypeEnum', 'VR_Invoice_InvoiceActionService'];

    function genericInvoiceEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceAPIService, VRButtonTypeEnum, VR_Invoice_InvoiceActionService) {
        var invoiceTypeId;
        $scope.invoiceTypeEntity;
        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();
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

            $scope.actions = [];
            $scope.issueDate = new Date();
            $scope.onPartnerSelectorReady = function (api) {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
            }
            $scope.validateToDate = function()
            {
                var date = new Date();
                if ($scope.toDate >= new Date(date.getFullYear(), date.getMonth(), date.getDate()))
                    return "Date must be less than date of today.";
                return null;
            }
            $scope.validateFromDate = function () {
                if ($scope.toDate < $scope.fromDate)
                    return "From date must be less than to date.";
                return null;
            }
            $scope.preview = function()
            {
                var partnerObject = partnerSelectorAPI.getData();

                var context = {
                    $type: "Vanrise.Invoice.Business.PreviewInvoiceActionContext,Vanrise.Invoice.Business",
                    InvoiceTypeId: invoiceTypeId,
                    PartnerId: partnerObject != undefined ? partnerObject.selectedIds : undefined,
                    FromDate:$scope.fromDate,
                    ToDate: $scope.toDate,
                    IssueDate: $scope.issueDate
                };

                var paramsurl = "";
                paramsurl += "invoiceActionContext=" + UtilsService.serializetoJson(context);
                paramsurl += "&actionTypeName=" + "OpenRDLCReportAction";
                window.open("Client/Modules/VR_Invoice/Reports/InvoiceReport.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1");
            }

            $scope.generateInvoice = function () {
                return generateInvoice();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;
            getInvoiceType().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPartnerSelectorDirective, buildInvoiceGeneratorActions])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function setTitle() {
            $scope.title = "Generate Invoice";
        }
        function getInvoiceType() {
            return VR_Invoice_InvoiceTypeAPIService.GetGeneratorInvoiceTypeRuntime(invoiceTypeId).then(function (response) {
                $scope.invoiceTypeEntity = response;
            });
        }
        function loadPartnerSelectorDirective() {
            var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            partnerSelectorReadyDeferred.promise.then(function () {
                var partnerSelectorPayload;
                VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
            });
            return partnerSelectorPayloadLoadDeferred.promise;
        }
        function loadStaticData() {
        }
        function buildInvoiceObjFromScope() {
            var partnerObject = partnerSelectorAPI.getData();

            var obj = {
                InvoiceTypeId: invoiceTypeId,
                PartnerId: partnerObject != undefined ? partnerObject.selectedIds:undefined,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate,
                IssueDate: $scope.issueDate
            };
            return obj;
        }
        function buildInvoiceGeneratorActions()
        {
            if($scope.invoiceTypeEntity != undefined)
            {
                if ($scope.invoiceTypeEntity.InvoiceType != undefined && $scope.invoiceTypeEntity.InvoiceType.Settings != undefined && $scope.invoiceTypeEntity.InvoiceType.Settings.InvoiceGeneratorActions != undefined)
                {
                    var dictionay = {};

                    for(var i=0;i<$scope.invoiceTypeEntity.InvoiceType.Settings.InvoiceGeneratorActions.length ;i++)
                    {
                        var invoiceGeneratorAction = $scope.invoiceTypeEntity.InvoiceType.Settings.InvoiceGeneratorActions[i];
                        var buttonType = UtilsService.getEnum(VRButtonTypeEnum, "value", invoiceGeneratorAction.ButtonType);
                        var invoiceAction = UtilsService.getItemByVal($scope.invoiceTypeEntity.InvoiceType.Settings.InvoiceActions, invoiceGeneratorAction.InvoiceGeneratorActionId, "InvoiceActionId");

                        if (dictionay[buttonType.value] == undefined)
                        {
                            dictionay[buttonType.value] = [];
                        }
                        dictionay[buttonType.value].push({
                            buttonType: buttonType,
                            invoiceAction: invoiceAction,
                            invoiceGeneratorAction: invoiceGeneratorAction
                        });
                    }
                    for(var prop in dictionay)
                    {

                        if(dictionay[prop].length > 1)
                        {
                            var menuActions = [];
                            var type;
                            for(var i=0;i<dictionay[prop].length;i++)
                            {
                                if (menuActions == undefined)
                                    menuActions = [];
                                var object = dictionay[prop][i];
                                type = object.buttonType != undefined ? object.buttonType.type : undefined;
                                addMenuAction(object.invoiceGeneratorAction, object.invoiceAction);
                            }
                            function addMenuAction(invoiceGeneratorAction, invoiceAction)
                            {
                                menuActions.push({
                                    name: invoiceGeneratorAction.Title,
                                    clicked: function () {
                                        return callActionMethod(invoiceAction);
                                    },
                                });
                            }
                            $scope.actions.push({ type: type, menuActions: menuActions });
                        }else
                        {
                            var object = dictionay[prop][0];
                            $scope.actions.push( {
                                type: object.buttonType != undefined ? object.buttonType.type : undefined,
                                onclick: function () {
                                    return callActionMethod(object.invoiceAction);
                                },
                            });
                        }
                    }
                }
            }
        }
        function callActionMethod(invoiceAction)
        {
            var partnerObject = partnerSelectorAPI.getData();
            var payload = {
                generatorEntity: {
                    invoiceTypeId: invoiceTypeId,
                    partnerId: partnerObject != undefined ? partnerObject.selectedIds : undefined,
                    fromDate: $scope.fromDate,
                    toDate: $scope.toDate,
                    issueDate: $scope.issueDate
                },
                invoiceAction: invoiceAction,
                isPreGenerateAction:true
            };
            var actionType = VR_Invoice_InvoiceActionService.getActionTypeIfExist(invoiceAction.Settings.ActionTypeName);

            var promise = actionType.actionMethod(payload);

            return promise;
        }
        function generateInvoice() {
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
               $scope.scopeModal.isLoading = false;
           });
        }
    }

    appControllers.controller('VR_Invoice_GenericInvoiceEditorController', genericInvoiceEditorController);
})(appControllers);
