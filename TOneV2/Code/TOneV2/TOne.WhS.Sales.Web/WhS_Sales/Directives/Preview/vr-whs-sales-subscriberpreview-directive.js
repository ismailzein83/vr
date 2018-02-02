'use strict';
app.directive('vrWhsSalesSubscriberpreviewDirective', ['WhS_Sales_RatePlanPreviewAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_SubscriberStatusEnum',
    function (WhS_Sales_RatePlanPreviewAPIService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SubscriberStatusEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var subscriberpreviewDirective = new SubscriberpreviewDirective($scope, ctrl, $attrs);
                subscriberpreviewDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Sales/Directives/Preview/Templates/SubscriberPreviewTemplate.html'
        };

        function SubscriberpreviewDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var processInstanceId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.subscriberPreviews = [];
                $scope.scopeModel.onGridReady = function (api) {
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    if (query != null)
                        processInstanceId = query.ProcessInstanceId;

                    return WhS_Sales_RatePlanPreviewAPIService.GetSubscriberPreviews(processInstanceId).then(function (response) {
                        if (response != null) {
                            if (response.SubscriberPreviewDetails != null) {
                                for (var i = 0; i < response.SubscriberPreviewDetails.length; i++)
                                    extendDataItem(response.SubscriberPreviewDetails[i]);
                                $scope.scopeModel.subscriberPreviews = response.SubscriberPreviewDetails;
                            }
                            if (response.SubscriberPreviewSummary != null) {
                                $scope.scopeModel.numberOfSubscriberWithSuccessStatus = response.SubscriberPreviewSummary.NumberOfSubscriberWithSuccessStatus;
                                $scope.scopeModel.numberOfSubscriberWithNoChangeStatus = response.SubscriberPreviewSummary.NumberOfSubscriberWithNoChangeStatus;
                                $scope.scopeModel.numberOfSubscriberWithFailedStatus = response.SubscriberPreviewSummary.NumberOfSubscriberWithFailedStatus;
                            }
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                api.hasData = function () {
                    return ($scope.scopeModel.subscriberPreviews.length != 0) ? true : false;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendDataItem(dataItem) {
                var subscriberStatus = UtilsService.getEnum(WhS_BE_SubscriberStatusEnum, 'value', dataItem.Entity.Status);
                if (subscriberStatus != undefined) {
                    dataItem.SubscriberStatusDescription = subscriberStatus.description;
                }
            }
        }
    }]);