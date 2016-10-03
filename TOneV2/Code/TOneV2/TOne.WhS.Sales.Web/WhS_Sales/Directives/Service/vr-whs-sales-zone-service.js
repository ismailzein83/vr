'use strict';

app.directive('vrWhsSalesZoneService', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_RatePlanUtilsService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_Sales_RatePlanAPIService, WhS_Sales_RatePlanUtilsService, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var zoneService = new ZoneService(ctrl, $scope);
            zoneService.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Service/Templates/ZoneServiceTemplate.html'
    };

    function ZoneService(ctrl, $scope) {

        this.initializeController = initializeController;

        var zoneItem;
        var settings;
        var oldIds;

        var isFirstSelection = true;
        var areNewServicesSet;

        var currentServiceViewerAPI;
        var currentServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

        var inheritedServiceViewerAPI;
        var inheritedServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();
        var inheritedServiceIds;

        var selectorAPI;
        var selectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onCurrentServiceViewerReady = function (api) {
                currentServiceViewerAPI = api;
                currentServiceViewerReadyDeferred.resolve();
            };

            $scope.scopeModel.onInheritedServiceViewerReady = function (api) {
                inheritedServiceViewerAPI = api;
                inheritedServiceViewerReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                selectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectionChanged = function ()
            {
                if (isFirstSelection) {
                    isFirstSelection = false;
                    return;
                }

                if (areNewServicesSet === false) {
                    areNewServicesSet = true;
                    return;
                }

                zoneItem.IsDirty = true;
                var selectedIds = selectorAPI.getSelectedIds();

                if (selectedIds != undefined) {
                    var newServiceBED = WhS_Sales_RatePlanUtilsService.getNowPlusDays(settings.newServiceDayOffset);
                    setServiceDates(newServiceBED, undefined); // null and undefined don't work!
                }
                else {
                    setServiceDates(zoneItem.CurrentServiceBED, zoneItem.CurrentServiceEED);
                    if (zoneItem.CurrentServices != null)
                        selectedIds = UtilsService.getPropValuesFromArray(zoneItem.CurrentServices, 'ServiceId');
                }

                loadZoneServiceViewer(selectedIds);
            };

            $scope.scopeModel.onSelectorBlurred = function ()
            {
                var selectedIds = selectorAPI.getSelectedIds();

                if (WhS_Sales_RatePlanUtilsService.isSameNewService(selectedIds, oldIds))
                    return;
                
                oldIds = selectedIds;
                zoneItem.context.saveDraft(false);
            };

            $scope.scopeModel.reset = function ()
            {
                zoneItem.IsDirty = true;
                $scope.scopeModel.isLoading = true;

                showLinks(false, true);

                var promises = [];

                var loadInheritedServicesPromise = loadInheritedServiceViewer();
                promises.push(loadInheritedServicesPromise);

                var loadZoneServicesDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadZoneServicesDeferred.promise);

                loadInheritedServicesPromise.then(function () {
                    loadZoneServiceViewer(inheritedServiceIds).then(function () {
                        loadZoneServicesDeferred.resolve();
                    }).catch(function (error) {
                        loadZoneServicesDeferred.reject(error);
                    });
                });

                UtilsService.waitMultiplePromises(promises).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.undo = function ()
            {
                var promises = [];
                $scope.scopeModel.isLoading = true;

                inheritedServiceIds = undefined;
                $scope.scopeModel.showInheritedServiceViewer = false;

                $scope.scopeModel.isSelectorDisabled = false;
                $scope.scopeModel.areDatesDisabled = false;
                setServiceDates(zoneItem.CurrentServiceBED, zoneItem.CurrentServiceEED);

                showLinks(true, false);
                var saveDraftPromise = zoneItem.context.saveDraft(false);
                promises.push(saveDraftPromise);

                var loadZoneServicesDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadZoneServicesDeferred.promise);

                saveDraftPromise.then(function () {
                    var currentServiceIds = zoneItem.CurrentServices != null ? UtilsService.getPropValuesFromArray(zoneItem.CurrentServices, 'ServiceId') : undefined;
                    loadZoneServiceViewer(currentServiceIds).then(function () {
                        loadZoneServicesDeferred.resolve();
                    }).catch(function (error) {
                        loadZoneServicesDeferred.reject(error);
                    });
                });

                UtilsService.waitMultiplePromises(promises).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.onServiceEEDChanged = function () {
                zoneItem.IsDirty = true;
            };

            UtilsService.waitMultiplePromises([currentServiceViewerReadyDeferred.promise, inheritedServiceViewerReadyDeferred.promise, selectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload)
            {
                var promises = [];
                var selectorPayload;

                if (payload != undefined) {
                    zoneItem = payload.zoneItem;
                    settings = payload.settings;
                }
                
                if (zoneItem != undefined) {
                    if (zoneItem.CurrentServices != null) {
                        var currentServiceIds = UtilsService.getPropValuesFromArray(zoneItem.CurrentServices, 'ServiceId');

                        var loadCurrentServiceViewerPromise = currentServiceViewerAPI.load({ selectedIds: currentServiceIds });
                        promises.push(loadCurrentServiceViewerPromise);

                        setServiceDates(zoneItem.CurrentServiceBED, zoneItem.CurrentServiceEED);

                        $scope.scopeModel.showCurrentServiceViewer = true;
                        $scope.scopeModel.isCurrentServiceInherited = zoneItem.IsCurrentServiceEditable == false;
                        showLinks(true, false);
                    }
                    else {
                        $scope.scopeModel.serviceBED = WhS_Sales_RatePlanUtilsService.getNowPlusDays(settings.newServiceDayOffset);
                    }

                    if (zoneItem.ResetService != null) {
                        zoneItem.IsDirty = true;
                        showLinks(false, true);
                        var loadInheritedServiceViewerPromise = loadInheritedServiceViewer();
                        promises.push(loadInheritedServiceViewerPromise);
                    }
                    else if (zoneItem.ClosedService != null) {
                        zoneItem.IsDirty = true;
                        // Note that the user can't undo a closed service. This should be resolved in the draft preview page
                        setServiceDates(zoneItem.CurrentServiceBED, zoneItem.ClosedService.EED);
                    }
                    else if (zoneItem.NewService != null) {
                        zoneItem.IsDirty = true;
                        areNewServicesSet = false;
                        var newServiceIds = UtilsService.getPropValuesFromArray(zoneItem.NewService.Services, 'ServiceId');
                        oldIds = newServiceIds;
                        selectorPayload = {
                            selectedIds: newServiceIds
                        };
                        setServiceDates(zoneItem.NewService.BED, zoneItem.NewService.EED);
                    }
                }

                var loadSelectorPromise = selectorAPI.load(selectorPayload);
                promises.push(loadSelectorPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.applyChanges = function ()
            {
                if (zoneItem.IsDirty)
                    applyChanges();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadInheritedServiceViewer()
        {
            var promises = [];

            var newDraft = zoneItem.context.getNewDraft();
            var input = {
                OwnerType: zoneItem.OwnerType,
                OwnerId: zoneItem.OwnerId,
                ZoneId: zoneItem.ZoneId,
                EffectiveOn: UtilsService.getDateFromDateTime(new Date()),
                NewDraft: newDraft
            };
            var getInheritedServicePromise = WhS_Sales_RatePlanAPIService.GetZoneInheritedService(input);
            promises.push(getInheritedServicePromise);

            var inheritedServicesViewerLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(inheritedServicesViewerLoadDeferred.promise);

            getInheritedServicePromise.then(function (response)
            {
                $scope.scopeModel.showInheritedServiceViewer = true;
                $scope.scopeModel.selectedValues.length = 0;
                $scope.scopeModel.isSelectorDisabled = true;
                
                var selectedIds;
                if (response != null) {
                    selectedIds = UtilsService.getPropValuesFromArray(response.Services, 'ServiceId');
                    $scope.scopeModel.areDatesDisabled = true;
                    setServiceDates(response.BED, response.EED);
                }
                // Update inheritedServiceIds to loadZoneServiceViewer
                if (selectedIds == undefined)
                    inheritedServices = undefined;
                else {
                    inheritedServiceIds = [];
                    for (var i = 0; i < selectedIds.length; i++)
                        inheritedServiceIds.push(selectedIds[i]);
                }
                VRUIUtilsService.callDirectiveLoad(inheritedServiceViewerAPI, { selectedIds: selectedIds }, inheritedServicesViewerLoadDeferred);
            });

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadZoneServiceViewer(serviceIds) {
            var zoneServiceViewerPayload = { selectedIds: serviceIds };
            return zoneItem.serviceViewerAPI.load(zoneServiceViewerPayload);
        }

        function setServiceDates(serviceBED, serviceEED) {
            $scope.scopeModel.serviceBED = serviceBED;
            $scope.scopeModel.serviceEED = serviceEED;
        }
        function showLinks(showResetLink, showUndoLink) {
            $scope.scopeModel.showResetLink = showResetLink;
            $scope.scopeModel.showUndoLink = showUndoLink;
        }

        function applyChanges() {
            zoneItem.NewService = null;
            zoneItem.ClosedService = null;
            zoneItem.ResetService = null;

            if (!trySetDraftNewService())
                if (!trySetDraftResetService())
                    trySetDraftClosedService();
        }
        function trySetDraftNewService()
        {
            var selectedIds = selectorAPI.getSelectedIds();
            if (selectedIds != undefined)
            {
                zoneItem.NewService = {
                    ZoneId: zoneItem.ZoneId,
                    BED: $scope.scopeModel.serviceBED,
                    EED: $scope.scopeModel.serviceEED
                };
                zoneItem.NewService.Services = [];
                for (var i = 0; i < selectedIds.length; i++) {
                    zoneItem.NewService.Services.push({
                        ServiceId: selectedIds[i]
                    });
                }
                return true;
            }
            return false;
        }
        function trySetDraftResetService() {
            if ($scope.scopeModel.showUndoLink) {
                zoneItem.ResetService = {
                    ZoneId: zoneItem.ZoneId,
                    EED: zoneItem.CurrentServiceBED
                };
                return true;
            }
            return false;
        }
        function trySetDraftClosedService()
        {
            if (zoneItem.IsCurrentServiceEditable && $scope.scopeModel.showResetLink && !areDatesEqual(zoneItem.CurrentServiceEED, $scope.scopeModel.serviceEED))
            {
                zoneItem.ClosedService = {
                    ZoneId: zoneItem.ZoneId,
                    EED: $scope.scopeModel.serviceEED
                };
                return true;
            }
            return false;
        }
        function areDatesEqual(date1, date2)
        {
            if (date1 && date2) {
                var d1 = new Date(date1);
                var d2 = new Date(date2);
                return (d1.getDay() == d2.getDay() && d1.getMonth() == d2.getMonth() && d1.getYear() == d2.getYear());
            }
            else if (!date1 && !date2)
                return true;
            else
                return false;
        }
    }
}]);