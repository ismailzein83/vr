(function (app) {

    'use strict';

    DataRecordStorageRDBExpressionFields.$inject = ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordStorageService'];

    function DataRecordStorageRDBExpressionFields(VRNotificationService, VRUIUtilsService, UtilsService, VR_GenericData_DataRecordStorageService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordStorageRDBExpressionFields = new DataRDBExpressionFieldsController($scope, ctrl, $attrs);
                dataRecordStorageRDBExpressionFields.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/DataRecordStorageRDBExpressionFieldsTemplate.html'
        };

        function DataRDBExpressionFieldsController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;

            var expressionFieldGridAPI;
            var expressionFieldGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.expressionFields = [];

                $scope.scopeModel.onExpressionFieldGridReady = function (api) {
                    expressionFieldGridAPI = api;
                    expressionFieldGridReadyDeferred.resolve();
                    defineAPI();
                };

                $scope.scopeModel.addExpressionField = function () {
                    var onExpressionFieldAdded = function (addedExpressionFieldObj) {
                        $scope.scopeModel.expressionFields.push(addedExpressionFieldObj);
                    };
                    VR_GenericData_DataRecordStorageService.addRDBExpressionField(getContext(), onExpressionFieldAdded);
                };

                $scope.scopeModel.removeExpressionField = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.expressionFields, dataItem.FieldName, 'FieldName');
                    if (index > -1) {
                        $scope.scopeModel.expressionFields.splice(index, 1);
                    }
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.isLoading = true;
                    var initialPromises = [];

                    if (payload != undefined) {
                        var expressionFields = payload.expressionFields;
                        context = payload.context;

                        if (expressionFields != undefined) {
                            for (var i = 0; i < expressionFields.length; i++) {
                                var expressionField = expressionFields[i];
                                $scope.scopeModel.expressionFields.push(expressionField);
                            }
                        }
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                        $scope.isLoading = false;
                    });
                };

                api.getData = function () {
                    return $scope.scopeModel.expressionFields;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editRDBExpressionField
                }];
            }

            function editRDBExpressionField(expressionFieldEntity) {
                var onExpressionFieldUpdated = function (updatedExpressionFieldObj) {
                    var index = $scope.scopeModel.expressionFields.indexOf(expressionFieldEntity);
                    $scope.scopeModel.expressionFields[index] = updatedExpressionFieldObj;
                };

                VR_GenericData_DataRecordStorageService.editRDBExpressionField(expressionFieldEntity, getContext(), onExpressionFieldUpdated);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }
    }

    app.directive('vrGenericdataDatarecordstorageRdbExpressionfields', DataRecordStorageRDBExpressionFields);

})(app);