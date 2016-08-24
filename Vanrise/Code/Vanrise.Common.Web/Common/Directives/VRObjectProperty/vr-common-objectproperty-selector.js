(function (app) {

    'use strict';

    VRPropertySelector.$inject = ['VRCommon_VRObjectTypeDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRPropertySelector(VRCommon_VRObjectTypeDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var propertySelector = new PropertySelector($scope, ctrl, $attrs);
                propertySelector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRObjectProperty/Templates/VRObjectPropertySelectorTemplate.html'
        };

        function PropertySelector($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var property; // property = {valueObjectName:valueObjectName, valuePropertyName:valuePropertyName}

            var objectSelectorAPI;
            var objectSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var onObjectSelectionChangedDeferred;

            var propertySelectorAPI;
            var propertySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                var _promises = [objectSelectorReadyDeferred.promise, propertySelectorReadyDeferred.promise];
                $scope.scopeModel = {};

                $scope.scopeModel.objects = [];
                $scope.scopeModel.properties = [];
                $scope.scopeModel.selectedObject;
                $scope.scopeModel.selectedProperty;

                $scope.scopeModel.onObjectSelectorReady = function (api) {
                    objectSelectorAPI = api;
                    objectSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onObjectSelectionChanged = function () {

                    if ($scope.scopeModel.selectedObject != undefined) {

                        if (onObjectSelectionChangedDeferred != undefined) {
                            onObjectSelectionChangedDeferred.resolve();
                        }
                        else {
                            loadPropertySelector();
                        }
                    }
                }

                $scope.scopeModel.onPropertySelectorReady = function (api) {
                    propertySelectorAPI = api;
                    propertySelectorReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises(_promises).then(function () {
                    defineAPI();
                });

                function loadPropertySelector() {
                    propertySelectorAPI.clearDataSource();
                    property = undefined;

                    $scope.scopeModel.isPropertySelectorLoading = true;
                    var vrObjectTypeDefinitionId = $scope.scopeModel.selectedObject.VRObjectTypeDefinitionId;

                    VRCommon_VRObjectTypeDefinitionAPIService.GetVRObjectTypeDefinition(vrObjectTypeDefinitionId).then(function (response) {
                        if (response != null) {
                            var objectTypeDefinition = response;
                            var properties = objectTypeDefinition.Settings.Properties;

                            for (var key in properties) {
                                if (key != "$type") {
                                    var property = properties[key];
                                    $scope.scopeModel.properties.push(property);
                                }
                            }
                            $scope.scopeModel.isPropertySelectorLoading = false;
                        }
                    });
                }
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    objectSelectorAPI.clearDataSource();
                    propertySelectorAPI.clearDataSource();

                    if (payload != undefined && payload.property != undefined)
                        property = payload.property;

                    //Loading ObjectSelector
                    if (payload != undefined && payload.objects != undefined) {
                        for (var key in payload.objects)
                            $scope.scopeModel.objects.push(payload.objects[key]);

                        if (property != undefined) {
                            $scope.scopeModel.selectedObject = UtilsService.getItemByVal($scope.scopeModel.objects, property.objectName, 'ObjectName');

                            //In Case we have deleted the object
                            if ($scope.scopeModel.selectedObject == undefined) {
                                property = undefined;
                            }
                        }
                    }

                    //Loading PropertySelector
                    if ($scope.scopeModel.selectedObject != undefined && $scope.scopeModel.selectedObject.VRObjectTypeDefinitionId != undefined) {
                        var vrObjectTypeDefinitionId = $scope.scopeModel.selectedObject.VRObjectTypeDefinitionId;

                        var loadPropertySelectorPromise = loadPropertySelector(vrObjectTypeDefinitionId);

                        return loadPropertySelectorPromise;
                    }
                };

                api.getData = function () {

                    if ($scope.scopeModel.selectedObject != undefined) {
                        var valueObjectName = $scope.scopeModel.selectedObject.ObjectName;
                    }
                    if ($scope.scopeModel.selectedProperty) {
                        var valuePropertyName = $scope.scopeModel.selectedProperty.Name;
                    }

                    var data;
                    if (valueObjectName != undefined && valuePropertyName != undefined)
                        data = { valueObjectName: valueObjectName, valuePropertyName: valuePropertyName };

                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadPropertySelector(vrObjectTypeDefinitionId) {

                if (onObjectSelectionChangedDeferred == undefined)
                    onObjectSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                var propertySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                onObjectSelectionChangedDeferred.promise.then(function () {
                    onObjectSelectionChangedDeferred = undefined;
                    VRCommon_VRObjectTypeDefinitionAPIService.GetVRObjectTypeDefinition(vrObjectTypeDefinitionId).then(function (response) {

                        if (response != null) {
                            var objectTypeDefinition = response;
                            var properties = objectTypeDefinition.Settings.Properties;

                            for (var key in properties) {
                                if (key != "$type") {
                                    var _property = properties[key];
                                    $scope.scopeModel.properties.push(_property);
                                }
                            }

                            if (property != undefined && property.propertyName != undefined) {
                                $scope.scopeModel.selectedProperty =
                                        UtilsService.getItemByVal($scope.scopeModel.properties, property.propertyName, 'Name');
                            }

                            propertySelectorLoadDeferred.resolve();
                        }
                    });
                });

                return propertySelectorLoadDeferred.promise;
            }
        }
    }

    app.directive('vrCommonObjectpropertySelector', VRPropertySelector);

})(app);
