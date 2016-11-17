'use strict';
app.directive('vrCommonAppendixsampleAppendixlist', ['UtilsService', 'Common_AppendixSample_Service', 'VRUIUtilsService',
function (UtilsService, Common_AppendixSample_Service, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
               
                ctrl.dynamicAppendixTemplatesToAdd = [];
                ctrl.dynamicAppendixList = [];

                ctrl.addDynamicAppendixToList = function () {
                    var dynamicAppendixItem = {
                        editor: ctrl.selectedDynamicAppendixToAdd.editor
                    };

                    dynamicAppendixItem.dynamicAppendixReady = function (api) {
                        dynamicAppendixItem.dynamicAppendixAPI = api;
                        var setLoader = function (value) { dynamicAppendixItem.isLoading = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dynamicAppendixItem.dynamicAppendixAPI, undefined, setLoader);
                    };

                    ctrl.dynamicAppendixList.push(dynamicAppendixItem);
                    ctrl.selectedDynamicAppendixToAdd = undefined;
                };

                function loadDynamicAppendixListSection(payload) {
                    var promises = [];

                    var appendixItems;
                    if (payload != undefined) {
                        appendixItems = [];
                        for (var i = 0; i < payload.length; i++) {
                            var appendixItem = {
                                payload: payload[i],
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(appendixItem.loadPromiseDeferred.promise);
                            appendixItems.push(appendixItem);
                        }
                    }

                    var loadTemplatesPromise = Common_AppendixSample_Service.getRemoteData(1000)
                        .then(function () {
                            ctrl.dynamicAppendixTemplatesToAdd.push({
                                name: "Appendix 1",
                                editor: "vr-common-appendixsample-appendix"
                            });
                            ctrl.dynamicAppendixTemplatesToAdd.push({
                                name: "Appendix 2",
                                editor: "vr-common-appendixsample-appendix2"
                            });
                            if (appendixItems != undefined) {
                                for (var i = 0; i < appendixItems.length; i++) {
                                    loadDynamicAppendixItem(appendixItems[i]);
                                }
                            }
                        });
                    promises.push(loadTemplatesPromise);                    

                    function loadDynamicAppendixItem(appendixItem) {
                        var matchItem = UtilsService.getItemByVal(ctrl.dynamicAppendixTemplatesToAdd, appendixItem.payload.name, "name");
                        if (matchItem == null) 
                            return;

                        var dynamicAppendixItem = {
                            editor: matchItem.editor
                        };

                        dynamicAppendixItem.dynamicAppendixReady = function (api) {
                            dynamicAppendixItem.dynamicAppendixAPI = api;
                            appendixItem.readyPromiseDeferred.resolve();
                        };                        

                        appendixItem.readyPromiseDeferred.promise
                            .then(function () {
                                VRUIUtilsService.callDirectiveLoad(dynamicAppendixItem.dynamicAppendixAPI, undefined, appendixItem.loadPromiseDeferred);
                            });

                        ctrl.dynamicAppendixList.push(dynamicAppendixItem);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                }

                var api = {};
                api.load = function (payload) {
                    return loadDynamicAppendixListSection(payload);
                };
                setTimeout(function () {
                    if (ctrl.onReady != undefined)
                        ctrl.onReady(api);
                }, 100);
              
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return '<vr-row>'
                +'<vr-columns width="normal">'
                +'    <vr-select datatextfield="name" datavaluefield="name" label="Directive Template" datasource="ctrl.dynamicAppendixTemplatesToAdd"'
                + 'selectedvalues="ctrl.selectedDynamicAppendixToAdd">'
 +'</vr-columns>'
 +'<vr-columns width="normal" withemptyline>'
   + '  <vr-button type="Add" standalone vr-disabled="ctrl.selectedDynamicAppendixToAdd == undefined" data-onclick="ctrl.addDynamicAppendixToList"></vr-button>'
 +'</vr-columns>'
+'</vr-row>'
+'<vr-row>'
 + '<vr-columns width="fullrow" ng-repeat="dynamicAppendixItem in ctrl.dynamicAppendixList" vr-loader="dynamicAppendixItem.isLoading">'
   +'  <vr-directivewrapper directive="dynamicAppendixItem.editor" on-ready="dynamicAppendixItem.dynamicAppendixReady"></vr-directivewrapper>'
 + '</vr-columns>'
+'</vr-row>';
            }

        };
        
        return directiveDefinitionObject;
    }]);