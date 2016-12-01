(function (app) {

    'use strict';

    BePackageServicesettingsVoiceInternationalDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function BePackageServicesettingsVoiceInternationalDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var packageServicesettingsVoiceNational = new PackageServicesettingsVoiceNational($scope, ctrl, $attrs);
                packageServicesettingsVoiceNational.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Package/Service/VoiceType/Templates/InterNationalVoiceTypeTemplate.html"

        };
        function PackageServicesettingsVoiceNational($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.selectedZones = [];

                $scope.addZones = function () {
                    var item = {
                        rate: 0,
                        OnCountryZoneDirectiveReady: function (api) {
                            item.directiveAPI = api;
                            var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, undefined, setLoader);
                        }

                    };
                    $scope.selectedZones.push(item);
                };
                $scope.removeZone = function (zone) {
                    $scope.selectedZones.splice($scope.selectedZones.indexOf(zone), 1);
                };

                defineAPI();
            }



            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                        if (payload.voiceType != undefined && payload.voiceType.TargetZones != undefined) {
                            var promises = [];
                            for (var i = 0; i < payload.voiceType.TargetZones.length; i++) {
                                var targetZone = payload.voiceType.TargetZones[i];
                                var item = {
                                    payload: targetZone,
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(item.loadPromiseDeferred.promise);
                                addItemToGrid(item);
                            }
                        }
                        function addItemToGrid(item) {
                            var dataItem = {
                                rate: item.payload.Rate,
                            };
                            var dataItemPayload = { CountryId: item.payload.CountryId, ZonesIds: item.payload.ZonesIds };

                            dataItem.OnCountryZoneDirectiveReady = function (api) {
                                dataItem.directiveAPI = api;
                                item.readyPromiseDeferred.resolve();
                            };
                            item.readyPromiseDeferred.promise
                                .then(function () {
                                    VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, item.loadPromiseDeferred);
                                });

                            $scope.selectedZones.push(dataItem);
                        }
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var targetZones = [];
                    for (var i = 0; i < $scope.selectedZones.length; i++)
                    {
                        var selectedZone = $scope.selectedZones[i];
                        var directiveData = selectedZone.directiveAPI.getData();
                        if (directiveData != undefined)
                        {
                            directiveData.Rate = selectedZone.rate;
                        }
                        targetZones.push(directiveData);
                    }
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.Package.InterNationalVoiceType,Retail.BusinessEntity.MainExtensions",
                        TargetZones: targetZones
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBePackageServicesettingsVoiceInternational', BePackageServicesettingsVoiceInternationalDirective);

})(app);