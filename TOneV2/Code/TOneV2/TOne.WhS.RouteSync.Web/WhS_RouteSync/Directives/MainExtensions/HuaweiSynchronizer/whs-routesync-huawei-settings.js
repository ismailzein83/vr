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
                $scope.scopeModel.OverriddenRSSNsInRSName = [];

                $scope.scopeModel.onOverriddenRSSNsGridReady = function (api) {
                    overriddenRSSNAPI = api;
                    overriddenRSSNGridReadyDeferred.resolve();
                };

                $scope.scopeModel.isValid = function () {
                    return;
                    //if ($scope.scopeModel.switchLoggers == undefined || $scope.scopeModel.switchLoggers.length == 0)
                    //    return 'At least one logger should be added';

                    //var oneLoggerIsActive = false;

                    //for (var x = 0; x < $scope.scopeModel.switchLoggers.length; x++) {
                    //    var currentItem = $scope.scopeModel.switchLoggers[x];
                    //    if (currentItem.isActive) {
                    //        oneLoggerIsActive = true;
                    //        break;
                    //    }
                    //}

                    //if (!oneLoggerIsActive)
                    //    return 'At least one logger should be activated';

                    //return;
                };

                $scope.scopeModel.addOverriddenRSSNInRSName = function () {
                    var dataItem = {};
                    $scope.scopeModel.OverriddenRSSNsInRSName.push(dataItem);
                };

                $scope.scopeModel.removeOverriddenRSSNInRSName = function (dataItem) {
                    var index = $scope.scopeModel.OverriddenRSSNsInRSName.indexOf(dataItem);
                    $scope.scopeModel.OverriddenRSSNsInRSName.splice(index, 1);
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
                        console.log("herersa");
                        var overriddenRSSNsInRSName = payload.overriddenRSSNsInRSName;
                        if (overriddenRSSNsInRSName != undefined) {
                            for (var key in overriddenRSSNsInRSName) {
                                if (key != "$type") {
                                    var overriddenRSSNsInRSNameItem = {
                                        ExisitingRSSN: key,
                                        OverriddenRSSN: overriddenRSSNsInRSName[key]
                                    }
                                    $scope.scopeModel.OverriddenRSSNsInRSName.push(overriddenRSSNsInRSNameItem);
                                }
                            }
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    };
                }
                api.getData = function () {
                    var overriddenRSSNsInRSName = {};
                    if ($scope.scopeModel.OverriddenRSSNsInRSName != undefined && $scope.scopeModel.OverriddenRSSNsInRSName.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.OverriddenRSSNsInRSName.length; i++) {
                            var dataItem = $scope.scopeModel.OverriddenRSSNsInRSName[i];
                            overriddenRSSNsInRSName[dataItem.ExisitingRSSN] = dataItem.OverriddenRSSN;
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