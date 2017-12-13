"use strict";

app.directive("vrCommonLocalizationressourcekeySelector", ["VRNotificationService", 'VRCommon_VRLocalizationTextResourceAPIService', 'VRCommon_VRLocalizationTextResourceService', 'VRUIUtilsService', 'UtilsService',
    function (VRNotificationService, VRCommon_VRLocalizationTextResourceAPIService, VRCommon_VRLocalizationTextResourceService, VRUIUtilsService, UtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '=',
                selectedValue: '=',
                customlabel:'@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrLocalizationressourcekeySelector = new VrLocalizationressourcekeySelector($scope, ctrl, $attrs);
                vrLocalizationressourcekeySelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            template: function (element, attrs) {
                return getTextResourceSelectorTemplate(attrs);
            }
        };
        function getTextResourceSelectorTemplate(attrs) {
            var label = "Text Resource";
            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }
            return '<vr-common-vrlocalizationtextresource-selector on-ready="scopeModel.onTextResourceSelectorReady" normal-col-num="12" selectedvalues="scopeModel.selectedResourceKey" isrequired="true" customlabel="' + label + '"></vr-common-vrlocalizationtextresource-selector>'

        }
        function VrLocalizationressourcekeySelector($scope, ctrl, $attrs) {
            var textResourceSelectorAPI;
            var textResourceSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedResourceKey;

            this.initializeController = initializeController;

            function initializeController() {
               

                $scope.scopeModel = {};

                $scope.scopeModel.onTextResourceSelectorReady = function (api) {
                    textResourceSelectorAPI = api;
                    textResourceSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                
                    if (payload != undefined) {
                        selectedResourceKey = payload.selectedResourceKey;
                    }
                    promises.push(loadTextResourceSelector());


                    return UtilsService.waitMultiplePromises(promises);
                };
                api.getResourceKey = function () {
                    return $scope.scopeModel.selectedResourceKey.ResourceKey;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
                ctrl.selectedValue = $scope.scopeModel.selectedResourceKey;
            }
            function loadTextResourceSelector() {
                var textResourceSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                textResourceSelectorReadyDeferred.promise.then(function () {
                    var payload = {};
                    if (selectedResourceKey != undefined)
                        payload.selectedValue = selectedResourceKey;
                    VRUIUtilsService.callDirectiveLoad(textResourceSelectorAPI, payload, textResourceSelectorLoadDeferred);
                });
                return textResourceSelectorLoadDeferred.promise;
            }
           
        }
       


        return directiveDefinitionObject;
    }
]);