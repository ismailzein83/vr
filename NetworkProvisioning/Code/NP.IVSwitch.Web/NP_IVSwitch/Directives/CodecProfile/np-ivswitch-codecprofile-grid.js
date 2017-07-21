'use strict';

app.directive('npIvswitchCodecprofileGrid', ['NP_IVSwitch_CodecProfileAPIService', 'NP_IVSwitch_CodecProfileService', 'VRNotificationService', 'VRUIUtilsService',
    function (NP_IVSwitch_CodecProfileAPIService, NP_IVSwitch_CodecProfileService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var codecProfileGrid = new CodecProfileGrid($scope, ctrl, $attrs);
                codecProfileGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/NP_IVSwitch/Directives/CodecProfile/Templates/CodecProfileGridTemplate.html'
        };

        function CodecProfileGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.codecProfile = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = NP_IVSwitch_CodecProfileService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_CodecProfileAPIService.GetFilteredCodecProfiles(dataRetrievalInput).then(function (response) {
                        if (response !=undefined && response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                          onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onCodecProfileAdded = function (addedCodecProfile) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedCodecProfile);
                    gridAPI.itemAdded(addedCodecProfile);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editCodecProfile,
                    haspermission: hasEditCodecProfilePermission
                });
            }
            function editCodecProfile(codecProfileItem) {
                var onCodecProfileUpdated = function (updatedCodecProfile) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedCodecProfile);
                    gridAPI.itemUpdated(updatedCodecProfile);
                };

                NP_IVSwitch_CodecProfileService.editCodecProfile(codecProfileItem.Entity.CodecProfileId, onCodecProfileUpdated);
            }
            function hasEditCodecProfilePermission() {
                return NP_IVSwitch_CodecProfileAPIService.HasEditCodecProfilePermission();
            }

        }
    }]);
