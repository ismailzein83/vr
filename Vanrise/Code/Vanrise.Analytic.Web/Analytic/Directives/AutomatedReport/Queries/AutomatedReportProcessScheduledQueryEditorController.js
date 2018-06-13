(function (appControllers) {
    "use strict";
    automatedReportProcessScheduledQueryEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRComponentTypeAPIService'];
    function automatedReportProcessScheduledQueryEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_VRComponentTypeAPIService) {


        var isEditMode;
        var automatedReportEntity;
        var runtimeEditorEntity;
        var runtimeDirectiveEntity;

        var automatedReportProcessScheduledSelectorAPI;
        var automatedReportProcessScheduledReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var automatedReportProcessScheduledSelectedPromiseDeferred;

        var runtimeDirectiveAPI;
        var runtimeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                automatedReportEntity = parameters.Entity;
                context = parameters.context;
                if (parameters.Entity != undefined) {
                    runtimeDirectiveEntity = parameters.Entity.Settings;
                }
            }
            isEditMode = (automatedReportEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onRuntimeDirectiveReady = function (api) {
                runtimeDirectiveAPI = api;
                runtimeDirectiveReadyDeferred.resolve();

            };
            $scope.saveQuery= function () {
                if (isEditMode)
                    return updateQuery();
                else
                    return insertQuery();
            };

            $scope.scopeModel.onAutomatedReportProcessScheduledSelectorReady = function (api) {
                automatedReportProcessScheduledSelectorAPI = api;
                automatedReportProcessScheduledReadyPromiseDeferred.resolve();
               };

            $scope.scopeModel.onAutomatedReportProcessScheduledSelectionChanged = function (value) {
                if (value != undefined) {
                    if (automatedReportProcessScheduledSelectedPromiseDeferred != undefined) {
                        automatedReportProcessScheduledSelectedPromiseDeferred.resolve();
                    }
                    else {
                        runtimeDirectiveReadyDeferred.promise.then(function () {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoading = value;
                            };
                            var runtimeDirectivePayload = {
                                definitionId: automatedReportProcessScheduledSelectorAPI.getSelectedIds(),
                                context: getContext()
                            };

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, runtimeDirectiveAPI, runtimeDirectivePayload, setLoader);
                        });
                    }

                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                automatedReportProcessScheduledSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                loadAllControls();
            } else {
                loadAllControls();
            }
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && automatedReportEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(automatedReportEntity.QueryTitle, "Automated Report Query");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Automated Report Query");
            }

            function loadStaticData() {

                if (automatedReportEntity == undefined)
                    return;
                $scope.scopeModel.Name = automatedReportEntity.QueryTitle;
            }

            function loadAutomatedReportProcessScheduledSelector() {
                var automatedReportProcessScheduledSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                automatedReportProcessScheduledReadyPromiseDeferred.promise.then(function () {
                    var automatedReportProcessSelectorPayload = {
                        selectedIds: automatedReportEntity != undefined ? automatedReportEntity.DefinitionId : undefined
                    };

                    VRUIUtilsService.callDirectiveLoad(automatedReportProcessScheduledSelectorAPI, automatedReportProcessSelectorPayload, automatedReportProcessScheduledSelectorLoadDeferred);
                });
                return automatedReportProcessScheduledSelectorLoadDeferred.promise;
            }

            function loadRuntimeDirective() {
                if (isEditMode) {
                    var promises = [];

                    var runtimeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    promises.push(runtimeDirectiveReadyDeferred.promise);

                    promises.push(automatedReportProcessScheduledSelectedPromiseDeferred.promise);

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        automatedReportProcessScheduledSelectedPromiseDeferred = undefined;
                        var runtimeDirectivePayload = {
                            definitionId: automatedReportProcessScheduledSelectorAPI.getSelectedIds(),
                            runtimeDirectiveEntity: runtimeDirectiveEntity,
                            context: getContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(runtimeDirectiveAPI, runtimeDirectivePayload, runtimeDirectiveLoadDeferred);
                    });
                    return runtimeDirectiveLoadDeferred.promise;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRuntimeDirective, loadAutomatedReportProcessScheduledSelector]) 
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function buildObjFromScope() {
            var obj = {
                DefinitionId: automatedReportProcessScheduledSelectorAPI.getSelectedIds(),
                QueryTitle: $scope.scopeModel.Name,
                Settings: runtimeDirectiveAPI.getData(),
            };
            return obj;
        }

        function insertQuery() {
            var object = buildObjFromScope();
            object.VRAutomatedReportQueryId = UtilsService.guid();
            if ($scope.onQueryAdded != undefined && typeof ($scope.onQueryAdded) == 'function')
                $scope.onQueryAdded(object);
            $scope.modalContext.closeModal();
        }

        function updateQuery() {
            var object = buildObjFromScope();
            object.VRAutomatedReportQueryId = automatedReportEntity.VRAutomatedReportQueryId;
            if ($scope.onQueryUpdated != undefined && typeof ($scope.onQueryUpdated) == 'function')
                $scope.onQueryUpdated(object);
            $scope.modalContext.closeModal();
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }
    appControllers.controller('VRAnalytic_AutomatedReportProcessScheduledQueryEditorController', automatedReportProcessScheduledQueryEditorController);
})(appControllers);