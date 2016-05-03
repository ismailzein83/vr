(function (appControllers) {

    "use strict";

    DataRecordStorageSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'Analytic_AnalyticService'];

    function DataRecordStorageSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, Analytic_AnalyticService) {

        var isEditMode;
        var viewTypeName = "VR_GenericData_RecordSearch";
        var recordTypeEntity;
        var viewId;

        var menuItems;

        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        var viewEntity;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
            isEditMode = (viewId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {}

            $scope.scopeModel.SaveGenericBEEditor = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.onDataRecordSourceReady = function (api) {
                api.loadGrid();
            }

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

            $scope.scopeModel.validateMenuLocation = function () {
                return ($scope.scopeModel.selectedMenuItem != undefined) ? null : 'No menu location selected';
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getView().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadTree]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

            function setTitle() {
                if (isEditMode && viewEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(viewEntity.Name, 'Record Search Editor');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Record Search Editor');
            }

            function loadStaticData() {
                if (viewEntity != undefined) {
                    $scope.scopeModel.reportName = viewEntity.Name;
                    $scope.scopeModel.reportTitle = viewEntity.Title;
                }
            }

            function loadTree() {
                var treeLoadDeferred = UtilsService.createPromiseDeferred();

                loadMenuItems().then(function () {
                    treeReadyDeferred.promise.then(function () {
                        if (viewEntity != undefined) {

                            $scope.scopeModel.selectedMenuItem = treeAPI.setSelectedNode(menuItems, viewEntity.ModuleId, "Id", "Childs");
                        }
                        treeAPI.refreshTree(menuItems);
                        treeLoadDeferred.resolve();
                    });
                }).catch(function (error) {
                    treeLoadDeferred.reject(error);
                });

                function loadMenuItems() {
                    return VR_Sec_MenuAPIService.GetAllMenuItems(false).then(function (response) {
                        if (response) {
                            menuItems = [];
                            for (var i = 0; i < response.length; i++) {
                                menuItems.push(response[i]);
                            }
                        }
                    });
                }

                return treeLoadDeferred.promise;
            }

            function getView() {
                return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
                    viewEntity = viewEntityObj;
                });
            }
        }

        function buildViewObjectFromScope() {

            var view = {
               
            };

            return view;
        }


        function insert() {
            
        }

        function update() {
           
        }

    }

    appControllers.controller('VR_GenericData_DataRecordStorageSettingsEditorController', DataRecordStorageSettingsEditorController);
})(appControllers);