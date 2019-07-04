'use strict';

app.directive('vrGenericdataGenericbusinessentityApprovalgrid', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new genricApprovalEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntityApprovalGrid.html"
        };

        function genricApprovalEditor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;
            var selectedValues;
            var gridAPI;
            var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var filterGroup;
            var businessDefinitionId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridDirectiveReady = function (api) {
                    gridAPI = api;
                    gridReadyPromiseDeferred.resolve();
                };

                defineApi();
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {

                    var promises = [];
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            filterGroup = JSON.parse(selectedValues.FilterGroup);
                            businessDefinitionId = selectedValues.GenericBeDefinitionId;
                        }
                    }

                    function loadGrid() {
                        var gridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        gridReadyPromiseDeferred.promise.then(function () {
                            gridAPI.load(getFilterObject()).then(function () {
                                gridLoadPromiseDeferred.resolve();
                            });
                        });

                        return gridLoadPromiseDeferred.promise;
                    }

                    promises.push(loadGrid());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (approvalObject) {
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getFilterObject() {

                var gridPayload = {
                    query: {
                        FilterGroup: filterGroup,
                    },
                    businessEntityDefinitionId: businessDefinitionId,
                };
                return gridPayload;
            }

        }
        return directiveDefinitionObject;
    }]);