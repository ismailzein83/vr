(function (app) {

    'use strict';

    whsRoutesyncHuaweiSettings.$inject = ["UtilsService", 'VRUIUtilsService',];

    function whsRoutesyncHuaweiSettings(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SwitchSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSynchronizer/Templates/HuaweiSettingsTemplate.html"
        };

        function SwitchSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var overriddenRSSNAPI;
            var overriddenRSSNGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.header = "Overridden RSSN";
                $scope.scopeModel.overriddenRSSNsInRSName = [];
                $scope.scopeModel.hint = "Upon creating RSNames, any Existing RSSN will be replaced by the corresponding Overridden RSSN.";


                $scope.scopeModel.onOverriddenRSSNsGridReady = function (api) {
                    overriddenRSSNAPI = api;
                    overriddenRSSNGridReadyDeferred.resolve();
                };

                $scope.scopeModel.isValid = function () {

                    if ($scope.scopeModel.overriddenRSSNsInRSName != undefined && $scope.scopeModel.overriddenRSSNsInRSName.length > 0) {
                        var overriddenRSSNsInRSName = $scope.scopeModel.overriddenRSSNsInRSName;
                        var length = overriddenRSSNsInRSName.length;
                        for (var i = 0; i < length - 1; i++) {
                            for (var j = i + 1; j < length; j++) {
                                if (overriddenRSSNsInRSName[i].exisitingRSSN.split(' ').join('').toLowerCase() == overriddenRSSNsInRSName[j].exisitingRSSN.split(' ').join('').toLowerCase())
                                    return 'Duplicate Existing RSSNs are not allowed';
                            }
                        }
                    }
                    return;
                };

                $scope.scopeModel.addOverriddenRSSNInRSName = function () {
                    var dataItem = {};
                    $scope.scopeModel.overriddenRSSNsInRSName.push(dataItem);
                };

                $scope.scopeModel.removeOverriddenRSSNInRSName = function (dataItem) {
                    var index = $scope.scopeModel.overriddenRSSNsInRSName.indexOf(dataItem);
                    $scope.scopeModel.overriddenRSSNsInRSName.splice(index, 1);
                };

                var promises = [overriddenRSSNGridReadyDeferred.promise];
                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var overriddenRSSNsInRSName;

                    if (payload != undefined) {
                        var overriddenRSSNsInRSName = payload.overriddenRSSNsInRSName;
                        if (overriddenRSSNsInRSName != undefined) {
                            for (var key in overriddenRSSNsInRSName) {
                                if (key != "$type") {
                                    var overriddenRSSNsInRSNameItem = {
                                        exisitingRSSN: key,
                                        overriddenRSSN: overriddenRSSNsInRSName[key]
                                    }
                                    $scope.scopeModel.overriddenRSSNsInRSName.push(overriddenRSSNsInRSNameItem);
                                }
                            }
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    };
                }
                api.getData = function () {
                    var overriddenRSSNsInRSName = {};
                    if ($scope.scopeModel.overriddenRSSNsInRSName != undefined && $scope.scopeModel.overriddenRSSNsInRSName.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.overriddenRSSNsInRSName.length; i++) {
                            var dataItem = $scope.scopeModel.overriddenRSSNsInRSName[i];
                            overriddenRSSNsInRSName[dataItem.exisitingRSSN] = dataItem.overriddenRSSN;
                        };
                    }

                    return {
                        OverriddenRSSNsInRSName: overriddenRSSNsInRSName
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

            }
        }
    }
    app.directive('whsRoutesyncHuaweiSettings', whsRoutesyncHuaweiSettings);
})(app);