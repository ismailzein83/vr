(function (appControllers) {

    "use strict";

    pageDirectiveHostManagementController.$inject = ['$scope', 'VRCommon_PageDirectiveHostAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function pageDirectiveHostManagementController($scope, VRCommon_PageDirectiveHostAPIService, UtilsService, VRUIUtilsService, VRNavigationService) {
        loadParameters();
        defineScope();
        load();
        var viewId;
        var directivePageApi;
        var directivePageReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onPageHostDirectiveReady = function (api) {
                directivePageApi = api;
                directivePageReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoading = true;         
            var rootPromiseNode = {
                promises: [GetPageDirectiveHostInfo()],
                getChildNode: function () {
                    return {
                        promises: [loadPageDirective()]
                    };
                }
            };

            return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                $scope.isLoading = false;
            });



            function GetPageDirectiveHostInfo() {
                 return VRCommon_PageDirectiveHostAPIService.GetPageDirectiveHostInfo(viewId).then(function (response) {
                    if (response) {
                        $scope.scopeModel.directive = response.Directive;
                    }
                })
            }

            function loadPageDirective() {
                var loadPageDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                directivePageReadyPromiseDeferred.promise.then(function () {
                    var payload = {};
                    VRUIUtilsService.callDirectiveLoad(directivePageApi, payload, loadPageDirectivePromiseDeferred);

                });
                return loadPageDirectivePromiseDeferred.promise;
            }
        }       
        
    }

    appControllers.controller('VRCommon_PageDirectiveHostManagementController', pageDirectiveHostManagementController);
})(appControllers);