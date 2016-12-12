(function (appControllers) {

    "use strict";

    CodecDefEditorController.$inject = ['$scope' , 'VRNotificationService', 'UtilsService'];
    function CodecDefEditorController($scope , VRNotificationService, UtilsService) {

 
        var selectorDirectiveAPI;
        var codecDefEntity;

        var selectedCodecDefEntity;


        defineScope();
 
        loadAllControls();

        function defineScope() {
            $scope.scopeModel = {};


            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.add = function () {

                if ($scope.onCodecDefAdded != undefined) {
                    selectedCodecDefEntity = {
                        Description: codecDefEntity.Description,
                        CodecId: codecDefEntity.CodecId
                    };
                    $scope.onCodecDefAdded(selectedCodecDefEntity);
                }
                    $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onSelectorDirectiveReady = function (api) {
                selectorDirectiveAPI = api;
                selectorDirectiveAPI.load({});
            };

            $scope.scopeModel.onSelectionChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    codecDefEntity = SelectedItem;
                    $scope.PayloadSize = codecDefEntity.DefaultMsPerPacket;
                    $scope.SamplingFrequency = codecDefEntity.ClockRate;
                    $scope.PassThru = codecDefEntity.PassThru;
                }
            };
        };        
      
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {

                $scope.scopeModel.isLoading = false;
            });
            function setTitle() { 
                $scope.title = UtilsService.buildTitleForAddEditor('Codec Def');
            }             
        }       
    }

    appControllers.controller('NP_IVSwitch_CodecDefEditorController', CodecDefEditorController);

})(appControllers);