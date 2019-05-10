//'use strict';

//restrict: 'E',
//    app.directive('vrCommonFtpconnectionEditor', ['UtilsService', function (UtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                normalColNum: '@'
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var editor = new FTPConnectionEditor($scope, ctrl, $attrs);
//                editor.initializeController();
//            },
//            controllerAs: 'ftpCtrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/Common/Directives/MainExtensions/ConnectionTypes/Templates/FTPConnectionEditorTemplate.html'
//        };

//        function FTPConnectionEditor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;
//            var ftpCommunicatorSettingDirectiveAPI;
//            var ftpCommunicatorSettingReadyDeferred = UtilsService.createPromiseDeferred();
//            function initializeController() {

//                $scope.scopeModel = {};
               
//                $scope.scopeModel.onFTPCommunicatorSettingDirectiveReady = function (api) {
//                    ftpCommunicatorSettingDirectiveAPI = api;
//                    ftpCommunicatorSettingReadyDeferred.resolve();
//                };

//                defineAPI();
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
                    
//                    ftpCommunicatorSettingReadyDeferred.promise.then(function () {
//                        ftpCommunicatorSettingDirectiveAPI.load({ ftpCommunicatorSettings:(payload && payload.data)? payload.data.FTPCommunicatorSettings:undefined});
//                    });
//                };

//                api.getData = function () {

//                    return {
//                        $type: 'Vanrise.Common.Business.VRFTPConnection, Vanrise.Common.Business',
//                        FTPCommunicatorSettings: ftpCommunicatorSettingDirectiveAPI.getData()
//                    };
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
//                    ctrl.onReady(api);
//            }
//        }
//    }]);


