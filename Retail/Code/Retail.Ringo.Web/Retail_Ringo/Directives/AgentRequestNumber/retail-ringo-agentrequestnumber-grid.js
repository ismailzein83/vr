'use strict';

app.directive('retailRingoAgentrequestnumberGrid', ['Retail_Ringo_AgentNumberRequestAPIService', 'Retail_Ringo_RingoAgentNumberRequestService', 'VR_GenericData_GenericRule', 'UtilsService', 'VRNotificationService', 'Retail_Ringo_AgentNumberRequestStatusEnum', 'Retail_Ringo_AgentNumberStatusEnum',
    function (Retail_Ringo_AgentNumberRequestAPIService, Retail_Ringo_RingoAgentNumberRequestService, VR_GenericData_GenericRule, UtilsService, VRNotificationService, Retail_Ringo_AgentNumberRequestStatusEnum, Retail_Ringo_AgentNumberStatusEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new RingoAgentRequestNumberGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Ringo/Directives/AgentRequestNumber/Templates/AgentRequestNumberGridTemplate.html'
        };

        function RingoAgentRequestNumberGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.agentNumberRequests = [];
                
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_Ringo_AgentNumberRequestAPIService.GetFilteredAgentNumberRequests(dataRetrievalInput).then(function (response) {
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

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    var menuActions = [];
                    if (dataItem.Entity.Status == Retail_Ringo_AgentNumberRequestStatusEnum.Pending.value) {
                        menuActions.push({
                            name: 'Add Rule',
                            clicked: addRule
                        });
                        menuActions.push({
                            name: 'Reject Request',
                            clicked: rejectRequest
                        });
                    }
                    return menuActions;
                };
            }

            function addRule(numberRequest) {

                var preDefinedData = {
                    settings: {
                        Value: numberRequest.Entity.AgentId
                    },
                    criteriaFieldsValues: {
                        Number: {
                            Values: getCriteriaFieldsValues(numberRequest.Entity)
                        }
                    }
                };

                var onNumberRequestAdded = function (addedRule) {
                    for (var i = 0; i < numberRequest.Entity.Settings.AgentNumbers.length; i++) {
                        var agentNumber = numberRequest.Entity.Settings.AgentNumbers[i];
                        if (UtilsService.contains(addedRule.Entity.Criteria.FieldsValues.Number.Values, agentNumber.Number)) {
                            agentNumber.Status = Retail_Ringo_AgentNumberStatusEnum.Accepted.value;
                        }
                        else
                            agentNumber.Status = Retail_Ringo_AgentNumberStatusEnum.Rejected.value;
                    }

                    numberRequest.Entity.Status = Retail_Ringo_AgentNumberRequestStatusEnum.Accepted.value;
                    numberRequest.StatusDescription = Retail_Ringo_AgentNumberRequestStatusEnum.Accepted.description;
                    Retail_Ringo_AgentNumberRequestAPIService.UpdateAgentNumberRequest(numberRequest.Entity).then(function () {
                        gridAPI.itemUpdated(numberRequest);
                    });

                };

                VR_GenericData_GenericRule.addGenericRule('432d290b-374a-4d50-860f-c8810af9a66d', onNumberRequestAdded, preDefinedData);
            }

            function rejectRequest(numberRequest) {
                var onNumberRequestUpdated = function () {
                    gridAPI.itemUpdated(numberRequest);
                };
                numberRequest.Entity.Status = Retail_Ringo_AgentNumberRequestStatusEnum.Rejected.value;
                numberRequest.StatusDescription = Retail_Ringo_AgentNumberRequestStatusEnum.Rejected.description;
                for (var i = 0; i < numberRequest.Entity.Settings.AgentNumbers.length; i++) {
                    numberRequest.Entity.Settings.AgentNumbers[i] = Retail_Ringo_AgentNumberStatusEnum.Rejected.value;
                }
                Retail_Ringo_RingoAgentNumberRequestService.rejectAgentNumberRequest($scope, numberRequest.Entity, onNumberRequestUpdated);
            }

            function hasAddRulePermission() {
                return true;
            }

            function getRulePredefinedData(numberRequest) {
                var preDefinedData = {
                    settings: {
                        Value: numberRequest.Entity.AgentId
                    },
                    criteriaFieldsValues: {
                        Number: {
                            Values: getCriteriaFieldsValues(numberRequest.Entity)
                        }
                    }
                };
                return preDefinedData;
            }

            function getCriteriaFieldsValues(numberRequest) {
                var values = [];
                if (numberRequest != undefined && numberRequest.Settings != undefined && numberRequest.Settings.AgentNumbers != undefined) {
                    for (var i = 0; i < numberRequest.Settings.AgentNumbers.length; i++) {
                        var item = numberRequest.Settings.AgentNumbers[i];
                        values.push(item.Number);
                    }
                }
                return values;
            }
        }
    }]);
