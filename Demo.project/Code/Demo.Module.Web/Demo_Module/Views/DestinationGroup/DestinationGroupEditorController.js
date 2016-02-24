(function (appControllers) {

    "use strict";

    destinationGroupEditorController.$inject = ['$scope', 'Demo_DestinationGroupAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService'];

    function destinationGroupEditorController($scope, Demo_DestinationGroupAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService) {
        var isEditMode;
        var destinationGroupId;
        var destinationGroupEntity;

        var sourceTemplateDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                destinationGroupId = parameters.DestinationGroupId;
            }

            isEditMode = (destinationGroupId != undefined);

        }

        function defineScope() {

            $scope.scopeModal = {
            };

            $scope.groupTypeTemplates = [];
            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTemplateDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTemplateDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
            }


            $scope.SaveDestinationGroup = function () {
                if (isEditMode) {
                    return updateDestinationGroup();
                }
                else {
                    return insertDestinationGroup();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getDestinationGroup().then(function () {
                    loadAllControls()
                        .finally(function () {
                            destinationGroupEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getDestinationGroup() {
            return Demo_DestinationGroupAPIService.GetDestinationGroup(destinationGroupId).then(function (destinationGroup) {
                destinationGroupEntity = destinationGroup;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadGroupSubTypes])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadGroupSubTypes() {
            var promises = [];
            var sourceConfigId;
            if (destinationGroupEntity != undefined && destinationGroupEntity.GroupSubTypeSettings) {
                sourceConfigId = destinationGroupEntity.GroupSubTypeSettings.ConfigId;
            }

            var loadGroupSubTypePromise = Demo_DestinationGroupAPIService.GetGroupTypeTemplates().then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.groupTypeTemplates.push(item);
                });

                if (sourceConfigId != undefined)
                    $scope.selectedGroupTypeTemplate = UtilsService.getItemByVal($scope.groupTypeTemplates, sourceConfigId, "TemplateConfigID");

            });
            promises.push(loadGroupSubTypePromise);

            if (sourceConfigId != undefined) {
                sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                    var serviceSubTypePayload;

                    if (destinationGroupEntity != undefined && destinationGroupEntity.GroupSubTypeSettings) {
                        serviceSubTypePayload = {
                            selectedIds: [destinationGroupEntity.GroupSubTypeSettings.SelectedId]
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(sourceTemplateDirectiveAPI, serviceSubTypePayload, loadSourceTemplatePromiseDeferred);
                });

                promises.push(loadSourceTemplatePromiseDeferred.promise);
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(destinationGroupEntity ? '' : null, 'Destination Group') : UtilsService.buildTitleForAddEditor('Destination Group');
        }

        function loadStaticSection() {
            if (destinationGroupEntity != undefined) {
                //$scope.scopeModal.toDate = destinationGroupEntity.ToDate;
            }
        }

        function buildDestinationGroupObjFromScope() {
            var obj = {
                DestinationGroupId: (destinationGroupId != null) ? destinationGroupId : 0,
               // ToDate: $scope.scopeModal.toDate,
                GroupSubTypeSettings: { $type: "Demo.Module.MainExtension.GroupSubTypeVoice, Demo.Module.MainExtension", SelectedId: sourceTemplateDirectiveAPI.getSelectedIds(), ConfigId: $scope.selectedGroupTypeTemplate.TemplateConfigID }
            };
            console.log(obj)
            return obj;
        }

        function insertDestinationGroup() {
            $scope.isLoading = true;

            var destinationGroupObject = buildDestinationGroupObjFromScope();

            return Demo_DestinationGroupAPIService.AddDestinationGroup(destinationGroupObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Destination Group", response, undefined)) {
                    if ($scope.onDestinationGroupAdded != undefined)
                        $scope.onDestinationGroupAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateDestinationGroup() {
            $scope.isLoading = true;

            var destinationGroupObject = buildDestinationGroupObjFromScope();

            Demo_DestinationGroupAPIService.UpdateDestinationGroup(destinationGroupObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Destination Group", response, undefined)) {
                    if ($scope.onDestinationGroupUpdated != undefined)
                        $scope.onDestinationGroupUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('Demo_DestinationGroupEditorController', destinationGroupEditorController);
})(appControllers);
