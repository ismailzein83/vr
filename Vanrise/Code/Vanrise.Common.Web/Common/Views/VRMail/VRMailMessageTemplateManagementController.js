(function (appControllers) {

    "use strict";

    MailMessageTemplateManagementController.$inject = ['$scope', 'VRCommon_VRMailMessageTemplateAPIService', 'VRCommon_VRMailMessageTemplateService', 'UtilsService', 'VRUIUtilsService'];

    function MailMessageTemplateManagementController($scope, VRCommon_VRMailMessageTemplateAPIService, VRCommon_VRMailMessageTemplateService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        var mailMessageTypeSelectorAPI;
        var mailMessageTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                mailMessageTypeSelectorAPI = api;
                mailMessageTypeSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.scopeModel.add = function () {
                var onMailMessageTemplateAdded = function (addedMailMessageTemplate) {
                    gridAPI.onMailMessageTemplateAdded(addedMailMessageTemplate);
                };
                VRCommon_VRMailMessageTemplateService.addMailMessageTemplate(onMailMessageTemplateAdded);
            };

            $scope.scopeModel.hasAddMailMessageTemplatePermission = function () {
                return VRCommon_VRMailMessageTemplateAPIService.HasAddMailMessageTemplatePermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadMailMessageTypeSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function loadMailMessageTypeSelector() {
            var loadMailMessageTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            mailMessageTypeSelectorPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(mailMessageTypeSelectorAPI, undefined, loadMailMessageTypeSelectorPromiseDeferred);
            });
            return loadMailMessageTypeSelectorPromiseDeferred.promise;
        }
        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                MailMessageType: mailMessageTypeSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('VRCommon_VRMailMessageTemplateManagementController', MailMessageTemplateManagementController);

})(appControllers);