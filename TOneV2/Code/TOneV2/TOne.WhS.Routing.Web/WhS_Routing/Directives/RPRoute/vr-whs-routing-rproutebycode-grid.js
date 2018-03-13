"use strict";

app.directive('vrWhsRoutingRproutebycodeGrid', ['VRNotificationService', 'UISettingsService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RPRouteAPIService', 'WhS_Routing_RouteRuleService', 'WhS_BE_ZoneRouteOptionsEnum', 'WhS_Routing_RouteOptionEvaluatedStatusEnum', 'WhS_Routing_SupplierStatusEnum',
    function (VRNotificationService, UISettingsService, UtilsService, VRUIUtilsService, WhS_Routing_RPRouteAPIService, WhS_Routing_RouteRuleService, WhS_BE_ZoneRouteOptionsEnum, WhS_Routing_RouteOptionEvaluatedStatusEnum, WhS_Routing_SupplierStatusEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new customerRouteGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/RPRouteByCodeGridTemplate.html"
        };

        function customerRouteGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var routingDatabaseId;
            var currencyId;

            var gridAPI;
            var routeOptionEvaluatedStatusEnum = UtilsService.getArrayEnum(WhS_Routing_RouteOptionEvaluatedStatusEnum);
            var longPrecision = UISettingsService.getUIParameterValue('LongPrecision');

            function initializeController() {
                $scope.showGrid = false;
                $scope.rpRoutes = [];
                $scope.isCustomerSelected = false;

                $scope.getColor = function (dataItem) {
                    var cssClass = 'span-summary bold-label';
                    if (dataItem.IsBlocked)
                        cssClass += ' danger-font';

                    return cssClass;
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    var loadGridPromiseDeffered = UtilsService.createPromiseDeferred();

                    WhS_Routing_RPRouteAPIService.GetFilteredRPRoutesByCode(dataRetrievalInput).then(function (response) {
                        var promises = [];
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var rpRouteDetail = response.Data[i];

                                if (rpRouteDetail.RouteOptionsDetails != undefined) {
                                    for (var k = 0; k < rpRouteDetail.RouteOptionsDetails.length; k++) {
                                        var currentOption = rpRouteDetail.RouteOptionsDetails[k];
                                        extendRPRouteOptionObject(currentOption);

                                        if (currentOption.Backups != undefined) {
                                            for (var j = 0; j < currentOption.Backups.length; j++) {
                                                var currentBackup = currentOption.Backups[j];
                                                extendRPRouteOptionObject(currentBackup);
                                            }
                                        }
                                    }
                                }
                                promises.push(setRouteOptionDetailsDirectiveonEachItem(rpRouteDetail));
                            }
                        }
                        onResponseReady(response);
                        UtilsService.waitMultiplePromises(promises).then(function () {
                            $scope.showGrid = true;
                            loadGridPromiseDeffered.resolve();
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                            loadGridPromiseDeffered.reject();
                        });
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                        loadGridPromiseDeffered.reject();
                    });

                    return loadGridPromiseDeffered.promise;
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    routingDatabaseId = query.RoutingDatabaseId;
                    currencyId = query.CurrencyId;

                    if (query.CustomerId != undefined)
                        $scope.isCustomerSelected = true;
                    else
                        $scope.isCustomerSelected = false;

                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{ name: "Matching Rule", clicked: openRouteRuleEditor }];
            }

            function openRouteRuleEditor(dataItem) {
                WhS_Routing_RouteRuleService.viewRouteRule(dataItem.ExecutedRuleId);
            }

            function setRouteOptionDetailsDirectiveonEachItem(rpRouteDetail) {
                var promises = [];

                rpRouteDetail.saleZoneServiceLoadDeferred = UtilsService.createPromiseDeferred();
                rpRouteDetail.onServiceViewerReady = function (api) {
                    rpRouteDetail.serviceViewerAPI = api;

                    var serviceViewerPayload;
                    if (rpRouteDetail != undefined) {
                        serviceViewerPayload = { selectedIds: rpRouteDetail.SaleZoneServiceIds };
                    }
                    VRUIUtilsService.callDirectiveLoad(rpRouteDetail.serviceViewerAPI, serviceViewerPayload, rpRouteDetail.saleZoneServiceLoadDeferred);
                };
                promises.push(rpRouteDetail.saleZoneServiceLoadDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };

            function extendRPRouteOptionObject(rpRouteOption) {
                var roundedConvertedSupplierRate = roundNumber(rpRouteOption.ConvertedSupplierRate);
                rpRouteOption.title = buildTooltip(rpRouteOption, roundedConvertedSupplierRate);
                rpRouteOption.titleToDisplay = buildTitleToDisplay(rpRouteOption, roundedConvertedSupplierRate);

                if (rpRouteOption.ConvertedSupplierRate == 0) {
                    rpRouteOption.ConvertedSupplierRate = 'N/A';
                }
                rpRouteOption.SupplierStatusDescription = UtilsService.getEnumDescription(WhS_Routing_SupplierStatusEnum, rpRouteOption.SupplierStatus);
                rpRouteOption.IsBlocked = rpRouteOption.SupplierStatus == WhS_Routing_SupplierStatusEnum.Block.value;

                var evaluatedStatus = UtilsService.getItemByVal(routeOptionEvaluatedStatusEnum, rpRouteOption.EvaluatedStatus, "value");
                if (evaluatedStatus != undefined) {
                    rpRouteOption.EvaluatedStatusCssClass = evaluatedStatus.cssclass;
                }
            };

            function buildTooltip(routeOption, roundedConvertedSupplierRate) {
                var result = buildTitle(routeOption, roundedConvertedSupplierRate);
                return routeOption.OptionOrder + '. ' + result;
            }

            function buildTitle(routeOption, roundedConvertedSupplierRate) {
                var supplierName = routeOption.SupplierName;
                var percentage = routeOption.Percentage;

                var result = percentage ? percentage + '% ' + supplierName : supplierName;

                if (roundedConvertedSupplierRate) {
                    result = result + ' (' + roundedConvertedSupplierRate + ')';
                }
                return result;
            };

            function buildTitleToDisplay(routeOption, roundedConvertedSupplierRate) {
                if (routeOption.Color == undefined)
                    routeOption.Color = '#616F77';

                var totalWhiteSpaceLength = 30;
                var roundedConvertedSupplierRateLength = roundedConvertedSupplierRate.toString().length + 2; //2 is length of " ()"
                var remainingWhiteSpaceLength = totalWhiteSpaceLength - roundedConvertedSupplierRateLength;

                var title = buildTitle(routeOption);
                if (title.length > remainingWhiteSpaceLength) {
                    title = title.substring(0, remainingWhiteSpaceLength - 2); //2 is length of "..."
                    title += "...";
                }

                return title + ' (' + roundedConvertedSupplierRate + ')';
            };

            function roundNumber(number) {
                var precisionNumber = Math.pow(10, longPrecision);
                return Math.round(number * precisionNumber) / precisionNumber;
            };
        }

        return directiveDefinitionObject;
    }]);
