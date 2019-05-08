(function (app) {

    'use strict';

    DataRecordStorageRDBJoins.$inject = ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordStorageService'];

    function DataRecordStorageRDBJoins(VRNotificationService, VRUIUtilsService, UtilsService, VR_GenericData_DataRecordStorageService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordStorageRDBJoins = new DataRecordStorageRDBJoinsController($scope, ctrl, $attrs);
                dataRecordStorageRDBJoins.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/DataRecordStorageRDBJoinsTemplate.html'
        };

        function DataRecordStorageRDBJoinsController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;

            var joinGridAPI;
            var joinGridReadyDeferred = UtilsService.createPromiseDeferred();
           
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.joins = [];

                $scope.scopeModel.onJoinGridReady = function (api) {
                    joinGridAPI = api;
                    joinGridReadyDeferred.resolve();
                    defineAPI();
                };

                $scope.scopeModel.addJoin = function () {
                    var onJoinAdded = function (addedJoinObj) {
                        $scope.scopeModel.joins.push(addedJoinObj);
                    };
                    VR_GenericData_DataRecordStorageService.addRDBJoinDataRecordStorage(getContext(), onJoinAdded);
                };

                $scope.scopeModel.removeJoinColumn = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.joins, dataItem.RDBRecordStorageJoinName, 'RDBRecordStorageJoinName');
                    if (index > -1) {
                        $scope.scopeModel.joins.splice(index, 1);
                    }
                };

                $scope.scopeModel.isJoinsValid = function () {
                    if ($scope.scopeModel.joins.length > 0 && checkDuplicateName())
                        return "Two or more columns have the same name.";

                    return null;
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.isLoading = true;
                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        var joinsList = payload.joins;

                        if (joinsList != undefined) {
                            for (var i = 0; i < joinsList.length; i++) {
                                var join = joinsList[i];
                                $scope.scopeModel.joins.push(join);
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
                    return $scope.scopeModel.joins;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editRDBJoinDataRecordStorage,
                }];
            }

            function editRDBJoinDataRecordStorage(joinEntity) {

                var onJoinUpdated = function (updatedJoinObj) {
                    var index = $scope.scopeModel.joins.indexOf(joinEntity);
                    $scope.scopeModel.joins[index] = updatedJoinObj;
                };

                VR_GenericData_DataRecordStorageService.editRDBJoinDataRecordStorage(joinEntity, getContext(), onJoinUpdated);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

            function checkDuplicateName() {
                var joinsLength = $scope.scopeModel.joins.length;
                for (var i = 0; i < joinsLength; i++) {
                    var currentItem = $scope.scopeModel.joins[i];
                    for (var j = i + 1; j < joinsLength; j++) {
                        if (i != j && $scope.scopeModel.joins[j].RDBRecordStorageJoinName == currentItem.RDBRecordStorageJoinName)
                            return true;
                    }
                }
                return false;
            }
        }
    }

    app.directive('vrGenericdataDatarecordstorageRdbJoins', DataRecordStorageRDBJoins);

})(app);