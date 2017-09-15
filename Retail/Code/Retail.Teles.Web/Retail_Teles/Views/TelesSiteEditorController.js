(function (appControllers) {

    "use strict";

    TelesSiteEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','Retail_Teles_SiteAPIService','Retail_Teles_SiteService'];

    function TelesSiteEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_Teles_SiteAPIService, Retail_Teles_SiteService) {
        var enterpriseId;
        var vrConnectionId;

        var telesTemplateSelectorApi;
        var telesTemplateSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.useTemplate = true;
            $scope.scopeModel.onTelesTemplateSelectorReady = function (api) {
                telesTemplateSelectorApi = api;
                var payload = {
                    filter: {
                        BusinessEntityDefinitionId: Retail_Teles_SiteService.getTelesTemplateBEDefinitionId()
                    }
                };
                var setLoader = function (value) {
                    $scope.scopeModel.isTelesTemplateLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, telesTemplateSelectorApi, payload, setLoader, telesTemplateSelectorPromiseDeferred);
            };
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTelesTemplateSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

           
        }
        function loadTelesTemplateSelector() {
            if ($scope.scopeModel.useTemplate)
            {
                var telesTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                telesTemplateSelectorPromiseDeferred.promise.then(function () {
                    telesTemplateSelectorPromiseDeferred = undefined;
                    var selectorPayload = {
                        filter: {
                            BusinessEntityDefinitionId: Retail_Teles_SiteService.getTelesTemplateBEDefinitionId()
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(telesTemplateSelectorApi, selectorPayload, telesTemplateSelectorLoadDeferred);
                });
                return telesTemplateSelectorLoadDeferred.promise;
            }else
            {
                telesTemplateSelectorPromiseDeferred = undefined;
            }
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
                TemplateName: $scope.scopeModel.selectedTelesTemplate != undefined?$scope.scopeModel.selectedTelesTemplate.Name:undefined,
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