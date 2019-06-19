(function (appControllers) {

    'use strict';

    NoteEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function NoteEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var zoneItem;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {

                zoneItem = parameters.zoneItem;
            }
        }
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.saveNote = function () {
                $scope.onNoteAdded($scope.scopeModel.note);
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadStaticData() {
            if (zoneItem != undefined )//&& zoneItem.NewRates != undefined) {
                //var normalRate = UtilsService.getItemByVal(zoneItem.NewRates, null, "RateTypeId");
                //if (normalRate != undefined)
                    //$scope.scopeModel.note = normalRate.Note;
                $scope.scopeModel.note = zoneItem.Note;
            //}
        }
        function setTitle() {

            if (zoneItem != undefined)
                $scope.title = zoneItem.ZoneName + ' Note';
        }

    }

    appControllers.controller('WhS_Sales_NoteEditorController', NoteEditorController);

})(appControllers);