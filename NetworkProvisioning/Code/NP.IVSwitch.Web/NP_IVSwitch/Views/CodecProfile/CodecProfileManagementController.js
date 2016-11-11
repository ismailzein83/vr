(function (appControllers) {

    "use strict";

    CodecProfileManagementController.$inject = ['$scope', 'NP_IVSwitch_CodecProfileService', 'NP_IVSwitch_CodecProfileAPIService'];
    function CodecProfileManagementController($scope, NP_IVSwitch_CodecProfileService, NP_IVSwitch_CodecProfileAPIService) {

        var gridAPI;

         defineScope();
 
        load();


        function defineScope() {
            $scope.search = function () {
                var query = buildGridQuery();
                 
                return gridAPI.load(query);
            };

            $scope.add = function () {
                var onCodecProfileAdded = function (addedCodecProfile) {
                    gridAPI.onCodecProfileAdded(addedCodecProfile);
                 }
                NP_IVSwitch_CodecProfileService.addCodecProfile(onCodecProfileAdded);
            };

            $scope.hasAddCodecProfilePermission = function () {
                return NP_IVSwitch_CodecProfileAPIService.HasAddCodecProfilePermission()
            }

            $scope.onGridReady = function (api) {
                 gridAPI = api;
                gridAPI.load({});

            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('NP_IVSwitch_CodecProfileManagementController', CodecProfileManagementController);

})(appControllers);