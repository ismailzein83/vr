﻿'use strict';
app.directive('vrGenericdataBusinessentityRuntimeeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.scopeModel = {};
                              

                ctrl.selectedvalues;
                if ($attrs.selectionmode == "multiple")
                    ctrl.selectedvalues = [];

                if ($attrs.selectionmode == "dynamic")
                {
                    $scope.scopeModel.calculatedColNum = ctrl.normalColNum * 2;
                    $scope.scopeModel.showInDynamicMode = true;
                }
                else
                {
                    $scope.scopeModel.calculatedColNum = ctrl.normalColNum;
                }

                ctrl.datasource = [];
                var ctor = new selectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
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
            //var label = "";
            if (attrs.selectionmode == "dynamic" || attrs.selectionmode == "multiple") {
                //label = "";
                multipleselection = "ismultipleselection";
            }

            //var required = "";
            //if (attrs.isrequired != undefined)
            //    required = "isrequired";

            //var hideremoveicon = "";
            //if (attrs.hideremoveicon != undefined)
            //    hideremoveicon = "hideremoveicon";
            

            return '<vr-columns colnum="{{scopeModel.calculatedColNum}}">' +
            '<vr-directivewrapper directive="selector.directive" on-ready="selector.onDirectiveReady" '
                + multipleselection + '></vr-directivewrapper>'
                + '<vr-section title="{{scopeModel.fieldTitle}}" ng-if="scopeModel.showInDynamicMode"><vr-directivewrapper directive="dynamic.directive" on-ready="dynamic.onDirectiveReady"></vr-directivewrapper>' +
                '</vr-section></vr-columns>'
        }

        function selectorCtor(ctrl, $scope, $attrs) {

            var missingGroupSelectorUIControl;

            function initializeController() {
                //ctrl.showSelector = ($attrs.selectionmode == "dynamic");

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fieldType;

                    if (payload != undefined) {
                        $scope.scopeModel.fieldTitle = payload.fieldTitle;
                        fieldType = payload.fieldType;
                    }

                    if (fieldType != undefined) {

                        var promises = [];

                        var loadWholeSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadWholeSectionPromiseDeferred.promise);

                        var loadBusinessEntityDefPromiseDeferred = VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(fieldType.BusinessEntityDefinitionId).then(function (businessEntityDef) {
                            if (businessEntityDef.Settings != null) {

                                if (businessEntityDef.Settings.GroupSelectorUIControl == null || businessEntityDef.Settings.GroupSelectorUIControl == "")
                                {
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
                                    }

                                    $scope.dynamic.directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    innerSectionPromises.push($scope.dynamic.directiveLoadPromiseDeferred.promise);

                                    $scope.dynamic.directiveReadyPromiseDeferred.promise.then(function () {
                                        VRUIUtilsService.callDirectiveLoad($scope.dynamic.directiveAPI, undefined, $scope.dynamic.directiveLoadPromiseDeferred);
                                    });

                                }
                                else {
                                    $scope.selector = {};
                                    $scope.selector.directive = businessEntityDef.Settings.SelectorUIControl;
                                    $scope.selector.directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    $scope.selector.onDirectiveReady = function (api) {
                                        $scope.selector.directiveAPI = api;
                                        $scope.selector.directiveReadyPromiseDeferred.resolve();
                                    }

                                    $scope.selector.directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    innerSectionPromises.push($scope.selector.directiveLoadPromiseDeferred.promise);

                                    $scope.selector.directiveReadyPromiseDeferred.promise.then(function () {
                                        VRUIUtilsService.callDirectiveLoad($scope.selector.directiveAPI, undefined, $scope.selector.directiveLoadPromiseDeferred);
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
                }

                api.getData = function()
                {
                    var retVal;

                    if (ctrl.selectionmode == "dynamic") {

                        if (missingGroupSelectorUIControl)
                        {
                            retVal = {
                                $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                                Values: $scope.selector.directiveAPI.getSelectedIds()
                            }
                        }
                        else
                        {
                            retVal = {
                                $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.BusinessEntityValues, Vanrise.GenericData.MainExtensions",
                                BusinessEntityGroup: $scope.dynamic.directiveAPI.getData()
                            }
                        }
                    }
                    else if (ctrl.selectionmode == "single" || ctrl.selectionmode == "multiple") {
                        retVal = $scope.selector.directiveAPI.getSelectedIds();
                    }

                    return retVal;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);