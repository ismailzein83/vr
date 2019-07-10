"use strict";

app.directive("vrGenericdataGenericbeGridactiongroupdefinitionGrid", ["UtilsService", "VRUIUtilsService","VRLocalizationService",
    function (UtilsService, VRUIUtilsService,  VRLocalizationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GridActionGroupDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/GridActionGroupDefinitionGridTemplate.html"
        };

        function GridActionGroupDefinitionGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                $scope.scopeModel = {};
                ctrl.isValid = function () {
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each action should be unique.";

                    return null;
                };
                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                ctrl.addGridActionGroup = function () {
                    var dataItem = {
                        entity: { GenericBEGridActionGroupId: UtilsService.guid()}
                    };
                
                    dataItem.onTextResourceSelectorReady = function (api) {
                        dataItem.textResourceSeletorAPI = api;
                        var setLoader = function (value) { dataItem.isFieldTextResourceSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                    };
                    ctrl.datasource.push(dataItem);
                };

                ctrl.removeGridActionGroup = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                defineAPI();
            }
            function prepareDataItem(groupActionObject) {

                var dataItem = {
                    entity: {
                        Title: groupActionObject.payload.Title,
                        GenericBEGridActionGroupId: groupActionObject.payload.GenericBEGridActionGroupId
                    },

                    oldTextResourceKey: groupActionObject.payload.TextResourceKey
                };
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    groupActionObject.textResourceReadyPromiseDeferred.resolve();
                };
               

                groupActionObject.textResourceReadyPromiseDeferred.promise.then(function () {
                    var textResourcePayload = { selectedValue: groupActionObject.payload.TextResourceKey };
                    VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, groupActionObject.textResourceLoadPromiseDeferred);
                });

                ctrl.datasource.push(dataItem);
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {

                        context = payload.context;
                        api.clearDataSource();
                        if (payload.genericBEGridActionGroups != undefined) {
                            for (var i = 0; i < payload.genericBEGridActionGroups.length; i++) {
                                var item = payload.genericBEGridActionGroups[i];
                                var groupActionObject = {
                                    payload: item,
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                };
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(groupActionObject.textResourceLoadPromiseDeferred.promise);
                                prepareDataItem(groupActionObject);
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    var gridActionGroups;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        gridActionGroups = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            gridActionGroups.push({
                                GenericBEGridActionGroupId: currentItem.entity.GenericBEGridActionGroupId,
                                Title: currentItem.entity.Title,
                                TextResourceKey: currentItem.textResourceSeletorAPI != undefined ? currentItem.textResourceSeletorAPI.getSelectedValues() : currentItem.oldTextResourceKey
                            });
                        }
                    }
                    return gridActionGroups;
                };

                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i].entity; 
                    for (var j = i + 1; j < ctrl.datasource.length; j++) {
                        if (ctrl.datasource[j].entity.Title == currentItem.Title)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);