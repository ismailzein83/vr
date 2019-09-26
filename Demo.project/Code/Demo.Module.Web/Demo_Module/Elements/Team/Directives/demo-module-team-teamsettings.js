(function (app) {

    "use strict";

    TeamSettingsDirective.$inject = ["Demo_Module_TeamAPIService", "UtilsService", "VRUIUtilsService"];

    function TeamSettingsDirective(Demo_Module_TeamAPIService, UtilsService, VRUIUtilsService) {

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
                var ctor = new TeamSettingsCtor($scope);
                ctor.initializeController();
            },
            template: function (attrs) {
                return getTemplate(attrs);
            }
        };

        function TeamSettingsCtor($scope) {
            this.initializeController = initializeController;

            var teamSettingsEntity;

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

                $scope.scopeModel.onTeamSettingsSelectionChanged = function () {
                    if (teamSettingsEntity == undefined) {
                        $scope.scopeModel.city = undefined;
                        $scope.scopeModel.stadium = undefined;
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
                        teamSettingsEntity = payload.teamSettingsEntity;
                    }

                    if (teamSettingsEntity != undefined) {
                        $scope.scopeModel.city = teamSettingsEntity.City;
                        $scope.scopeModel.stadium = teamSettingsEntity.Stadium;

                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getTeamSettingsConfigsPromise = getTeamSettingsConfigs();
                    promises.push(getTeamSettingsConfigsPromise);

                    function getTeamSettingsConfigs() {
                        return Demo_Module_TeamAPIService.GetTeamSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);

                                }
                                if (teamSettingsEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, teamSettingsEntity.ConfigID, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                teamSettingsEntity: teamSettingsEntity
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        teamSettingsEntity = undefined;
                    });
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigID = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                            data.City = $scope.scopeModel.city;
                            data.Stadium = $scope.scopeModel.stadium;
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
                + '<vr-columns colnum="{{normalColNum}}">'
                + '<span vr-disabled="isdisabled">'
                + ' <vr-select  on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' onselectionchanged="scopeModel.onTeamSettingsSelectionChanged"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + 'label="Team Settings" '
                + ' ' + hideremoveicon + ' '
                + 'isrequired ="isrequired"'
                + ' >'
                + '</vr-select>'
                + '</span>'
                + '</vr-columns>'
                + '<vr-columns width="1/2row" ng-if="scopeModel.selectedTemplateConfig != undefined">'
                + '<vr-textbox value="scopeModel.stadium" label="Stadium" isrequired></vr-textbox>'
                + '</vr-columns>'
                + '</vr-row>'

                + '<vr-row ng-if="scopeModel.selectedTemplateConfig != undefined">'
                + '<vr-columns width="1/2row">'
                + '<vr-textbox value="scopeModel.city" label="City" isrequired></vr-textbox>'
                + '</vr-columns>'


                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{normalColNum}}" isrequired="isrequired" customvalidate="customvalidate">'
                + '</vr-directivewrapper>'
                + '</vr-row>';
            
            return template;
        }
    }

    app.directive("demoModuleTeamTeamsettings", TeamSettingsDirective);
})(app);