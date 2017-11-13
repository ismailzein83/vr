(function (appControllers) {

    "use strict";

    menualInvoiceBulkActionsRunTimeEditorController.$inject = ['$scope', 'VRNotificationService','VRUIUtilsService', 'VRNavigationService', 'UtilsService', 'VR_Invoice_InvoiceAPIService','VR_Invoice_InvoiceTypeAPIService','BusinessProcess_BPInstanceService'];

    function menualInvoiceBulkActionsRunTimeEditorController($scope, VRNotificationService, VRUIUtilsService, VRNavigationService, UtilsService, VR_Invoice_InvoiceAPIService, VR_Invoice_InvoiceTypeAPIService, BusinessProcess_BPInstanceService) {
        var invoiceTypeId;
        var menualInvoiceBulkActionsDefinitions;
        var selectedMenualInvoiceBulkAction;
        var context;
        var directiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
                context = parameters.context;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.menualBulkActions = [];

            $scope.scopeModel.execute = function () {
                var getTargetInvoicesEntity = context.getTargetInvoicesEntity();
                var executeMenualInvoiceActionsInput = {
                    InvoiceTypeId: invoiceTypeId,
                    InvoiceBulkActionIdentifier: context.getInvoiceBulkActionIdentifier(),
                    InvoiceBulkActions: [{
                        InvoiceBulkActionId: $scope.scopeModel.selectedMenualBulkAction.InvoiceBulkActionId,
                        Settings: directiveAPI.getData()
                    }],
                    IsAllInvoicesSelected: getTargetInvoicesEntity != undefined ? getTargetInvoicesEntity.IsAllInvoicesSelected : undefined,
                    TargetInvoicesIds: getTargetInvoicesEntity != undefined ? getTargetInvoicesEntity.TargetInvoicesIds : undefined,
                };
                return VR_Invoice_InvoiceAPIService.ExecuteMenualInvoiceActions(executeMenualInvoiceActionsInput).then(function (response) {
                    if (response.Succeed) {
                        $scope.modalContext.closeModal();
                        BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                    }
                    else {
                        VRNotificationService.showError(response.OutputMessage, $scope);
                    }
                });
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onMenualBulkActionSelectionChanged = function () {
                if($scope.scopeModel.selectedMenualBulkAction != undefined)
                {
                    if(menualInvoiceBulkActionsDefinitions != undefined)
                    {
                        for(var i=0;i<menualInvoiceBulkActionsDefinitions.length;i++)
                        {
                            var menualInvoiceBulkActionDefinition = menualInvoiceBulkActionsDefinitions[i];
                            if(menualInvoiceBulkActionDefinition.InvoiceBulkAction.InvoiceBulkActionId == $scope.scopeModel.selectedMenualBulkAction.InvoiceBulkActionId)
                            {
                                selectedMenualInvoiceBulkAction = menualInvoiceBulkActionDefinition;
                                $scope.scopeModel.runtimeEditor = menualInvoiceBulkActionDefinition.InvoiceBulkAction.Settings.RuntimeEditor;
                            }
                        }
                    }
                }else
                {
                    $scope.scopeModel.runtimeEditor = undefined;
                    selectedMenualInvoiceBulkAction = undefined;
                }
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var payload = {
                    invoiceTypeId: invoiceTypeId,
                    invoiceAttachments: selectedMenualInvoiceBulkAction.InvoiceAttachments,
                    emailActionSettings: selectedMenualInvoiceBulkAction.InvoiceBulkAction.Settings,
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
            };
          
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            getInvoiceBulkActionsDefinitions().then(function () {
                loadAllControls();
            });
        }
        function getInvoiceBulkActionsDefinitions()
        {
            return VR_Invoice_InvoiceTypeAPIService.GetMenualInvoiceBulkActionsDefinitions(invoiceTypeId).then(function (response) {
                menualInvoiceBulkActionsDefinitions = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = "Bulk Invoice Actions";
            }
            function loadStaticData() {
                //if (invoiceEntity != undefined) {
                //   // $scope.scopeModel.invoiceNote = invoiceEntity.Note;
                //}
            }

            function loadMenualInvoiceBulkActions()
            {
                if(menualInvoiceBulkActionsDefinitions != undefined)
                {
                    for (var i = 0; i < menualInvoiceBulkActionsDefinitions.length; i++)
                    {
                        var menualInvoiceBulkActionDefinition = menualInvoiceBulkActionsDefinitions[i];
                        $scope.scopeModel.menualBulkActions.push({
                            InvoiceBulkActionId: menualInvoiceBulkActionDefinition.InvoiceMenualBulkAction.InvoiceBulkActionId,
                            InvoiceMenualBulkActionId: menualInvoiceBulkActionDefinition.InvoiceMenualBulkAction.InvoiceMenualBulkActionId,
                            Title: menualInvoiceBulkActionDefinition.InvoiceMenualBulkAction.Title
                        });
                    }
                }
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadMenualInvoiceBulkActions])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }
    }

    appControllers.controller('VR_Invoice_MenualInvoiceBulkActionsRunTimeEditorController', menualInvoiceBulkActionsRunTimeEditorController);
})(appControllers);
