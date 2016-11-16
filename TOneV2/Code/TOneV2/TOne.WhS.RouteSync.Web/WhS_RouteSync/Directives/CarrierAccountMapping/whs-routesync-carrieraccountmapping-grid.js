'use strict';

app.directive('whsRoutesyncCarrieraccountmappingGrid', ['WhS_BE_CarrierAccountAPIService', 'VRNotificationService', 'UtilsService',
    function (WhS_BE_CarrierAccountAPIService, VRNotificationService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                mappingColumns: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var carrierAccountMappingGrid = new CarrierAccountMappingGrid($scope, ctrl, $attrs);
                carrierAccountMappingGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };


        function getTemplate(attrs) {

            return '<vr-datagrid datasource="scopeModel.filterdCarrierMappings" loadmoredata="scopeModel.loadMoreData" maxheight="250px" on-ready="scopeModel.onGridReady">'
                + '<vr-datagridcolumn headertext="\'ID\'" field="\'CarrierId\'" type="\'Text\'" widthfactor="2"></vr-datagridcolumn>'
                + '<vr-datagridcolumn headertext="\'Account Name\'" field="\'CarrierAccountName\'" type="\'Text\'"></vr-datagridcolumn>'
                + '<vr-datagridcolumn ng-repeat="mapping in ctrl.mappingColumns" headertext="mapping.Name" widthfactor="6" field="mapping" columnindex="$index" >'
                + '<vr-label ng-if="colDef.field.Type == \'label\'">{{dataItem[colDef.field.Column]}}</vr-label>'
                + '<vr-textbox  ng-if="colDef.field.Type == \'text\'" type="text" tonextinput value="dataItem[colDef.field.Column]"></vr-textbox>'
                + '</vr-datagridcolumn>'
                + '</vr-datagrid>';
        }

        function CarrierAccountMappingGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var separator = ";";
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.mappingColumns = ctrl.mappingColumns;
                $scope.scopeModel.filterdCarrierMappings = [];
                $scope.scopeModel.carrierAccountMappings = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.loadMoreData = function () {
                    return loadMoreCarrierMappings();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    var promises = [];

                    var loadCarrierMappingPromise = loadCarrierMappings(query);
                    promises.push(loadCarrierMappingPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var result = {};
                    var filteredLength = $scope.scopeModel.filterdCarrierMappings.length;
                    for (var i = 0; i < filteredLength; i++) {
                        result[$scope.scopeModel.filterdCarrierMappings[i].CarrierId] = GetMappingObject($scope.scopeModel.filterdCarrierMappings[i]);
                    }
                    for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
                        var item = $scope.scopeModel.carrierAccountMappings[i];
                        if (UtilsService.getItemIndexByVal($scope.scopeModel.filterdCarrierMappings, item.CarrierId, 'CarrierId') == -1)
                            result[item.CarrierId] = GetMappingObject(item);
                    }
                    return result;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function GetMappingObject(carrierMapping) {
                var obj = {
                    CarrierId: carrierMapping.CarrierId
                };
                for (var j = 0; j < $scope.scopeModel.mappingColumns.length; j++) {
                    var mapping = $scope.scopeModel.mappingColumns[j];
                    obj[mapping['Column']] = carrierMapping[mapping['Column']] != null ? carrierMapping[mapping['Column']].split(separator) : undefined;
                }
                return obj;
            }

            function loadCarrierMappings(query) {
                if (query && query.switchSynchronizerSettings)
                    separator = query.switchSynchronizerSettings.MappingSeparator;
                $scope.scopeModel.isLoading = true;
                var serializedFilter = {};
                return WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serializedFilter)
                 .then(function (response) {

                     if (response) {
                         if (query && query.switchSynchronizerSettings && query.switchSynchronizerSettings.CarrierMappings) {
                             for (var i = 0; i < response.length; i++) {
                                 var carrierAccount = response[i];
                                 var carrierMapping = {
                                     CarrierId: carrierAccount.CarrierAccountId,
                                     CarrierAccountName: carrierAccount.Name
                                 };
                                 for (var j = 0; j < $scope.scopeModel.mappingColumns.length; j++) {
                                     var mapping = $scope.scopeModel.mappingColumns[j];
                                     var accountCarrierMappings = query.switchSynchronizerSettings.CarrierMappings[carrierAccount.CarrierAccountId];

                                     carrierMapping[mapping['Column']] = accountCarrierMappings != undefined && accountCarrierMappings[mapping['Column']] != null ? accountCarrierMappings[mapping['Column']].join(separator) : undefined;
                                 }
                                 $scope.scopeModel.carrierAccountMappings.push(carrierMapping);
                             }
                         }
                         else {
                             for (var i = 0; i < response.length; i++) {
                                 var carrierAccount = response[i];
                                 var carrierMapping = {
                                     CarrierId: carrierAccount.CarrierAccountId,
                                     CarrierAccountName: carrierAccount.Name,
                                 };
                                 for (var j = 0; j < $scope.scopeModel.mappingColumns.length; j++) {
                                     var mapping = $scope.scopeModel.mappingColumns[j];
                                     carrierMapping[mapping['Column']] = '';
                                 }
                                 $scope.scopeModel.carrierAccountMappings.push(carrierMapping);
                             }
                         }
                     }
                     loadMoreCarrierMappings();
                 })
                  .catch(function (error) {
                      VRNotificationService.notifyException(error, $scope);
                      $scope.scopeModel.isLoading = false;
                  }).finally(function () {
                      $scope.scopeModel.isLoading = false;
                  });
            }

            function loadMoreCarrierMappings() {
                var filter = buildFilter();
                var pageInfo = gridAPI.getPageInfo();

                var getRoutingInput = {
                    Filter: filter,
                    FromRow: pageInfo.fromRow,
                    ToRow: pageInfo.toRow
                };

                var items = [];
                var itemsLength = pageInfo.toRow;
                if (pageInfo.toRow > $scope.scopeModel.carrierAccountMappings.length) {
                    if (pageInfo.fromRow < $scope.scopeModel.carrierAccountMappings.length) {
                        itemsLength = $scope.scopeModel.carrierAccountMappings.length;
                    }
                    else
                        return;
                }

                for (var i = pageInfo.fromRow; i < itemsLength; i++) {
                    items.push($scope.scopeModel.carrierAccountMappings[i]);
                }
                gridAPI.addItemsToSource(items);
            }

            function buildFilter() {
                var filter = {};
                return filter;
            }

        }

    }]);
