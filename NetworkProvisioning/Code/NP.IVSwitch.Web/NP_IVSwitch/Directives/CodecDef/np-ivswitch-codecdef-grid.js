'use strict';

app.directive('npIvswitchCodecdefGrid', ['NP_IVSwitch_CodecDefAPIService', 'NP_IVSwitch_CodecDefService', 'VRNotificationService',
    function (NP_IVSwitch_CodecDefAPIService, NP_IVSwitch_CodecDefService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var codecDefGrid = new CodecDefGrid($scope, ctrl, $attrs);
                codecDefGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/NP_IVSwitch/Directives/CodecDef/Templates/CodecDefGridTemplate.html'
        };

        function CodecDefGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.codecDef = [];
                $scope.scopeModel.menuActions = [];
 
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.add = function ()
                {
                    var onCodecDefAdded = function (addedCodecDef) {
                         $scope.scopeModel.codecDef.push({ Entity: addedCodecDef });

                    }
                    NP_IVSwitch_CodecDefService.addCodecDef(onCodecDefAdded);
                }

                ctrl.isGridValid = function() {
                    if ($scope.scopeModel.codecDef.length == 0) {
                        return 'At least one codec must be added.'
                    }
                    return null;
                }

                ctrl.deleterow = function (DeletedItem) {
                    var index = $scope.scopeModel.codecDef.indexOf(DeletedItem); 
                    $scope.scopeModel.codecDef.splice(index, 1);                       
                    
                };                

            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {                 
                    var Data = [];
                    for (var i = 0; i < query.length ; i++) {
                         $scope.scopeModel.codecDef.push({ Entity: query[i] });
                    }                    
                };             
                api.getData = function () {
                    var Data = [];
                    for (var i = 0; i < $scope.scopeModel.codecDef.length ; i++) {
                        Data.push($scope.scopeModel.codecDef[i].Entity);
                    };
                    return Data;
                }              
              
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

           

        }
    }]);
