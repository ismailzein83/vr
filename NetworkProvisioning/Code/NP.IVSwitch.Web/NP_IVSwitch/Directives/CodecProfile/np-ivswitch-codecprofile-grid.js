'use strict';

app.directive('npIvswitchCodecprofileGrid', ['NP_IVSwitch_CodecProfileAPIService', 'NP_IVSwitch_CodecProfileService', 'VRNotificationService',
    function (NP_IVSwitch_CodecProfileAPIService, NP_IVSwitch_CodecProfileService, VRNotificationService) {
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

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.codecProfile = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_CodecProfileAPIService.GetFilteredCodecProfiles(dataRetrievalInput).then(function (response) {
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
                    gridAPI.itemAdded(addedCodecProfile);
                }

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
                    gridAPI.itemUpdated(updatedCodecProfile);
                };

                NP_IVSwitch_CodecProfileService.editCodecProfile(codecProfileItem.Entity.CodecProfileId, onCodecProfileUpdated);
            }
            function hasEditCodecProfilePermission() {
                return NP_IVSwitch_CodecProfileAPIService.HasEditCodecProfilePermission();
            }

        }
    }]);
