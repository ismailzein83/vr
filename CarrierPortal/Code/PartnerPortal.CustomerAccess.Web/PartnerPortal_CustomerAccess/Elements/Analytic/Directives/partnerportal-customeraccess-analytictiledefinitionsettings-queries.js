'use strict';

app.directive('partnerportalCustomeraccessAnalytictiledefinitionsettingsQueries', ['UtilsService', 'VRUIUtilsService', 'PartnerPortal_CustomerAccess_AnalyticService',
    function (UtilsService, VRUIUtilsService, PartnerPortal_CustomerAccess_AnalyticService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BankDetailsSettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/Analytic/Directives/Templates/QueriesAnalyticTileDefinitionSettings.html"
        };

        function BankDetailsSettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.orderedMeasures = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one query.";
                };
                ctrl.addQuery = function () {
                    var onAnalyticQueryAdded = function (analyticQueryObj) {
                        ctrl.datasource.push({ Entity: analyticQueryObj });
                        addOrderedMeasures(analyticQueryObj.Measures);
                    };
                    PartnerPortal_CustomerAccess_AnalyticService.addAnalyticQuery(onAnalyticQueryAdded, ctrl.datasource);
                };
                ctrl.removeQuery = function (dataItem) {
                    removeOrderedMeasures(dataItem.Entity.Measures);
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        var allMeasures = [];
                        if (payload.analyticQueries != undefined) {
                            for (var i = 0, length = payload.analyticQueries.length; i < length; i++) {
                                var analyticQuery = payload.analyticQueries[i];
                                ctrl.datasource.push({ Entity: analyticQuery });
                                allMeasures = allMeasures.concat(analyticQuery.Measures);
                            }
                        }
                        if(payload.orderedMeasuresIds != undefined)
                        {
                            for (var i = 0, length = payload.orderedMeasuresIds.length; i < length; i++) {
                                var orderedMeasureId = payload.orderedMeasuresIds[i];
                                addOrderedMeasures(allMeasures, orderedMeasureId);
                            }
                        }
                       
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {

                    var analyticQueries;
                    if (ctrl.datasource != undefined) {
                        analyticQueries = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            analyticQueries.push(currentItem.Entity);
                        }
                    }
                    var orderedMeasuresIds = [];
                    if (ctrl.orderedMeasures != undefined) {
                        orderedMeasuresIds = [];
                        for (var i = 0; i < ctrl.orderedMeasures.length; i++) {
                            var orderedMeasure = ctrl.orderedMeasures[i];
                            orderedMeasuresIds.push(orderedMeasure.Entity.MeasureItemId);
                        }
                    }
                    return {
                        Queries: analyticQueries,
                        OrderedMeasureIds: orderedMeasuresIds
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }

            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editAnalyticQuery
                }];
                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editAnalyticQuery(analyticQueryObj) {
                var onAnalyticQueryUpdated = function (analyticQuery) {
                    var index = ctrl.datasource.indexOf(analyticQueryObj);
                    ctrl.datasource[index] = { Entity: analyticQuery };

                    removeOrderedMeasures(analyticQueryObj.Entity.Measures);
                    addOrderedMeasures(analyticQuery.Measures);
                };
                PartnerPortal_CustomerAccess_AnalyticService.editAnalyticQuery(analyticQueryObj.Entity, onAnalyticQueryUpdated, ctrl.datasource);
            }

            function removeOrderedMeasures(measures) {
                if (measures != undefined) {
                    for (var i = 0, length = measures.length; i < length; i++) {
                        var measure = measures[i];
                        var index = UtilsService.getItemIndexByVal(ctrl.orderedMeasures, measure.MeasureItemId, "Entity.MeasureItemId");
                        ctrl.orderedMeasures.splice(index, 1);
                    }
                }
            }
            function addOrderedMeasures(measures, orderedMeasureId)
            {
                if (measures != undefined) {
                    if (orderedMeasureId != undefined) {
                        var measure = UtilsService.getItemByVal(measures, orderedMeasureId, "MeasureItemId");
                        if (measure != undefined) {
                            ctrl.orderedMeasures.push({
                                Entity: {
                                    MeasureTitle: measure.MeasureTitle,
                                    MeasureItemId: measure.MeasureItemId,
                                }
                            });
                        }
                    } else {
                        for (var i = 0, length = measures.length; i < length; i++) {
                            var measure = measures[i];
                            ctrl.orderedMeasures.push({
                                Entity: {
                                    MeasureTitle: measure.MeasureTitle,
                                    MeasureItemId: measure.MeasureItemId,
                                }
                            });
                        }
                    }
                }
            }
        }
    }]);