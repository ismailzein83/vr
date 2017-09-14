(function (appControllers) {

    "use strict";

    TelesSiteEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','Retail_Teles_SiteAPIService'];

    function TelesSiteEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_Teles_SiteAPIService) {
        var enterpriseId;
        var vrConnectionId
        loadParameters();
        defineScope();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                enterpriseId = parameters.enterpriseId;
                vrConnectionId= parameters.vrConnectionId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.useTemplate = false;
            $scope.scopeModel.save = function () {
                return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
       
        function loadAllControls() {
            function setTitle() {
                $scope.title = 'Add Teles Site';
            }
            function loadStaticData() {
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

           
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return Retail_Teles_SiteAPIService.AddTelesSite(buildSiteObjFromScope()).then(function (response) {
                if (response) {
                    if ($scope.onSiteAdded != undefined) {
                        $scope.onSiteAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSiteObjFromScope() {
            return {
                CentrexFeatSet: $scope.scopeModel.centrexFeatSet,
                TemplateName: $scope.scopeModel.templateName,
                TelesEnterpriseId:enterpriseId,
                VRConnectionId:vrConnectionId,
                Site: {
                    name: $scope.scopeModel.name,
                    description: $scope.scopeModel.description,
                    maxCalls: $scope.scopeModel.siteMaxCalls,
                    maxCallsPerUser: $scope.scopeModel.siteMaxCallsPerUser,
                    maxRegistrations: $scope.scopeModel.siteMaxRegistrations,
                    maxRegsPerUser: $scope.scopeModel.siteMaxRegsPerUser,
                    maxSubsPerUser: $scope.scopeModel.siteMaxSubsPerUser,
                    maxBusinessTrunkCalls: $scope.scopeModel.siteMaxBusinessTrunkCalls,
                    maxUsers: $scope.scopeModel.siteMaxUsers,
                }
            };
        }
    }

    appControllers.controller('Retail_Teles_TelesSiteEditorController', TelesSiteEditorController);

})(appControllers);