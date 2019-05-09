(function (app) {

    'use strict';

    DatarecordstoragePermanentfilterAccountManager.$inject = ['VRUIUtilsService', 'UtilsService'];

    function DatarecordstoragePermanentfilterAccountManager(VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordStoragePermanentFilterAccountManager = new DataRecordStoragePermanentFilterAccountManager($scope, ctrl, $attrs);
                dataRecordStoragePermanentFilterAccountManager.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/AccountManagerAssignment/Templates/DataRecordStoragePermanentFilterAccountManager.html"
        };

        function DataRecordStoragePermanentFilterAccountManager($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
          
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                  
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Business.AccountManagerDataRecordStoragePermanentFilter,TOne.WhS.BusinessEntity.Business",
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }


        }
    }

    app.directive('whsBeDatarecordstoragePermanentfilterAccountmanager', DatarecordstoragePermanentfilterAccountManager);

})(app); 