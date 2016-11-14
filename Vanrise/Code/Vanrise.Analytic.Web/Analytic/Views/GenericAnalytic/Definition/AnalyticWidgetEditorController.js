(function (appControllers) {

    "use strict";

    AnalyticWidgetEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function AnalyticWidgetEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var widgetSelectiveAPI;
        var widgetSelectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var widgetEntity;
        var tableIds;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                widgetEntity = parameters.widgetEntity;
                tableIds = parameters.tableIds;
            }
            isEditMode = (widgetEntity != undefined);
        }

        function defineScope() {

            $scope.onWidgetSelectiveDirectiveReady = function (api) {
                widgetSelectiveAPI = api;
                widgetSelectiveReadyDeferred.resolve();
            };


            $scope.SaveWidget = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            //$scope.scopeModal.hasSaveGenericBEEditor = function () {
            //    if (isEditMode) {
            //        return VR_GenericData_BusinessEntityDefinitionAPIService.HasUpdateBusinessEntityDefinition();
            //    }
            //    else {
            //        return VR_GenericData_BusinessEntityDefinitionAPIService.HasAddBusinessEntityDefinition();
            //    }
            //}
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadWidgetSelective]).then(function () {

                }).finally(function () {
                    $scope.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


                function setTitle() {
                    if (isEditMode && widgetEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(widgetEntity.WidgetTitle, 'Widget Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Widget Editor');
                }


                function loadWidgetSelective() {
                    var loadWidgetSelectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    widgetSelectiveReadyDeferred.promise.then(function () {
                        var payLoad = {
                            tableIds: tableIds,
                            widgetEntity: widgetEntity
                        };
                        VRUIUtilsService.callDirectiveLoad(widgetSelectiveAPI, payLoad, loadWidgetSelectivePromiseDeferred);
                    });
                    return loadWidgetSelectivePromiseDeferred.promise;
                }
            }

        }


        function buildWidgetObjectFromScope() {
            var widgetObject = widgetSelectiveAPI != undefined ? widgetSelectiveAPI.getData() : undefined;
            return widgetObject;
        }


        function insert() {
            var widgetObj = buildWidgetObjectFromScope();
            if ($scope.onWidgetAdded != undefined) {
                $scope.onWidgetAdded(widgetObj);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var widgetObj = buildWidgetObjectFromScope();
            if ($scope.onWidgetUpdated != undefined) {
                $scope.onWidgetUpdated(widgetObj);
            }
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VR_Analytic_AnalyticWidgetEditorController', AnalyticWidgetEditorController);
})(appControllers);
