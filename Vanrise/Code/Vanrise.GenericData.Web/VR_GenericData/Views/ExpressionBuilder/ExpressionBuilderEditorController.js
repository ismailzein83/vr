(function (appControllers) {

    "use strict";

    ExpressionBuilderEditorController.$inject = ['$scope', 'VR_GenericData_ExpressionBuilderConfigAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ExpressionBuilderEditorController($scope, VR_GenericData_ExpressionBuilderConfigAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var expressionBuilderValue;
        var context;
        var expressionBuilderDirectiveAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                expressionBuilderValue = parameters.ExpressionBuilderValue;
                context = parameters.Context;
            }
        }

        function defineScope() {

            $scope.expressionBuilderTemplates = [];

            $scope.onExpressionBuilderTemplateDirectiveReady = function (api) {
                expressionBuilderDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.isLoadingDirective = value;
                    UtilsService.safeApply($scope);
                };
                var payload = { context: context };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, expressionBuilderDirectiveAPI, payload, setLoader);
            };
            $scope.isbuttonDisabled = function () {
                if (expressionBuilderDirectiveAPI != undefined)
                    return expressionBuilderDirectiveAPI.getData() == undefined;
                return true;
            };
            $scope.addData = function () {

                var obj = expressionBuilderDirectiveAPI.getData();
                if ($scope.expressionValue != undefined)
                    $scope.expressionValue += obj;
                else
                    $scope.expressionValue = obj;
            };
         
            $scope.saveExpressionBuilder = function () {
                return setExpressionBuilder();
            };
             
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.validateExpression = function () {
                if ($scope.expressionValue == expressionBuilderValue)
                    return 'No changed.';
                return null;
            };
        }

        function load() {
            $scope.isLoading = true;
                loadAllControls();

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadTextAreaSection, setTitle, loadExpressionBuilderTemplates]).then(function () {
                })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                   .finally(function () {
                       $scope.isLoading = false;
                   });


                    function setTitle() {
                         $scope.title = UtilsService.buildTitleForUpdateEditor('Expression Builder');
                    }

                    function loadExpressionBuilderTemplates() {
                        return VR_GenericData_ExpressionBuilderConfigAPIService.GetExpressionBuilderTemplates().then(function (response) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.expressionBuilderTemplates.push(response[i]);
                            }
                        });
                    }
                    
                    function loadTextAreaSection() {
                        if (expressionBuilderValue != undefined) {
                            $scope.expressionValue = expressionBuilderValue;
                        }
                    }
                }
        }

        function buildExpressionBuilderObjFromScope() {
            return $scope.expressionValue;
        }

        function setExpressionBuilder() {

            var expressionBuilderObject = buildExpressionBuilderObjFromScope();
            if ($scope.onSetExpressionBuilder != undefined)
                $scope.onSetExpressionBuilder(expressionBuilderObject);
             $scope.modalContext.closeModal();

        }
    }

    appControllers.controller('VR_GenericData_ExpressionBuilderEditorController', ExpressionBuilderEditorController);
})(appControllers);
