(function (app) {

    'use strict';

    GroupStaticDirective.$inject = ['VR_Sec_UserAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GroupStaticDirective(VR_Sec_UserAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var groupStatic = new GroupStatic(ctrl, $scope, $attrs);
                groupStatic.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getGroupTemplate(attrs);
            }
        };

        function GroupStatic(ctrl, $scope, attrs) {
            var userSelectorAPI;
            var userSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onUserSelectorReady = function (api) {
                    userSelectorAPI = api;
                    userSelectorReadyPromiseDeferred.resolve();
                    getDirectiveAPI();
                };
            }

            function getDirectiveAPI() {
                var api = {};
                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.Business.StaticGroup, Vanrise.Security.Business",
                        MemberIds: userSelectorAPI.getSelectedIds()
                    };
                };

                api.load = function (payload) {
                    var data;
                    if (payload != undefined )
                        data = payload.settings;

                    var promises = [];

                    var loadUserSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    userSelectorReadyPromiseDeferred.promise.then(function () {
                        var payloadSelector;
                        if (data != undefined) {
                            payloadSelector = {
                                selectedIds:  data.MemberIds 
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(userSelectorAPI, payloadSelector, loadUserSelectorPromiseDeferred);
                    });
                    promises.push(loadUserSelectorPromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                };
                if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }

        function getGroupTemplate(attrs) {           

            return '<vr-sec-user-selector on-ready="onUserSelectorReady" customlabel="Group Members" ismultipleselection isrequired="true"></vr-sec-user-selector>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrSecGroupStatic', GroupStaticDirective)

})(app);
