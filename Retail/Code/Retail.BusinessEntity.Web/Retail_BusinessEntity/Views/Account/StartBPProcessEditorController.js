//(function (appcontrollers) {

//    'use strict';

//    startbpprocesseditorcontroller.$inject = ['$scope', 'utilsservice', 'vruiutilsservice', 'vrnavigationservice', 'vrnotificationservice', 'retail_be_accountbeapiservice', 'retail_be_accountbedefinitionapiservice'];

//    function startbpprocesseditorcontroller($scope, utilsservice, vruiutilsservice, vrnavigationservice, vrnotificationservice, retail_be_accountbeapiservice, retail_be_accountbedefinitionapiservice) {

//        var accountbedefinitionid;
//        var accountid;
//        var accountentity;
//        var accountviewdefinitions;

//        var workflowmanualexeceditorapi;
//        var workflowmanualexeceditorreadypromisedeferred = utilsservice.createpromisedeferred();

//        loadparameters();
//        definescope();
//        load();

//        function loadparameters() {
//            var parameters = vrnavigationservice.getparameters($scope);
//            console.log(parameters);

//            if (parameters != undefined) {
//                accountbedefinitionid = parameters.accountbedefinitionid;
//                accountid = parameters.accountid;
//            }
//        }

//        function definescope() {
//            $scope.scopemodel = {};

//            $scope.scopemodel.onworkflowmanualexeceditorready = function (api) {
//                workflowmanualexeceditorapi = api;
//                workflowmanualexeceditorreadypromisedeferred.resolve();
//            }

//            $scope.scopemodel.startbpprocess = function () {
//                loadworkflowmanualexeceditor().then(function (repsonse) {
//                    if (response != undefined) {
//                        var obj = response;
//                        obj.inputarguments = {
//                            accountidinputfieldname: accountid,
//                            accountbedefinitionidinputfieldname: accountbedefinitionid
//                        };
//                    }
//                });
//            };

//            $scope.scopemodel.close = function () {
//                $scope.modalcontext.closemodal();
//            };
//        }

//        function load() {
//            $scope.scopemodel.isloading = true;

//            loadallcontrols()
//                .catch(function (error) {
//                    vrnotificationservice.notifyexceptionwithclose(error, $scope);
//                })
//                .finally(function () {
//                    $scope.scopemodel.isloading = false;
//                });
//        }

//        function loadallcontrols() {
//            var initialpromises = [];

//            initialpromises.push(getaccount());

//            function getaccount() {
//                return retail_be_accountbeapiservice.getaccount(accountbedefinitionid, accountid).then(function (response) {
//                    accountentity = response;
//                });
//            }

//            function settitle() {
//                $scope.title = (accountentity != undefined) ? "account name: " + accountentity.name : undefined;
//            }

//            var rootpromisenode = {
//                promises: initialpromises,
//                getchildnode: function () {
//                    return {
//                        promises: [utilsservice.waitmultipleasyncoperations([settitle, workflowmanualexeceditorreadypromisedeferred.promise])]
//                    };
//                }
//            };

//            return utilsservice.waitpromisenode(rootpromisenode);
//        }

//        function loadworkflowmanualexeceditor() {
//            var loadworkflowmanualexeceditorpromisedeferred = utilsservice.createpromisedeferred();

//            workflowmanualexeceditorreadypromisedeferred.promise.then(function () {
//                var payload = {};
//                vruiutilsservice.calldirectiveload(workflowmanualexeceditorapi, payload, loadworkflowmanualexeceditorpromisedeferred);
//            });

//            return loadworkflowmanualexeceditorpromisedeferred.promise;
//        }
//    }

//    appcontrollers.controller('retail_be_startbpprocesseditorcontroller', startbpprocesseditorcontroller);

//})(appcontrollers);