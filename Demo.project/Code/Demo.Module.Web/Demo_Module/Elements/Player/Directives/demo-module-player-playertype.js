(function (app) {

    "use strict";

    PlayerTypeDirective.$inject = ["Demo_Module_TeamAPIService", "UtilsService", "VRUIUtilsService"];

    function PlayerTypeDirective(Demo_Module_TeamAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '=',
                isdisabled: '='
            },
            controller: function ($scope) {
                var ctor = new PlayerTypeCtor($scope);
                ctor.initializeController();
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function PlayerTypeCtor($scope) {
            this.initializeController = initializeController;

            var playerTypeEntity;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onplayerTypeSelectionChanged = function () {
                    if (playerTypeEntity == undefined) {
                        $scope.scopeModel.nationality = undefined;
                    }
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    if (payload != undefined) {
                        playerTypeEntity = payload.playerTypeEntity;
                    }

                    if (playerTypeEntity != undefined) {
                        $scope.scopeModel.nationality = payload.playerTypeEntity.Nationality;

                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getPlayerTypeConfigsPromise = getPlayerTypeConfigs();
                    promises.push(getPlayerTypeConfigsPromise);

                    function getPlayerTypeConfigs() {
                        return Demo_Module_TeamAPIService.GetPlayerTypeConfigs().then(function (response) { 
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (playerTypeEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, playerTypeEntity.ConfigID, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                playerTypeEntity: playerTypeEntity
                            };

                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        playerTypeEntity = undefined;
                    });
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigID = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                            data.Nationality = $scope.scopeModel.nationality;
                        }
                    }
                    return data;
                };

                if ($scope.onReady != null) {
                    $scope.onReady(api);
                }
            }
        }

        function getTemplate(attrs) {

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }
            
            var template =
                '<vr-row>'
                + '<vr-columns colnum="{{normalColNum}}" width="1/2row">'
                + '<span vr-disabled="isdisabled">'
                + ' <vr-select  on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' onselectionchanged="scopeModel.onplayerTypeSelectionChanged"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + 'label="Type" '
                + ' ' + hideremoveicon + ' '
                + 'isrequired ="isrequired">'
                + '</vr-select>'
                + '</span>'
                + '</vr-columns>'
                + '<vr-columns width = "1/2row" ng-if="scopeModel.selectedTemplateConfig != undefined" >'
                + '<vr-textbox value="scopeModel.nationality" label="Nationality" isrequired></vr-textbox>'
                + '</vr-columns>'
                + '</vr-row>'

                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{normalColNum}}" isrequired="isrequired" customvalidate="customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }
    }

    app.directive("demoModulePlayerPlayertype", PlayerTypeDirective);
})(app);