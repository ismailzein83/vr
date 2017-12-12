(function (app) {

    "use strict";

    VRLocalizationTextResourceTranslationManagementController.$inject = ['$scope', 'VRCommon_VRLocalizationTextResourceAPIService', 'VRCommon_VRLocalizationTextResourceService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRCommon_VRLocalizationTextResourceTranslationService'];

    function VRLocalizationTextResourceTranslationManagementController($scope, VRCommon_VRLocalizationTextResourceAPIService, VRCommon_VRLocalizationTextResourceService, UtilsService, VRUIUtilsService, VRNotificationService, VRCommon_VRLocalizationTextResourceTranslationService) {

        var gridAPI;

        var textResourceId;

        var textResourceSelectorAPI;
        var textResourceSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var languageSelectorAPI;
        var languageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();

        load();

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load(getGridQuery());
            };
            $scope.scopeModel.addTextResourceTranslation = function () {
                var onVRLocalizationTextResourceTranslationAdded = function (addedItem) {
                    gridAPI.onVRLocalizationTextResourceTranslationAdded(addedItem);
                };
                VRCommon_VRLocalizationTextResourceTranslationService.addVRLocalizationTextResourceTranslation(onVRLocalizationTextResourceTranslationAdded);
            };
            $scope.scopeModel.onTextResourceSelectorReady = function (api) {
                textResourceSelectorAPI = api;
                textResourceSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onLanguageSelectorReady = function (api) {
                languageSelectorAPI = api;
                languageSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.search = function () {
                gridAPI.load(getGridQuery());
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            function loadTextResourceSelector() {
                var textResourceSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                textResourceSelectorReadyDeferred.promise.then(function () {
                    var payload;
                    VRUIUtilsService.callDirectiveLoad(textResourceSelectorAPI, payload, textResourceSelectorLoadDeferred);
                });
                return textResourceSelectorLoadDeferred.promise;
            }

            function loadLanguageSelector() {
                var languageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                languageSelectorReadyDeferred.promise.then(function () {
                    var payload;
                    VRUIUtilsService.callDirectiveLoad(languageSelectorAPI, payload, languageSelectorLoadDeferred);
                });
                return languageSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadLanguageSelector, loadTextResourceSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function getGridQuery() {
            var gridPayload= {
                 query : {
                    ResourceIds: textResourceSelectorAPI.getSelectedIds(),
                    LanguageIds: languageSelectorAPI.getSelectedIds(),
                 },
                 textResourceId: textResourceId,
            };
            return gridPayload;
        }

    }
    app.controller('VRCommon_VRLocalizationTextResourceTranslationManagementController', VRLocalizationTextResourceTranslationManagementController);

})(app);