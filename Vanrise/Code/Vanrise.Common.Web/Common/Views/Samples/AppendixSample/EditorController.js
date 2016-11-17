(function (appControllers) {

    "use strict";

    EditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'Common_AppendixSample_Service', 'VRUIUtilsService'];

    function EditorController($scope, VRNavigationService, UtilsService, VRNotificationService, Common_AppendixSample_Service, VRUIUtilsService) {


        var isEditMode;
        var appendixAPI;
        var appendix2API;
        var dynamicAppendixAPI;
        var appendixListAPI;

        var entity;
        var appendixReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var appendix2ReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dynamicAppendixReadyPromiseDeferred;

        var appendixListReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                isEditMode = parameters.edit;
            }

        }

        function defineScope() {
            $scope.testModel = 'test value 1';
            $scope.title = 'Appendix Sample Editor: ' + (isEditMode ? 'Edit' : 'New');
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.select1Source = [];

            $scope.appendixReady = function (api) {
                appendixAPI = api;
                appendixReadyPromiseDeferred.resolve();
            };

            $scope.appendix2Ready = function (api) {
                appendix2API = api;
                appendix2ReadyPromiseDeferred.resolve();
            };

            $scope.dynamicAppendixReady = function (api) {
                dynamicAppendixAPI = api;
                var setLoader = function (value) { $scope.isLoadingDynamicAppendix = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dynamicAppendixAPI, undefined, setLoader, dynamicAppendixReadyPromiseDeferred);
            };

            $scope.dynamicAppendixTemplates = [];


            $scope.dynamicAppendixTemplatesToAdd = [];
            $scope.dynamicAppendixList = [];

            $scope.addDynamicAppendixToList = function () {
                var dynamicAppendixItem = {
                    editor: $scope.selectedDynamicAppendixToAdd.editor
                };

                dynamicAppendixItem.dynamicAppendixReady = function (api) {
                    dynamicAppendixItem.dynamicAppendixAPI = api;
                    var setLoader = function (value) { dynamicAppendixItem.isLoading = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dynamicAppendixItem.dynamicAppendixAPI, undefined, setLoader);
                };

                $scope.dynamicAppendixList.push(dynamicAppendixItem);
                $scope.selectedDynamicAppendixToAdd = undefined;
            };

            $scope.appendixListReady = function (api) {
                appendixListAPI = api;
                appendixListReadyPromiseDeferred.resolve();
            };
        }

        function load() {

            $scope.isLoading = true;
            if (isEditMode) {
                getEntity().then(function () {
                    loadAllControls()
                        .finally(function () {
                            entity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getEntity() {
            return Common_AppendixSample_Service.getRemoteData(1000).then(function () {
                entity = {

                };
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSelect1, loadAppendix, loadAppendix2, loadDynamicAppendixSection, loadDynamicAppendixListSection, loadAppendixList])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }

        function loadSelect1() {
            return Common_AppendixSample_Service.getRemoteData(2000)
                .then(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $scope.select1Source.push(data[i]);
                    }
                    if (entity != undefined)
                        $scope.selectedItem1 = $scope.select1Source[0];
                });
        }

        function loadAppendix() {     

            var loadAppendixPromiseDeferred = UtilsService.createPromiseDeferred();

            appendixReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;
                    if (entity != undefined)
                        directivePayload = undefined;
                    VRUIUtilsService.callDirectiveLoad(appendixAPI, directivePayload, loadAppendixPromiseDeferred);
                });

            return loadAppendixPromiseDeferred.promise;
        }

        function loadAppendix2() {
            
            var loadAppendix2PromiseDeferred = UtilsService.createPromiseDeferred();

            appendix2ReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;
                    if (entity != undefined)
                        directivePayload = undefined;
                    VRUIUtilsService.callDirectiveLoad(appendix2API, directivePayload, loadAppendix2PromiseDeferred);
                });

            return loadAppendix2PromiseDeferred.promise;
        }

        function loadDynamicAppendixSection() {
            var promises = [];
            var dynamicAppendixPayload;
            var selectedDefaultItem;
            if (entity != undefined)
                dynamicAppendixPayload = undefined;
            else
                selectedDefaultItem = {};
            var loadTemplatesPromise = Common_AppendixSample_Service.getRemoteData(1000)
                .then(function () {
                    $scope.dynamicAppendixTemplates.push({
                        name: "Appendix 1",
                        editor: "vr-common-appendixsample-appendix"
                    });
                    $scope.dynamicAppendixTemplates.push({
                        name: "Appendix 2",
                        editor: "vr-common-appendixsample-appendix2"
                    });
                    if (dynamicAppendixPayload != undefined)
                        $scope.selectedDynamicAppendix = $scope.dynamicAppendixTemplates[0];
                    else if(selectedDefaultItem != undefined)
                        $scope.selectedDynamicAppendix = $scope.dynamicAppendixTemplates[1];
                });

            promises.push(loadTemplatesPromise);
            if (dynamicAppendixPayload != undefined) {
                dynamicAppendixReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var dynamicAppendixLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(dynamicAppendixLoadPromiseDeferred.promise);

                dynamicAppendixReadyPromiseDeferred.promise.then(function () {
                    dynamicAppendixReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(dynamicAppendixAPI, dynamicAppendixPayload, dynamicAppendixLoadPromiseDeferred);
                });
            }
            return UtilsService.waitMultiplePromises(promises);
        }

        function loadDynamicAppendixListSection() {
            var promises = [];
           
            var loadTemplatesPromise = Common_AppendixSample_Service.getRemoteData(1000)
                .then(function () {
                    $scope.dynamicAppendixTemplatesToAdd.push({
                        name: "Appendix 1",
                        editor: "vr-common-appendixsample-appendix"
                    });
                    $scope.dynamicAppendixTemplatesToAdd.push({
                        name: "Appendix 2",
                        editor: "vr-common-appendixsample-appendix2"
                    });
                });
            promises.push(loadTemplatesPromise);

            if (entity != undefined) {

               var directivePayload = [
                       { name: "Appendix 1" },
                       { name: "Appendix 2" },
                       { name: "Appendix 2" },
                       { name: "Appendix 1" },
                { name: "Appendix 2" }];
                var appendixItems = [];
                for (var i = 0; i < directivePayload.length; i++) {
                    var appendixItem = {
                        payload: directivePayload[i],
                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                    };
                    promises.push(appendixItem.loadPromiseDeferred.promise);
                    appendixItems.push(appendixItem);
                }
                loadTemplatesPromise.then(function () {
                    for (var i = 0; i < appendixItems.length; i++) {
                        loadDynamicAppendixItem(appendixItems[i]);
                    }
                });
            }

            function loadDynamicAppendixItem(appendixItem) {
                var matchItem = UtilsService.getItemByVal($scope.dynamicAppendixTemplatesToAdd, appendixItem.payload.name, "name");
                if (matchItem == null)
                    return;

                var dynamicAppendixItem = {
                    editor: matchItem.editor
                };
                
                dynamicAppendixItem.dynamicAppendixReady = function (api) {
                    dynamicAppendixItem.dynamicAppendixAPI = api;
                    appendixItem.readyPromiseDeferred.resolve();
                };

                appendixItem.readyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dynamicAppendixItem.dynamicAppendixAPI, undefined, appendixItem.loadPromiseDeferred);
                    });

                $scope.dynamicAppendixList.push(dynamicAppendixItem);
            }
            return UtilsService.waitMultiplePromises(promises);
        }

        function loadAppendixList() {

            var loadAppendixListPromiseDeferred = UtilsService.createPromiseDeferred();

            appendixListReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;
                    if (entity != undefined) {
                        directivePayload = [
                        { name: "Appendix 1" },
                        { name: "Appendix 2" },
                        { name: "Appendix 1" }];
                    }
                    VRUIUtilsService.callDirectiveLoad(appendixListAPI, directivePayload, loadAppendixListPromiseDeferred);
                });

            return loadAppendixListPromiseDeferred.promise;
        }
    }

    appControllers.controller('Common_AppendixSample_EditorController', EditorController);
})(appControllers);