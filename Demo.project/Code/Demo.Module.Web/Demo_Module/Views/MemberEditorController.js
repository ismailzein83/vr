(function (appControllers) {

    "use strict";
    memberEditorController.$inject = ['$scope', 'Demo_Module_MemberAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function memberEditorController($scope, Demo_Module_MemberAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var memberId;
        var memberEntity;

        var familyIdItem;

        var familyDirectiveApi;
        var familyReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                memberId = parameters.memberId;
                familyIdItem = parameters.familyIdItem;

            }
            isEditMode = (memberId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.disableFamily = familyIdItem != undefined;
            $scope.scopeModel.onFamilyDirectiveReady = function (api) {
                familyDirectiveApi = api;
                familyReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveMember = function () {
                if (isEditMode)
                    return updateMember();
                else
                    return insertMember();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getMember().then(function () {
                    loadAllControls().finally(function () {
                        memberEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getMember() {
            return Demo_Module_MemberAPIService.GetMemberById(memberId).then(function (response) {
                memberEntity = response;
            });
        };

        function loadAllControls() {

            function loadFamilySelector() {
                var familyLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                familyReadyPromiseDeferred.promise.then(function () {
                    var familyPayload = {};
                    if (familyIdItem != undefined)
                        familyPayload.selectedIds = familyIdItem.FamilyId;

                    if (memberEntity != undefined)
                        familyPayload.selectedIds = memberEntity.FamilyId;

                    VRUIUtilsService.callDirectiveLoad(familyDirectiveApi, familyPayload, familyLoadPromiseDeferred);
                });
                return familyLoadPromiseDeferred.promise;

            }

            function setTitle() {
                if (isEditMode && memberEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(memberEntity.Name, "Member");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Member");
            };

            function loadStaticData() {
                if (memberEntity != undefined)
                    $scope.scopeModel.name = memberEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFamilySelector])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildMemberObjectFromScope() {
            var object = {
                MemberId: (memberId != undefined) ? memberId : undefined,
                Name: $scope.scopeModel.name,
                FamilyId: familyDirectiveApi.getSelectedIds()
            };
            return object;
        };

        function insertMember() {

            $scope.scopeModel.isLoading = true;
            var memberObject = buildMemberObjectFromScope();
            return Demo_Module_MemberAPIService.AddMember(memberObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Member", response, "Name")) {
                    if ($scope.onMemberAdded != undefined) {
                        $scope.onMemberAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function updateMember() {
            $scope.scopeModel.isLoading = true;
            var memberObject = buildMemberObjectFromScope();
            Demo_Module_MemberAPIService.UpdateMember(memberObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Member", response, "Name")) {
                    if ($scope.onMemberUpdated != undefined) {
                        $scope.onMemberUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        };

    };
    appControllers.controller('Demo_Module_MemberEditorController', memberEditorController);
})(appControllers);