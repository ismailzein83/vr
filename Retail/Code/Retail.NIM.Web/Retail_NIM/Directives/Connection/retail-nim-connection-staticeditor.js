"use strict";

app.directive("retailNimConnectionStaticeditor", ["VRUIUtilsService", "UtilsService",
    function (VRUIUtilsService, UtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                isrequired: "=",
                normalColNum: '@',
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ConnectionStaticEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_NIM/Directives/Connection/Templates/ConnectionStaticEditorTemplate.html'
        };

        function ConnectionStaticEditorCtor(ctrl, $scope, attrs) {
            var connectionDirectiveAPI;
            var connectionSelectedDeferred = UtilsService.createPromiseDeferred();
            var isAddMode;


            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onConnectionDirectiveReady = function (api) {
                    connectionDirectiveAPI = api;
                    defineAPI();
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        isAddMode = payload.genericContext != undefined ? payload.genericContext.isAddMode() : undefined;
                        var connectionDirectivePayload = {
                            selectedValues: payload.selectedValues,
                            parentFieldValues: payload.parentFieldValues,
                            isAddMode: isAddMode
                        };
                    }

                    var connectionDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    VRUIUtilsService.callDirectiveLoad(connectionDirectiveAPI, connectionDirectivePayload, connectionDirectiveLoadPromiseDeferred);

                    return connectionDirectiveLoadPromiseDeferred.promise.finally(function () {
                        connectionSelectedDeferred = undefined;
                    });

                };

                api.onFieldValueChanged = function (selectedItems, changedItem) {
                    var isConnectionModelChanged = changedItem.changedField.fieldName.search("Model");
                    if (isConnectionModelChanged < 0) {
                        if (selectedItems != undefined) {
                            if (connectionSelectedDeferred != undefined) {
                                connectionSelectedDeferred.resolve();
                            } else {
                                connectionDirectiveAPI.clearDataSource();
                                if (selectedItems.Area != undefined && selectedItems.Site != undefined) {
                                    var connectionDirectivePayload = {
                                        parentFieldValues: {
                                            Area: selectedItems.Area[0],
                                            Site: selectedItems.Site[0]
                                        },
                                        isAddMode: isAddMode
                                    };

                                    var setLoader = function (value) {
                                        $scope.scopeModel.selectedItems = value;
                                    };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, connectionDirectiveAPI, connectionDirectivePayload, setLoader);
                                }
                            }
                        }
                    }
                };

                api.setData = function (Connection) {
                    var object = connectionDirectiveAPI.getData();
                    //Connection.Model = object.Model;
                    Connection.Port1 = object.Port1;
                    Connection.Port2 = object.Port2;
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);