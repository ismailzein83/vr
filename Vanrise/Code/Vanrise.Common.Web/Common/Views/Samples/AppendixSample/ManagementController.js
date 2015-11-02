(function (appControllers) {

    "use strict";

    ManagementController.$inject = ['$scope', 'VRModalService'];

    function ManagementController($scope, VRModalService) {
        
        defineScope();
        load();

        function defineScope() {
            $scope.testModel = 'test value';

            $scope.addNew = function () {
                VRModalService.showModal('/Client/Modules/Common/Views/Samples/AppendixSample/Editor.html', { edit: false }, {});
            };

            $scope.edit = function () {
                VRModalService.showModal('/Client/Modules/Common/Views/Samples/AppendixSample/Editor.html', { edit: true }, {});
            };
        }

        function load() {

            

        }
    }

    appControllers.controller('Common_AppendixSample_ManagementController', ManagementController);
})(appControllers);