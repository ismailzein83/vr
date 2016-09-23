'use strict';

app.directive('vrWhsSalesDefaultService', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_RatePlanUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_Sales_RatePlanAPIService, WhS_Sales_RatePlanUtilsService, WhS_BE_SalePriceListOwnerTypeEnum, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var defaultService = new DefaultService(ctrl, $scope);
            defaultService.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Service/Templates/DefaultServiceTemplate.html'
    };

    function DefaultService(ctrl, $scope) {

        this.initializeController = initializeController;

        var defaultItem;
        var settings;
        var oldIds;

        var currentServiceViewerAPI;
        var currentServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

        var inheritedServiceViewerAPI;
        var inheritedServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorAPI;
        var selectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.renderResetLink = false;

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

            // Note that the onBlurred event doesn't happen when the selector is ready AND when it's programmatically loaded
            $scope.scopeModel.onSelectorBlurred = function () {
                defaultItem.IsDirty = true;
                var selectedIds = selectorAPI.getSelectedIds();

                if (WhS_Sales_RatePlanUtilsService.isSameNewService(selectedIds, oldIds))
                    return;

                oldIds = selectedIds;

                if (selectedIds != undefined) {
                    var newServiceBED = WhS_Sales_RatePlanUtilsService.getNowPlusDays(settings.newServiceDayOffset);
                    setServiceDates(newServiceBED, undefined); // null and undefined don't work!
                }
                else {
                    setServiceDates(defaultItem.CurrentServiceBED, defaultItem.CurrentServiceEED);
                }
                defaultItem.onChange();
            };

            $scope.scopeModel.reset = function () {
                defaultItem.IsDirty = true;
                $scope.scopeModel.isLoading = true;
                loadInheritedServiceViewer().catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.undo = function () {
                $scope.scopeModel.showInheritedServiceViewer = false;
                $scope.scopeModel.isSelectorDisabled = false;
                $scope.scopeModel.areDatesDisabled = false;
                setServiceDates(defaultItem.CurrentServiceBED, defaultItem.CurrentServiceEED);
                showLinks(true, false);
            };

            $scope.scopeModel.onServiceEEDChanged = function () {
                defaultItem.IsDirty = true;
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
                    defaultItem = payload.defaultItem;
                    settings = payload.settings;
                }
                
                if (defaultItem != undefined)
                {
                    $scope.scopeModel.renderResetLink = defaultItem.OwnerType === WhS_BE_SalePriceListOwnerTypeEnum.Customer.value;

                    if (defaultItem.CurrentServices != null) {
                        var currentServiceIds = UtilsService.getPropValuesFromArray(defaultItem.CurrentServices, 'ServiceId');

                        var loadCurrentServiceViewerPromise = currentServiceViewerAPI.load({ selectedIds: currentServiceIds });
                        promises.push(loadCurrentServiceViewerPromise);

                        setServiceDates(defaultItem.CurrentServiceBED, defaultItem.CurrentServiceEED);

                        $scope.scopeModel.showCurrentServiceViewer = true;
                        $scope.scopeModel.isCurrentServiceInherited = defaultItem.IsCurrentServiceEditable == false;
                        showLinks(true, false);
                    }
                    else {
                        $scope.scopeModel.serviceBED = WhS_Sales_RatePlanUtilsService.getNowPlusDays(settings.newServiceDayOffset);
                    }

                    if (defaultItem.ResetService != null) {
                        defaultItem.IsDirty = true;
                        var loadInheritedServiceViewerPromise = loadInheritedServiceViewer();
                        promises.push(loadInheritedServiceViewerPromise);
                    }
                    else if (defaultItem.ClosedService != null) {
                        defaultItem.IsDirty = true;
                        // Note that the user can't undo a closed service. This should be resolved in the draft preview page
                        setServiceDates(defaultItem.CurrentServiceBED, defaultItem.ClosedService.EED);
                    }
                    else if (defaultItem.NewService != null) {
                        defaultItem.IsDirty = true;
                        var newServiceIds = UtilsService.getPropValuesFromArray(defaultItem.NewService.Services, 'ServiceId');
                        oldIds = newServiceIds;
                        selectorPayload = {
                            selectedIds: newServiceIds
                        };
                        setServiceDates(defaultItem.NewService.BED, defaultItem.NewService.EED);
                    }
                }

                var loadSelectorPromise = selectorAPI.load(selectorPayload);
                promises.push(loadSelectorPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.applyChanges = function () {
                if (defaultItem.IsDirty)
                    applyChanges();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadInheritedServiceViewer() {
            var promises = [];

            var inheritedServicesViewerLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(inheritedServicesViewerLoadDeferred.promise);

            var effectiveOn = UtilsService.getDateFromDateTime(new Date());
            var getInheritedServicePromise = WhS_Sales_RatePlanAPIService.GetInheritedService(defaultItem.OwnerType, defaultItem.OwnerId, effectiveOn, defaultItem.ZoneId);
            promises.push(getInheritedServicePromise);

            getInheritedServicePromise.then(function (response) {
                $scope.scopeModel.showInheritedServiceViewer = true;
                showLinks(false, true);
                $scope.scopeModel.selectedValues.length = 0;
                $scope.scopeModel.isSelectorDisabled = true;

                var selectedIds;
                if (response != null) {
                    selectedIds = UtilsService.getPropValuesFromArray(response.Services, 'ServiceId');
                    $scope.scopeModel.areDatesDisabled = true;
                    setServiceDates(response.BED, response.EED);
                }
                VRUIUtilsService.callDirectiveLoad(inheritedServiceViewerAPI, { selectedIds: selectedIds }, inheritedServicesViewerLoadDeferred);
            });

            return UtilsService.waitMultiplePromises(promises);
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
            defaultItem.NewService = null;
            defaultItem.ClosedService = null;
            defaultItem.ResetService = null;

            if (!trySetDraftNewService())
                if (!trySetDraftResetService())
                    trySetDraftClosedService();
        }
        function trySetDraftNewService() {
            var selectedIds = selectorAPI.getSelectedIds();
            if (selectedIds != undefined) {
                defaultItem.NewService = {
                    ZoneId: defaultItem.ZoneId,
                    BED: $scope.scopeModel.serviceBED,
                    EED: $scope.scopeModel.serviceEED
                };
                defaultItem.NewService.Services = [];
                for (var i = 0; i < selectedIds.length; i++) {
                    defaultItem.NewService.Services.push({
                        ServiceId: selectedIds[i]
                    });
                }
                return true;
            }
            return false;
        }
        function trySetDraftResetService() {
            if ($scope.scopeModel.showUndoLink) {
                defaultItem.ResetService = {
                    ZoneId: defaultItem.ZoneId,
                    EED: defaultItem.CurrentServiceBED
                };
                return true;
            }
            return false;
        }
        function trySetDraftClosedService()
        {
            if (defaultItem.IsCurrentServiceEditable && !areDatesEqual(defaultItem.CurrentServiceEED, $scope.scopeModel.serviceEED)) {
                defaultItem.ClosedService = {
                    EED: $scope.scopeModel.serviceEED
                };
                return true;
            }
            return false;
        }
        function areDatesEqual(date1, date2) {
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
