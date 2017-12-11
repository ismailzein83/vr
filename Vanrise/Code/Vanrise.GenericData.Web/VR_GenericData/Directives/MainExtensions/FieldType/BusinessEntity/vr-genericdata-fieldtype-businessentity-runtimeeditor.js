'use strict';
app.directive('vrGenericdataFieldtypeBusinessentityRuntimeeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                $scope.scopeModel = {};

                ctrl.selectedvalues;
                if ($attrs.selectionmode == "multiple")
                    ctrl.selectedvalues = [];

                if ($attrs.selectionmode == "dynamic") {
                    $scope.scopeModel.calculatedColNum = ctrl.normalColNum * 4;
                    $scope.scopeModel.showInDynamicMode = true;
                }
                else {
                    $scope.scopeModel.calculatedColNum = ctrl.normalColNum;
                }

                ctrl.datasource = [];
                var ctor = new businessEntityCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'runtimeEditorCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {
                    }
                }
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };


        function getTemplate(attrs) {

            var multipleselection = "";

            if (attrs.selectionmode == "dynamic" || attrs.selectionmode == "multiple") {
                multipleselection = "ismultipleselection";
            }

            if (attrs.selectionmode == "dynamic") {
                return '<vr-directivewrapper directive="selector.directive" normal-col-num="{{scopeModel.calculatedColNum}}" on-ready="selector.onDirectiveReady" onselectionchanged="selector.onselectionchanged" '
                + multipleselection + ' isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>'
                + '<vr-row removeline> <vr-columns width="fullrow"> <vr-section title="{{scopeModel.fieldTitle}}" ng-if="scopeModel.showInDynamicMode"><vr-directivewrapper directive="dynamic.directive" normal-col-num="{{scopeModel.calculatedColNum}}" on-ready="dynamic.onDirectiveReady" isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>' +
                '</vr-section> </vr-columns> </vr-row> ';
            }
            else {
                return '<vr-directivewrapper directive="selector.directive" normal-col-num="{{scopeModel.calculatedColNum}}" on-ready="selector.onDirectiveReady" onselectionchanged="selector.onselectionchanged" '
                + multipleselection + ' isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>'
                + '<vr-section title="{{scopeModel.fieldTitle}}" ng-if="scopeModel.showInDynamicMode"><vr-directivewrapper directive="dynamic.directive" normal-col-num="{{scopeModel.calculatedColNum}}" on-ready="dynamic.onDirectiveReady" isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>' +
                '</vr-section>';
            }

        }

        function businessEntityCtor(ctrl, $scope, $attrs) {
            var missingGroupSelectorUIControl;

            function initializeController() {
                //ctrl.showSelector = ($attrs.selectionmode == "dynamic");

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var fieldType;
                    var fieldValue;
                    var fieldTitle;
                    var genericUIContext;
                    if (payload != undefined) {
                        $scope.scopeModel.fieldTitle = payload.fieldTitle;
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        genericUIContext = payload.genericUIContext;
                    }

                    if (fieldType != undefined) {
                        var pendingNotifications = [];

                        var promises = [];

                        var loadWholeSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadWholeSectionPromiseDeferred.promise);

                        var loadBusinessEntityDefPromiseDeferred = VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(fieldType.BusinessEntityDefinitionId).then(function (businessEntityDef) {
                            if (businessEntityDef.Settings != null) {

                                if (businessEntityDef.Settings.GroupSelectorUIControl == null || businessEntityDef.Settings.GroupSelectorUIControl == "") {
                                    $scope.scopeModel.showInDynamicMode = false;
                                    missingGroupSelectorUIControl = true;
                                    $scope.scopeModel.calculatedColNum = ctrl.normalColNum;
                                }

                                var innerSectionPromises = [];
                                if ($attrs.selectionmode == "dynamic" && !missingGroupSelectorUIControl) {
                                    $scope.dynamic = {};
                                    $scope.dynamic.directive = businessEntityDef.Settings.GroupSelectorUIControl;
                                    $scope.dynamic.directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    $scope.dynamic.onDirectiveReady = function (api) {
                                        $scope.dynamic.directiveAPI = api;
                                        $scope.dynamic.directiveReadyPromiseDeferred.resolve();
                                    };

                                    $scope.dynamic.directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    innerSectionPromises.push($scope.dynamic.directiveLoadPromiseDeferred.promise);

                                    $scope.dynamic.directiveReadyPromiseDeferred.promise.then(function () {
                                        var payload;

                                        if (fieldValue != undefined)
                                            payload = fieldValue.BusinessEntityGroup;

                                        VRUIUtilsService.callDirectiveLoad($scope.dynamic.directiveAPI, payload, $scope.dynamic.directiveLoadPromiseDeferred);
                                    });
                                }
                                else {
                                    $scope.selector = {};
                                    $scope.selector.directive = businessEntityDef.Settings.SelectorUIControl;
                                    $scope.selector.directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    $scope.selector.onDirectiveReady = function (api) {
                                        $scope.selector.directiveAPI = api;
                                        $scope.selector.directiveReadyPromiseDeferred.resolve();
                                    };

                                    $scope.selector.onselectionchanged = function (selectedvalue) {
                                        if (genericUIContext != undefined && genericUIContext.notifyValueChanged != undefined && typeof (genericUIContext.notifyValueChanged) == "function") {
                                            genericUIContext.notifyValueChanged(selectedvalue);
                                        }
                                    };

                                    $scope.selector.directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    innerSectionPromises.push($scope.selector.directiveLoadPromiseDeferred.promise);

                                    $scope.selector.directiveReadyPromiseDeferred.promise.then(function () {
                                        var payload = {
                                            businessEntityDefinitionId: fieldType.BusinessEntityDefinitionId,
                                            fieldTitle: $scope.scopeModel.fieldTitle,
                                            beRuntimeSelectorFilter: fieldType.BERuntimeSelectorFilter,
                                            genericUIContext: genericUIContext
                                        };
                                        if (fieldValue != undefined) {
                                            payload.selectedIds = ($attrs.selectionmode == "dynamic" && missingGroupSelectorUIControl) ? fieldValue.Values : fieldValue;
                                        }

                                        VRUIUtilsService.callDirectiveLoad($scope.selector.directiveAPI, payload, $scope.selector.directiveLoadPromiseDeferred);
                                    });
                                }

                                UtilsService.waitMultiplePromises(innerSectionPromises).then(function () {
                                    loadWholeSectionPromiseDeferred.resolve();
                                }).catch(function (error) {
                                    loadWholeSectionPromiseDeferred.reject(error);
                                });
                            }
                        });

                        promises.push(loadBusinessEntityDefPromiseDeferred);
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var retVal;

                    if (ctrl.selectionmode == "dynamic") {
                        if (missingGroupSelectorUIControl) {
                            var selectedIds = $scope.selector.directiveAPI.getSelectedIds();
                            if (selectedIds != undefined) {
                                retVal = {
                                    $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                                    Values: selectedIds
                                };
                            }
                        }
                        else {
                            var directiveData = $scope.dynamic.directiveAPI.getData();
                            if (directiveData != undefined) {
                                retVal = {
                                    $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.BusinessEntityValues, Vanrise.GenericData.MainExtensions",
                                    BusinessEntityGroup: directiveData
                                };
                            }
                        }
                    }
                    else if (ctrl.selectionmode == "single" || ctrl.selectionmode == "multiple") {
                        retVal = $scope.selector.directiveAPI.getSelectedIds();
                    }

                    return retVal;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);