var app = angular.module("Grid", []);

app.controller("page1Controller", ["$scope", "$http", "$q", function ($scope, $http, $q) {
    $scope.entity = 0;
    $scope.longestarray = 0;
    var unilongestarray = 0;
    var unielements = [];
    var collageelements = [];
    var collagelongestarray = 0;
    var collength;
    var arrayelements = [];
    var AElength;
    var unilength;

    $scope.isEditMode = false;
    $scope.Add = function () {
        $scope.showPage1Editor = true;
        $scope.isEditMode = false;
        $scope.name = "";
        $scope.age = "";

    };

    $scope.Edit = function (a) {
        $scope.showPage1Editor = true;
        $scope.isEditMode = true;

        $scope.name = a.name;
        $scope.age = a.age;
        $scope.entity = a;

    };

    WaitMultiplePromises = function (deferarray1) {
        var tester = 0;
        var waitdefer = $q.defer();
        var deferlength = deferarray1.length;
        for (var i = 0; i < deferlength; i++) {
            var d = deferarray1[i];
            d.then(function (response) {
                tester++;
                if (tester === deferlength) waitdefer.resolve();
            }); d.catch(function (error) {
                waitdefer.reject();
            });

        } return waitdefer.promise;
    };


    var rowpromises = [];

    function fillloadpromises(element) {
        element.clasReadyPromiseDefer = $q.defer();
        element.MajorReadyPromiseDefer = $q.defer();
        element.UniversityReadyPromiseDefer = $q.defer();
        element.CollageReadyPromiseDefer = $q.defer();
        element.clasonDirectiveReady = function (classapi) {
            element.directiveapiclass = classapi;
            element.clasReadyPromiseDefer.resolve();
        }; 
        element.MajoronDirectiveReady = function (majorapi) {
            element.directiveapimajor = majorapi;
            element.MajorReadyPromiseDefer.resolve();
        }; 
        element.UniversityonDirectiveReady = function (universityapi) {
            element.directiveapiuniversity = universityapi;
            element.UniversityReadyPromiseDefer.resolve();
        }; 
        element.CollageonDirectiveReady = function (collageapi) {
            element.directiveapicollage = collageapi;
            element.CollageReadyPromiseDefer.resolve();
        }; 
        element.OnUniversityChange = function () {
            var data = element.directiveapiuniversity;  
            if (data.getdata() != undefined) {
                if (element.universityselectedPromise != undefined) {
                    element.universityselectedPromise.resolve();
                } else {
                    if (element.directiveapiclass != undefined)
                        element.directiveapiclass.clear();
                    if (element.directiveapimajor != undefined) element.directiveapimajor.clear();
                    var collagepayload = {};
                    collagepayload.search = element.directiveapiuniversity.getdata();
                    loadCollageDirective(collagepayload,element);
                }
            }
        };
        element.OnCollageChange = function () {
            var data = element.directiveapicollage;
            if (data.getdata() != undefined) {
                if (element.collageselectedPromise != undefined) {
                    element.collageselectedPromise.resolve();
                } else {
                    var classpayload = {};
                    classpayload.collageid = element.directiveapicollage.getdata();
                    loadClasDirective(classpayload, element);
                    var majorpayload = {};
                    majorpayload.collageid = element.directiveapicollage.getdata();
                    loadMMajorDirective(majorpayload, element);
                }
            };
        };
        rowpromises.push(Load(element));
    };



    $scope.Searchf = function () {
        var loader = true;
        rowpromises.length = 0;
        var searchValue = $scope.search === undefined ? "" : $scope.search;
        $http({
            method: "GET",
            url: 'api/Student/GetFiltered?name=' + searchValue
        }).then(function mySuccess(response) {
            arrayelements.length = 0;
            arrayelements = response.data.student;
            if ($scope.longestarray < arrayelements.length) $scope.longestarray = arrayelements.length;
            unielements.length = 0;
            unielements = response.data.university;
            collageelements.length = 0;
            collageelements = response.data.collage;

            AElength = arrayelements.length;
            unilength = unielements.length;
            collength = collageelements.length;


            $scope.universityinfo = [];
            var trial = {};
            for (i = 0; i < unilength; i++) {
                trial = unielements[i];
                $scope.universityinfo.push(trial);
            }
            $scope.collageinfo = [];
            var ctrial;
            for (i = 0; i < collength; i++) {
                ctrial = collageelements[i];
                $scope.collageinfo.push(ctrial);
            }

            $scope.grid = [];

            for (j = 0; j < AElength; j++) {
                trial = arrayelements[j];
                if (trial !== undefined) {
                    for (m = 0; m < unilength; m++) {
                        var triall = $scope.universityinfo[m];
                        if (trial.iduni === triall.id) {
                            trial.uniname = triall.name; break;
                        }

                    }
                    for (p = 0; p < collength; p++) {
                        var trialll = $scope.collageinfo[p];
                        if (trial.idcollage === trialll.id) {
                            trial.collagename = trialll.faculty; break;
                        }
                    }
                }
                fillloadpromises(trial);
                $scope.grid.push(trial);
            }
            WaitMultiplePromises(rowpromises).then(function (response) {
                loader = false; console.log("All Finished");
            });
        });

    };
    function Load(element) {
        var promisesarray = [];
        var lloader = true;
        var rowpromise = $q.defer();
        var payload = {};
        payload.onScope = element.iduni;
        element.universityselectedPromise = $q.defer();
        var promises = [];
        promises.push(loadUniversityDirective(payload,element));
        var collagepayload = {};
        collagepayload.update = element.idcollage;
        collagepayload.search = payload.onScope;
        element.collageselectedPromise = $q.defer();
        promises.push(loadCollageDirective(collagepayload,element));
        var classpayload = {};
        classpayload.collageid = element.idcollage;
        classpayload.class = element.idclass;
        var majorpayload = {};
        majorpayload.collageid = element.idcollage;
        majorpayload.major = element.idmajor;
        promises.push(loadClasDirective(classpayload,element));
        promises.push(loadMMajorDirective(majorpayload,element));
        WaitMultiplePromises(promises).then(function () {
            lloader = false; console.log(element.id); console.log("row finished");
            rowpromise.resolve();
        });
        return rowpromise.promise;

    };
    function loadClasDirective(payload,element) {

        var promises = [];
        var clasLoadPromiseDefer = $q.defer();
        if (element.collageselectedPromise != undefined)
            promises.push(element.collageselectedPromise.promise);
        promises.push(element.clasReadyPromiseDefer.promise);

        WaitMultiplePromises(promises).then(function () {
            element.directiveapiclass.load(payload).then(function () {
                clasLoadPromiseDefer.resolve();
            });
            element.collageselectedPromise = undefined;
        });
        return clasLoadPromiseDefer.promise;

    };
    function loadMMajorDirective(payload, element) {

        var promises = [];
        var MajorLoadPromiseDefer = $q.defer();
        if (element.collageselectedPromise != undefined)
            promises.push(element.collageselectedPromise.promise);
        promises.push(element.MajorReadyPromiseDefer.promise);

        WaitMultiplePromises(promises).then(function () {
            element.directiveapimajor.load(payload).then(function () {
                MajorLoadPromiseDefer.resolve();
            });
            element.collageselectedPromise = undefined;
        });
        return MajorLoadPromiseDefer.promise;

    };
    function loadUniversityDirective(payload, element) {

        var UniversityLoadPromiseDefer = $q.defer();
        element.UniversityReadyPromiseDefer.promise.then(function () {

            element.directiveapiuniversity.load(payload).then(function () {
                UniversityLoadPromiseDefer.resolve();
            });
        });
        return UniversityLoadPromiseDefer.promise;

    };
    function loadCollageDirective(payload, element) {

        var promises = [];
        var CollageLoadPromiseDefer = $q.defer();
        if (element.universityselectedPromise != undefined)
            promises.push(element.universityselectedPromise.promise);
        promises.push(element.CollageReadyPromiseDefer.promise);

        WaitMultiplePromises(promises).then(function () {
            element.directiveapicollage.load(payload).then(function () {
                CollageLoadPromiseDefer.resolve();
            });
            element.universityselectedPromise = undefined;
        });
        return CollageLoadPromiseDefer.promise;

    };

    $scope.Searchf();

}]);

app.controller("page2Controller", ["$scope", "$http", "$q", function ($scope, $http, $q) {

    $scope.isUniversityEditMode = false;
    var unientity;
    DirectivesReady = function (element) {
        element.DrillDownonDirectiveReady = function (drilldownapi) {
            element.drilldowndirectiveapi = drilldownapi;
            element.drilldownReadyPromiseDefer.resolve(); 
        };
    }
    $scope.DrillDownCollages = function (x) {
        x.isdrilldown = !x.isdrilldown;
        x.drilldownReadyPromiseDefer =$q.defer();
        var universityID = x.id;
        if (x.isdrilldown == true) {
            x.drilldownReadyPromiseDefer.promise.then(function (response) {
                x.drilldowndirectiveapi.load(universityID);
            });
        }

    }  


    $scope.ManageUniversities = function () {

        $scope.showUniversityEditor = true;
    }


    $scope.UEdit = function (a) {

        $scope.euniname = a.name;
        $scope.eunidesc = a.description;
        unientity = a;
    };
    $scope.UAdd = function () {


        var addedobject;

        var uniarraylength = ++unilongestarray;
        addedobject = { id: uniarraylength, name: $scope.auniname, description: $scope.aunidesc };

        $http({
            method: "POST",
            url: 'api/Student/UniAddStudent',
            data: addedobject

        }).then(function mySuccess(response) {
            $scope.USearchf();
            $scope.auniname = "";
            $scope.aunidesc = "";
        });

    };

    $scope.USave = function () {

        var updatedobject = {
            id: unientity.id, name: $scope.euniname, description: $scope.eunidesc
        };
        $http({
            method: "POST",
            url: 'api/Student/UniUpdateList',
            data: updatedobject
        }).then(function mySuccess(response) {
            $scope.USearchf();
        });


    };
    var unidata = [];


    var unilongestarray = 0;

    $scope.USearchf = function () {

        var unisearchValue = $scope.unisearch === undefined ? "" : $scope.unisearch;
        $http({
            method: "GET",
            url: 'api/Student/UniGetFiltered?uniname=' + unisearchValue
        }).then(function mySuccess(response) {
            unidata.length = 0;
            unidata = response.data;
            if (unilongestarray < unidata.length) unilongestarray = unidata.length;

            var unidatalength = unidata.length;
            $scope.universitydata = [];
            var trial;
            for (i = 0; i < unidatalength; i++) {
                trial = unidata[i];
                trial.isdrilldown = false;
                DirectivesReady(trial);
                $scope.universitydata.push(trial);
            }

        });
    };

    $scope.USearchf();


}]);

app.controller("page3Controller", ["$scope", "$http", "$q", function ($scope, $http, $q) {
    var colentity;
    $scope.CEdit = function (a) {

        $scope.ecollagename = a.faculty;
        $scope.ecoluni = a.uniID;
        colentity = a;
    };
    var cuniv
    $scope.CAdd = function () {
        var unilen = unielements.length;

        for (i = 0; i < unilen; i++) {
            var test = unielements[i].name;


            if (test === $scope.acoluni) {
                cuniv = unielements[i].id; break;
            }

        }




        var addedobject;

        var uniarraylength = ++collagelongestarray;
        addedobject = { id: collagelongestarray, faculty: $scope.acollagename, uniId: cuniv };

        $http({
            method: "POST",
            url: 'api/Student/CollageAddStudent',
            data: addedobject

        }).then(function mySuccess(response) {
            $scope.CSearchf();
            $scope.acollagename = "";

        });

    };
    var ceuniv;
    var longestarray = 0;
    var unilongestarray = 0;
    var unielements = [];
    var collageelements = [];
    var collagelongestarray = 0;
    var collength;
    var arrayelements = [];
    var AElength;
    var unilength;

    $scope.Searchf = function () {


        var searchValue = $scope.search === undefined ? "" : $scope.search;
        $http({
            method: "GET",
            url: 'api/Student/GetFiltered?name=' + searchValue
        }).then(function mySuccess(response) {
            arrayelements.length = 0;
            arrayelements = response.data.student;
            if (longestarray < arrayelements.length) longestarray = arrayelements.length;
            unielements.length = 0;
            unielements = response.data.university;
            collageelements.length = 0;
            collageelements = response.data.collage;

            AElength = arrayelements.length;
            unilength = unielements.length;
            collength = collageelements.length;

            $scope.universityinfo = [];
            var trial;
            for (i = 0; i < unilength; i++) {
                trial = unielements[i];
                $scope.universityinfo.push(trial);
            }
            $scope.collageinfo = [];
            var ctrial;
            for (i = 0; i < collength; i++) {
                ctrial = collageelements[i];
                $scope.collageinfo.push(ctrial);
            }

            $scope.grid = [];

            for (j = 0; j < AElength; j++) {
                trial = arrayelements[j];
                if (trial !== undefined) {
                    for (m = 0; m < unilength; m++) {
                        var triall = $scope.universityinfo[m];
                        if (trial.iduni === triall.id) {
                            trial.iduni = triall.name; break;
                        }

                    }
                    for (p = 0; p < collength; p++) {
                        var trialll = $scope.collageinfo[p];
                        if (trial.idcollage === trialll.id) {
                            trial.idcollage = trialll.faculty; break;
                        }
                    }

                    $scope.grid.push(trial);
                }
            } 

        });
    };
    $scope.Searchf();
    $scope.CSave = function () {

        var unilen = unielements.length;

        ceuniv = "";


        for (i = 0; i < unilen; i++) {
            if (unielements[i].name === $scope.ecoluni) {
                ceuniv = unielements[i].id; break;
            }
        }

        var updatedobject = {
            id: colentity.id, faculty: $scope.ecollagename, uniID: ceuniv
        };
        $http({
            method: "POST",
            url: 'api/Student/CollageUpdateList',
            data: updatedobject
        }).then(function mySuccess(response) {
            $scope.CSearchf();
        });


    };
    var coldata = [];
    var unicolelements = [];
    var unicollength;
    var collagelongestarray = 0;
    var universityDirectiveApi;
    var universityReadyPromiseDefer = $q.defer();
    $scope.onUniversityDirectiveReady = function (api) {console.log("filtercollage");
        universityDirectiveApi = api;
        universityReadyPromiseDefer.resolve();
    }
    universityReadyPromiseDefer.promise.then(function (response) {
        var payload = {};
        universityDirectiveApi.load(payload);
        $scope.CSearchf();

    });

    $scope.CSearchf = function () {

        var selectedUniversityId = universityDirectiveApi.getdata();
        if (selectedUniversityId=== undefined)
            selectedUniversityId = -1;

        var csearchValue = $scope.collagesearch === undefined ? "" : $scope.collagesearch;
        $http({
            method: "GET",
            url: 'api/Student/ColGetFiltered?cname='+ csearchValue + '&universityId=' + selectedUniversityId
        }).then(function mySuccess(response) {
            coldata.length = 0;
            coldata = response.data.collage;
            unicolelements.length = 0;
            unicolelements = response.data.university;
            unicollength = unicolelements.length;
            var unicolinfo = [];
            var ttrial;
            for (t = 0; t < unicollength; t++) {
                ttrial = unicolelements[t];
                unicolinfo.push(ttrial);
            }


            if (collagelongestarray < coldata.length) collagelongestarray = coldata.length;
            var collagedatalength = coldata.length;
            $scope.collagedata = [];
            var trial;
            for (i = 0; i < collagedatalength; i++) {
                trial = coldata[i];

                for (r = 0; r < unicollength; r++) {
                    var triall = unicolinfo[r];
                    if (trial.uniID === triall.id) {
                        trial.uniID = triall.name; break;
                    }

                }



                $scope.collagedata.push(trial);
            }

        });
    };


}]);

app.controller("page1EditorController", ["$scope", "$http", "$q", function ($scope, $http, $q) {


    WaitMultiplePromises = function (deferarray1) {
        var tester = 0;
        var waitdefer = $q.defer();
        var deferlength = deferarray1.length;
        for (var i = 0; i < deferlength; i++) {
            var d = deferarray1[i];
            d.then(function (response) {
                tester++;
                if (tester === deferlength) waitdefer.resolve();
            }); d.catch(function (error) {
                waitdefer.reject();
            });

        } return waitdefer.promise;
    };


    $scope.paymentmethods = [];
    var cashpaymentmethod = { configid:1,method: "cash", directive: "cash-payment-method-directive" }
    var checkpaymentmethod = { configid:2,method: "check", directive: "check-payment-method-directive" }
    $scope.paymentmethods.push(cashpaymentmethod);
    $scope.paymentmethods.push(checkpaymentmethod);
    if ($scope.isEditMode) {
        var configid = $scope.entity.PaymentMethod.ConfigurationID; 

        for (w = 0; w < $scope.paymentmethods.length; w++) {

            if ($scope.paymentmethods[w].configid === configid) {

                $scope.paymentmethodselect = $scope.paymentmethods[w]; break;
            }
        }
    }

    var universityReadyPromiseDefer = $q.defer();
    var collageReadyPromiseDefer = $q.defer();
    var classReadyPromiseDefer = $q.defer();
    var majorReadyPromiseDefer = $q.defer();
    var PaymentReadyPromiseDefer = $q.defer();

    var selectedPromise;
    var selectedallPromise;

    var directiveapi;
    var cdirectiveapi;
    var classdirectiveapi; 
    var majordirectiveapi;
    var paymentdirectiveapi;


    function defineScope() {
        $scope.onDirectiveReady = function (api) {
            directiveapi = api;
            universityReadyPromiseDefer.resolve();

        };

        $scope.conDirectiveReady = function (api) {
            cdirectiveapi = api;
            collageReadyPromiseDefer.resolve();
        };

        $scope.classonDirectiveReady = function (classapi) {
            classdirectiveapi = classapi;
            var classpayload = {};
            classReadyPromiseDefer.resolve();
        };

        $scope.majoronDirectiveReady = function (majorapi) {
            majordirectiveapi = majorapi;
  
            majorReadyPromiseDefer.resolve();
        };

        $scope.paymentonDirectiveReady = function (paymentapi) {
            paymentdirectiveapi = paymentapi;

            PaymentReadyPromiseDefer.resolve();
        };
        $scope.Directive = function () {
            if ($scope.paymentmethodselect != undefined)
                return $scope.paymentmethodselect.directive;
            else return null;
        }

        $scope.checkpaymentonDirectiveReady = function (checkapi) {
            checkpaymentdirectiveapi = cashapi;

            checkPaymentdirectiveapi.resolve();
        };
        $scope.onUniversityChanged = function (value) {
  
            var data = directiveapi.getdata();
            if (data != undefined) {
                if (selectedPromise != undefined) {
                    selectedPromise.resolve();
                } else {
                    if (classdirectiveapi!=undefined)
                        classdirectiveapi.clear();
                    if (majordirectiveapi != undefined) majordirectiveapi.clear();
    
                    var collagepayload = {};
                    collagepayload.search = directiveapi.getdata();
                    loadCollageDirective(collagepayload);
                }
            }
        }; 
        $scope.onCollageChanged = function () {

            var data = cdirectiveapi.getdata(); 
            if (data != undefined) {

                if (selectedallPromise != undefined) {
                    selectedallPromise.resolve();
                } else {
                    var classpayload = {};
                    classpayload.collageid = cdirectiveapi.getdata();
                    loadClassDirective(classpayload);
                    var majorpayload = {};
                    majorpayload.collageid = cdirectiveapi.getdata();
                    loadMajorDirective(majorpayload);
                }
            }
  
        };
 

        $scope.Close = function () {
            $scope.$parent.$parent.showPage1Editor = false;
        };

        $scope.Save = function () {

            if (!$scope.$parent.isEditMode) {

                var getuniversity = directiveapi.getdata();
                var getcollage = cdirectiveapi.getdata();
                var getmajor = majordirectiveapi.getdata();
                var getclass = classdirectiveapi.getdata();
                var getpaymentmethod = paymentdirectiveapi.getdata();
                addedobject = { id: ++$scope.$parent.longestarray, name: $scope.name, age: $scope.age, iduni: getuniversity, idcollage: getcollage, idmajor: getmajor, idclass: getclass, PaymentMethod: getpaymentmethod};
                $http({
                    method: "POST",
                    url: 'api/Student/AddStudent',
                    data: addedobject

                }).then(function mySuccess(response) {
                    $scope.Searchf();

                });

            } else {
                var getuniversity = directiveapi.getdata();
                var getcollage = cdirectiveapi.getdata();
                var getmajor = majordirectiveapi.getdata();
                var getclass = classdirectiveapi.getdata();
                var getpaymentmethod = paymentdirectiveapi.getdata();
                $scope.entity.method = $scope.paymentmethodselect.method;
                var updatedobject = {
                    id: $scope.entity.id, name: $scope.name, age: $scope.age, iduni: getuniversity, idcollage: getcollage, idmajor: getmajor, idclass: getclass, PaymentMethod: getpaymentmethod
                };
                $http({
                    method: "POST",
                    url: 'api/Student/UpdateList',
                    data: updatedobject
                }).then(function mySuccess(response) {
                    $scope.Searchf();
                });

            };

            $scope.$parent.$parent.showPage1Editor = false;

        };
    };
    function load() {
        var promisesarray = [];
        var loader = true;
        if ($scope.isEditMode) {
            var payload = {};
            payload.onScope = $scope.entity.iduni;
            selectedPromise = $q.defer();
            var promises = [];
            promises.push(loadUniversityDirective(payload));
            var collagepayload = {};
            collagepayload.update = $scope.entity.idcollage;
            collagepayload.search = payload.onScope;
            selectedallPromise = $q.defer();
            promises.push(loadCollageDirective(collagepayload));
            var classpayload = {};
            classpayload.collageid = $scope.entity.idcollage;
            classpayload.class = $scope.entity.idclass;
            var majorpayload = {};
            majorpayload.collageid = $scope.entity.idcollage;
            majorpayload.major = $scope.entity.idmajor;
            promises.push(loadClassDirective(classpayload));
            promises.push(loadMajorDirective(majorpayload));
            var paymentpayload;
            if ($scope.isEditMode) { paymentpayload = $scope.entity.PaymentMethod; }
 

            promises.push(loadPaymentDirective(paymentpayload));
            WaitMultiplePromises(promises).then(function () {


                loader = false;
            });
        }
        else {
            loadUniversityDirective().then(function () { loader = false;});
        }
    };
    function loadUniversityDirective(payload) {


        var universityLoadPromiseDefer = $q.defer();
        universityReadyPromiseDefer.promise.then(function () {
            directiveapi.load(payload).then(function () {
                universityLoadPromiseDefer.resolve();
            });
        });
        return universityLoadPromiseDefer.promise;

    };
    function loadCollageDirective(payload) {
        var promises = [];
        var collageLoadPromiseDefer = $q.defer();
        if (selectedPromise != undefined)
            promises.push(selectedPromise.promise);
        promises.push(collageReadyPromiseDefer.promise);

        WaitMultiplePromises(promises).then(function () {
            cdirectiveapi.load(payload).then(function () {
                collageLoadPromiseDefer.resolve();
            });
            selectedPromise = undefined;
        });
        return collageLoadPromiseDefer.promise;
    };
    function loadClassDirective(classpayload) {
        var promises = [];
        var classLoadPromiseDefer = $q.defer();
        if (selectedallPromise != undefined)
            promises.push(selectedallPromise.promise);
        promises.push(classReadyPromiseDefer.promise);

        WaitMultiplePromises(promises).then(function () {
            classdirectiveapi.load(classpayload).then(function () {

                classLoadPromiseDefer.resolve();
            });
            selectedallPromise = undefined;
        });
        return classLoadPromiseDefer.promise;
    };
    function loadMajorDirective(majorpayload) {
        var promises = [];
        var majorLoadPromiseDefer = $q.defer();
        if (selectedallPromise != undefined)
            promises.push(selectedallPromise.promise);
        promises.push(majorReadyPromiseDefer.promise);

        WaitMultiplePromises(promises).then(function () {
            majordirectiveapi.load(majorpayload).then(function () {

                majorLoadPromiseDefer.resolve();
            });
            selectedallPromise = undefined;
        });
        return majorLoadPromiseDefer.promise;
    };
    function loadPaymentDirective(paymentpayload) {

        var PaymentLoadPromiseDefer = $q.defer();
        PaymentReadyPromiseDefer.promise.then(function () {
            paymentdirectiveapi.load(paymentpayload).then(function () {
                PaymentLoadPromiseDefer.resolve();
            });
        });
        return PaymentLoadPromiseDefer.promise;

    };



    defineScope();
    load();



}]);

app.controller("UniversityController", ["$scope", "$http", "$q", function ($scope, $http, $q) {
    WaitMultiplePromises = function (deferarray1) {
        var tester = 0;
        var waitdefer = $q.defer();
        var deferlength = deferarray1.length;
        for (var i = 0; i < deferlength; i++) {
            var d = deferarray1[i];
            d.then(function (response) {
                tester++;
                if (tester === deferlength) waitdefer.resolve();
            }); d.catch(function (error) {
                waitdefer.reject();
            });

        } return waitdefer.promise;
    };

    var countryDirectiveApi;

    var uniCountryDirectiveApi;

    function defineScope() {

        countryReadyPromiseDefer = $q.defer();

        $scope.countryOnDirectiveReady = function (api) {

            countryDirectiveApi = api;
            countryReadyPromiseDefer.resolve();
        };

        universityFilteredAccordingToCountryReadyPromiseDefer = $q.defer();

        $scope.OnUniversityFilteredAccordingToCountryCountryDirectiveReady = function (api) {


            universityFilteredAccordingToCountryDirectiveApi = api;
            universityFilteredAccordingToCountryReadyPromiseDefer.resolve();

        }
        $scope.onCountryChange = function () {
            var data = countryDirectiveApi;
            if (data.getdata() != undefined) {
                console.log(data);

                var Filter = {
                    Filters: [{
                        "$type": "MohammadProject.Controllers.CountryUniversityFilter,MohammadProject",
                        CountryId: countryDirectiveApi.getdata()
                    }]
                };
   
                universityFilteredAccordingToCountryReadyPromiseDefer.promise.then(function (response) {
                    console.log(Filter);
                    loadUniversityFilteredAccordingToCountryCountryDirective(Filter);
                });
            }
        };

        $scope.Close = function () {
            $scope.$parent.$parent.showUniversityEditor = false;
        };

        $scope.save = function () {
            var universityInfoLength = $scope.universityInfo.length;
            var dataElement;
            var dataElementsArray = [];

            for (s = 0; s < universityInfoLength; s++) {
                dataElement = $scope.universityInfo[s];
                var getUniversityID = dataElement.universityDirectiveApi.getdata();
                var getCollageID = dataElement.collageDirectiveApi.getdata();
                var getPaymentMethod = dataElement.paymentDirectiveApi.getdata();
                dataElement = { ID: s + 1, UniversityID: getUniversityID, CollageID: getCollageID, PaymentMethod: getPaymentMethod };

                dataElementsArray.push(dataElement);
            }
            var dataElements = {
                addedObject: dataElementsArray
            };
            $http({
                method: "POST",
                url: 'api/ManageUniversities/ManageUniversities',
                data: dataElements.addedObject

            }).then(function mySuccess(response) {
                console.log("alldataadded");
            });

            $scope.$parent.$parent.showUniversityEditor = false;

        };

        function prepareDataItem() {
            var element = {};
            element.paymentMethods = [];
            element.universityReadyPromiseDefer = $q.defer();
            element.collageReadyPromiseDefer = $q.defer();
            element.selectPaymentReadyPromiseDefer = $q.defer();
            element.currencyReadyPromiseDefer = $q.defer();
            element.paymentReadyPromiseDefer = $q.defer();
            element.universityOnDirectiveReady = function (api) {
                element.universityDirectiveApi = api;
                element.universityReadyPromiseDefer.resolve();

            };
            element.collageOnDirectiveReady = function (api) {
                element.collageDirectiveApi = api;
                element.collageReadyPromiseDefer.resolve();

            };
            element.paymentOnDirectiveReady = function (api) {
                element.paymentDirectiveApi = api;
                if (element.paymentReadyPromiseDefer != undefined)
                    element.paymentReadyPromiseDefer.resolve();
                else
                    element.paymentDirectiveApi.load();

            };
            element.selectPaymentOnDirectiveReady = function (selectPaymentapi) {
                element.selectPaymentDirectiveApi = selectPaymentapi;

                element.selectPaymentReadyPromiseDefer.resolve();
            };

            element.onUniversityChange = function () {

                var data = element.universityDirectiveApi;
                if (data.getdata() != undefined) {

                    if (element.selectPaymentDirectiveApi !== undefined)
                        element.selectPaymentDirectiveApi.clear();
                    element.directive = undefined;
                    var collagePayload = {};
                    collagePayload.search = element.universityDirectiveApi.getdata();
                    element.collageReadyPromiseDefer.promise.then(function (response) {
                        loadCollageDirective(collagePayload, element);
                    });
                }
            };
            element.onCollageChange = function () {
                var data = element.collageDirectiveApi;
                if (data.getdata() != undefined) {

                    element.selectPaymentReadyPromiseDefer.promise.then(function (response) {
                        element.selectPaymentDirectiveApi.clear();
                        element.directive = undefined;
                        element.selectPaymentDirectiveApi.load();

                    });
                };
            };
            element.onSelectorChange = function () {
                var data = element.selectPaymentDirectiveApi;
                element.paymentReadyPromiseDefer = $q.defer();
                if (data.getdata() != undefined) {
                    element.directive = element.selectPaymentDirectiveApi.getdata();

                    element.paymentReadyPromiseDefer.promise.then(function (response) {
                        element.paymentDirectiveApi.load();
                        element.paymentReadyPromiseDefer = undefined;
                    });
                };
            };

            return element;
        }

        $scope.addNewItem = function () {

            $scope.isUniversityEditMode = false;

            var dataItem = prepareDataItem();

            $scope.universityInfo.push(dataItem);

            var universityPayload = {};

            loadUniversityDirective(universityPayload, dataItem);

        };

    }

    var itemsPreparedPromises = [];
    $scope.universityInfo = [];

    function loadDataItem(element) {
        element.paymentMethods = [];
        element.universityReadyPromiseDefer = $q.defer();
        element.collageReadyPromiseDefer = $q.defer();
        element.selectPaymentReadyPromiseDefer = $q.defer();
        element.currencyReadyPromiseDefer = $q.defer();

        element.universityOnDirectiveReady = function (api) {
            element.universityDirectiveApi = api;
            element.universityReadyPromiseDefer.resolve();
        };

        element.collageOnDirectiveReady = function (api) {
            element.collageDirectiveApi = api;
            element.collageReadyPromiseDefer.resolve();

        };

        element.paymentOnDirectiveReady = function (api) {
            element.paymentDirectiveApi = api;
            if (element.paymentReadyPromiseDefer != undefined)
            {
                element.paymentReadyPromiseDefer.resolve();
            }
            else
                element.paymentDirectiveApi.load();
        };

        element.selectPaymentOnDirectiveReady = function (selectPaymentapi) {
            element.selectPaymentDirectiveApi = selectPaymentapi;
            element.selectPaymentReadyPromiseDefer.resolve();
        };

        element.onUniversityChange = function () {

            var data = element.universityDirectiveApi;
            if (data.getdata() != undefined) {
                if (element.universitySelectedPromise != undefined) {
                    element.universitySelectedPromise.resolve();
                } else {
                    if (element.selectPaymentDirectiveApi !== undefined)
                        element.selectPaymentDirectiveApi.clear();
                    element.directive = undefined;

                    var collagePayload = {};
                    collagePayload.search = element.universityDirectiveApi.getdata();
                    loadCollageDirective(collagePayload, element);
                }
            }
        };

        element.onCollageChange = function () {
            var data = element.collageDirectiveApi;
            if (data.getdata() != undefined) {
                if (element.collageSelectedPromise != undefined) {
                    element.collageSelectedPromise.resolve();
                } else {
                    element.selectPaymentDirectiveApi.clear();
                    element.directive = undefined;
                    element.selectPaymentDirectiveApi.load();

                }
            };
        };

        element.onSelectorChange = function () {
            var data = element.selectPaymentDirectiveApi;
            if (data.getdata() != undefined) {
                element.directive = element.selectPaymentDirectiveApi.getdata();

                element.paymentReadyPromiseDefer = $q.defer();
                element.paymentReadyPromiseDefer.promise.then(function () {
                    paymentPayload = element.PaymentMethod;
                    element.paymentDirectiveApi.load(paymentPayload);
    
                    element.paymentReadyPromiseDefer = undefined;
                });
            };
        };

        itemsPreparedPromises.push(load(element));
    }

    function load(element) {
        var promisesArray = [];
        var lloader = true;
        var dataItemPromise = $q.defer();
        universityPayload = {
            onScope:element.UniversityID,
        };
        element.universitySelectedPromise = $q.defer();

        promisesArray.push(loadUniversityDirective(universityPayload, element));

        var collagePayload = {
            update:element.CollageID,
            search:universityPayload.onScope
        };
        element.collageSelectedPromise = $q.defer();

        promisesArray.push(loadCollageDirective(collagePayload, element));
 
        var selectPaymentPayload = element.PaymentMethod;

        promisesArray.push(loadSelectPaymentDirective(selectPaymentPayload, element));

        /*element.selectorSelectedPromise = $q.defer();
        var paymentPayload = element.PaymentMethod;
       
        promisesArray.push(loadPaymentDirective(paymentPayload, element));*/

        WaitMultiplePromises(promisesArray).then(function () {
            lloader = false;  console.log("row finished");
            dataItemPromise.resolve();
        });
        return dataItemPromise.promise;

    };

    function loadUniversityDirective(payload, element) {

        element.universityLoadPromiseDefer = $q.defer();
        element.universityReadyPromiseDefer.promise.then(function () {
            element.universityDirectiveApi.load(payload).then(function () {
                element.universityLoadPromiseDefer.resolve();
            });
        });
        return element.universityLoadPromiseDefer.promise;

    };

    function loadCollageDirective(payload, element) {
        var promises = [];
        element.collageLoadPromiseDefer = $q.defer();
        if (element.universitySelectedPromise != undefined)

            promises.push(element.universitySelectedPromise.promise);
        promises.push(element.collageReadyPromiseDefer.promise);

        WaitMultiplePromises(promises).then(function () {
            element.collageDirectiveApi.load(payload).then(function () {
                element.collageLoadPromiseDefer.resolve();
            });
            element.universitySelectedPromise = undefined;
        });
        return element.collageLoadPromiseDefer.promise;

    };

    function loadSelectPaymentDirective(payload, element) {
        var promises = [];

        element.selectPaymentLoadPromiseDefer = $q.defer();
        if (element.collageSelectedPromise != undefined)

            promises.push(element.collageSelectedPromise.promise);
        promises.push(element.selectPaymentReadyPromiseDefer.promise);
        WaitMultiplePromises(promises).then(function () {
            element.selectPaymentDirectiveApi.load(payload).then(function () {

                element.selectPaymentLoadPromiseDefer.resolve();
            });
            element.collageSelectedPromise = undefined;

        });
        return element.selectPaymentLoadPromiseDefer.promise;

    };
    /*
    function loadPaymentDirective(payload, element) {
     var promises = [];
     element.paymentLoadPromiseDefer = $q.defer();
    
     promises.push(element.paymentReadyPromiseDefer.promise);
     WaitMultiplePromises(promises).then(function () {
      console.log("payment");
    
      element.paymentDirectiveApi.load(payload).then(function () {
      
     element.paymentLoadPromiseDefer.resolve();
      });
     element.selectorSelectedPromise = undefined;
    
     });
     return element.paymentLoadPromiseDefer.promise;
    
    };
    */
    function loadCountryDirective(payload) {

        countryLoadPromiseDefer = $q.defer();
        countryReadyPromiseDefer.promise.then(function () {
  
            countryDirectiveApi.load(payload).then(function () {
                countryLoadPromiseDefer.resolve();
            });
        });
        return countryLoadPromiseDefer.promise;

    };

    function loadUniversityFilteredAccordingToCountryCountryDirective(payload) {
        var promises = [];
        universityFilteredAccordingToCountryLoadPromiseDefer = $q.defer();
 
        promises.push(universityFilteredAccordingToCountryReadyPromiseDefer.promise);

        WaitMultiplePromises(promises).then(function () {
            universityFilteredAccordingToCountryDirectiveApi.load(payload).then(function () {
                console.log("unico");
                universityFilteredAccordingToCountryLoadPromiseDefer.resolve();
            });

        });
        return universityFilteredAccordingToCountryLoadPromiseDefer.promise;

    };

    function getAllDaTa() {
        var dataItems = [];
        $http({
            method: "GET",
            url: 'api/ManageUniversities/GetAllUniversities'
        }).then(function mySuccess(response) {

            dataItems = response.data; 
            var dataItemsLength = dataItems.length;
            var dataElement;

            for (t = 0; t < dataItemsLength; t++) {
                var dataElement = dataItems[t]; 
                loadDataItem(dataElement);
                $scope.universityInfo.push(dataElement);
            }
            WaitMultiplePromises(itemsPreparedPromises).finally(function (response) {
                loader = false;
                console.log("All Finished");
            });
        });
    };

    defineScope();

    loadCountryDirective();

    getAllDaTa();

}])

app.controller("Searchctr", ["$scope", "$http", "$q", function ($scope, $http, $q) {
    $scope.clickOn = "Page1";

    $scope.showCon = function (con) {
        $scope.clickOn = con;

    };

    var addunipromise;
    var directiveapi;
    var adirectiveapi;
    var cdirectiveapi;
    var acdirectiveapi;
    var selectedunitocol;



    $scope.aconDirectiveReady = function (api) {
        acdirectiveapi = api;


    };

    /*$scope.conDirectiveReady = function (api) {
     cdirectiveapi = api;
     api.load();
    
    };*/


    /*$scope.onDirectiveReady(api);*/







    /*le = data.length;*/

    var AElength;
    /* $scope.pageinfo = [];*/

    var z;









    var abc = "";
    var activemember =
     {
         indx: 1,
         from: 0,
         to: 4,
         isactive: true,
         isfirst: true,
         islast: false
     };

    var activemember2 =
     {
         indx: 1,
         from: 0,
         to: 4,
         isactive: true,
         isfirst: true,
         islast: false
     };

    /*  $scope.view = function (g) {
       if (actuallength == 0) {$scope.pageinfo[0].isdisabled = $scope.pageinfo[1].isdisabled = true; activemember.isactive = false; }
       else {
    $scope.pageinfo[1].isactive = false;
    $scope.pageinfo[activemember.indx].isactive = false;
    console.log(activemember);
        if ((g.indx == "prev") && (activemember.isfirst == false)) {
    console.log(activemember);
    
    var p = parseInt(activemember.indx);
    activemember = $scope.pageinfo[p - 1];
    }
        else if ((g.indx == "next") && (activemember.islast == false) && actuallength != 1) {
    console.log(actuallength);
    console.log($scope.pageinfo);
    console.log(activemember);
    
    
    var n = parseInt(activemember.indx);
    activemember = $scope.pageinfo[n + 1];
    }
        else if ((g.indx != "prev") && (g.indx != "next")) {
    console.log(activemember);
    
    activemember = g;
    }
    console.log(activemember);
    
    
    
    
    $scope.pageinfo[activemember.indx].isactive = true;
    if ($scope.pageinfo[activemember.indx].isfirst == true)
    $scope.pageinfo[0].isdisabled = true;
    else $scope.pageinfo[0].isdisabled = false;
    if ($scope.pageinfo[activemember.indx].islast == true)
    $scope.pageinfo[actuallength + 1].isdisabled = true;
    else $scope.pageinfo[actuallength + 1].isdisabled = false;
    }
    $scope.grid = [];
    var i = activemember.from;
    console.log(i);
       for (j = i; j < i + 5; j++) {
        var trial = arrayelements[j];
    if (trial != undefined)
    $scope.grid.push(trial);
    }
    };
    */



    var kl;
    var actuallength;

    /* function fill() {
    fillsearch();
    var element, prev, next;
    $scope.pageinfo.length = 0;
    prev =
       {
    indx: "prev",
    isactive: false,
    isdisabled: false
    }
    $scope.pageinfo.push(prev);
    var roundedlength = Math.round(AElength / 5);
    actuallength = (roundedlength >= AElength / 5) ? roundedlength : roundedlength + 1;
      for (i = 0; i < actuallength; i++) {
       var k = i * 5;
       element = {
    indx: i + 1,
    from: k,
    to: k + 4,
    isactive: ((i == 0) ? true : false),
    isdisabled: false,
    isfirst: (i == 0) ? true : false,
    islast: ((i + 1 == actuallength) ? true : false)
    };
    $scope.pageinfo.push(element);
    }
    next =
       {
    indx: "next",
    isactive: false,
    isdisabled: false
    }
    $scope.pageinfo.push(next);
    };*/




    /*
    fill();
    abc = $scope.search;
    $scope.sbutton = abc;
    arrayelements.length = 0;
    arrayelements = fillsearch();
    
     if (actuallength != 0) {
    activemember = activemember2;
    console.log(activemember);
    console.log($scope.pageinfo);
    $scope.pageinfo[1].isdisabled = false;
    if ($scope.pageinfo[activemember.indx].isfirst == true)
    $scope.pageinfo[0].isdisabled = true;
    else $scope.pageinfo[0].isdisabled = false;
    
    if ($scope.pageinfo[activemember.indx].islast == true)
    $scope.pageinfo[actuallength + 1].isdisabled = true;
    else $scope.pageinfo[actuallength + 1].isdisabled = false;
    }
     else {$scope.pageinfo[0].isdisabled = $scope.pageinfo[1].isdisabled = true; }*/
    /*
    
    $scope.aonDirectiveReady = function (api) {
     $scope.Searchf();
     adirectiveapi = api;
     console.log(unielements);
    
     addunipromise = api.load();
     select = "AUB";
     var unilen = unielements.length;
     console.log(addunipromise);
     if (select != "" && select != null) addunipromise.then(function (response) {
    
      /*for (w = 0; w < unilen; w++) {
       if (unielements[w].name === select) onchange = unielements[w].id;
      } console.log(onchange);
      var onchange = 4;
      var apayload = {
       b: "",
       c: onchange
      };
      acdirectiveapi.load(apayload);
    
    
     });
    
    };
    
    */




    /*var tester = 0;
    
    var deferarray = [];
    var defer1 = $q.defer();
    setTimeout(function () { defer1.resolve(1);  }, 20000);
    deferarray.push(defer1.promise);
    
    
    var defer2 = $q.defer(); 
    setTimeout(function () {defer2.resolve(2); }, 5000);
    deferarray.push(defer2.promise);
    
    var defer3 = $q.defer();
    setTimeout(function () {defer3.resolve(3);  }, 10000);
    deferarray.push(defer3.promise);
    
    
    
    
     WaitMultiplePromises(deferarray).then(function (response) { console.log("all promises succeeded"); })
      .catch(function (error) {
      console.log("Some promises failed");
     }); */
}]);

app.directive("myDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            onvaluechange:'='
        },
        controller: function ($scope, $http, $q, $rootScope) {
            var ctrl = this;

            $scope.$watch("selectedvalue", function () {
                if (ctrl.onvaluechange != undefined)
                    ctrl.onvaluechange();
            });
            var api = {};
            api.load = function (payload) {
                var unidirectivepromise = $q.defer();

                var plupdate = payload.onScope;
                var Filters;
                if (payload.onScope == undefined||payload.onScope=="") Filters = payload;
                var onviewuniversity;
  
                var defer = $q.defer();
                var uelements = [];
                $http({
                    method: "POST",
                    url: 'api/University/GetUniversityInfo',
                    data: Filters

                }).then(function mySuccess(response) {
                    console.log(Filters);
    
                    unidirectivepromise.resolve();
                    var trial;
                    $scope.universityinfo = [];
                    $scope.universityinfo.length = 0;
                    $scope.universityinfo = response.data;
                    unilength = $scope.universityinfo.length;
                    var universityFiltered = [];
    

                    for (i = 0; i < unilength; i++) {
                        trial = $scope.universityinfo[i];
                        if (trial.id == plupdate) { onviewuniversity = trial; }
                    }


                    $scope.selectedvalue = onviewuniversity;
                }); 
                return unidirectivepromise.promise;
            }; 
            api.getdata = function () {
                if ($scope.selectedvalue!=undefined)
                    return $scope.selectedvalue.id;
            };
            if (ctrl.onReady != undefined)
                ctrl.onReady(api);
  
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<select ng-model="selectedvalue" ng-options="x as x.name for x in universityinfo track by x.id" ></select > '
    };
});

app.directive("collageDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            onvaluechange: '='
        },
        controller: function ($scope, $http, $q, $rootScope) {



            var ctrl = this;
            $scope.$watch("collageselectedvalue", function () {
                if (ctrl.onvaluechange != undefined)
                    ctrl.onvaluechange();
            });

            var api = {};
            api.load = function (payload) {
                var collagedirectivepromise = $q.defer();
                var plupdate = payload.update;
                var plsearch = payload.search;
                var onviewcollage;
                var cvalue = "-1";
                var celements = [];
                var collagetrial;
                /*var pc = payload.c;
                var pb = payload.b;*/
                $http({
                    method: "GET",
                    url: 'api/Collage/CollageGetInfo?uniID=' + plsearch
                }).then(function mySuccess(response) {
                    collagedirectivepromise.resolve(); 
                    $scope.collageinfo = [];
                    $scope.collageinfo.length = 0;
                    $scope.collageinfo = response.data;
                    clength = $scope.collageinfo.length;

                    for (n = 0; n < clength; n++) {
                        collagetrial = $scope.collageinfo[n];
    
                        if (collagetrial.id == plupdate) { onviewcollage = collagetrial; }
                    }

                    $scope.collageselectedvalue = onviewcollage; 
                }); return collagedirectivepromise.promise;

            };
            api.getdata = function () {
                if ($scope.collageselectedvalue != undefined)
                    return $scope.collageselectedvalue.id;
            };
            if (ctrl.onReady != undefined)
                ctrl.onReady(api);

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<select ng-model="collageselectedvalue" ng-options="x as x.faculty for x in collageinfo track by x.id"></select > '
    };
});

app.directive("classDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "="  },
        controller: function ($scope, $http, $q,$rootScope) {
            var ctrl = this;
            var classapi = {};
            classapi.load = function (payload) {
                var classdirectivepromise = $q.defer();
                var editedclass = payload.class;
                var collageid = payload.collageid;
                var classinfolength;
                $http({
                    method: "GET",
                    url: 'api/Student/ClassGetInfo?collageid=' + collageid
                }).then(function mySuccess(response) {

                    classdirectivepromise.resolve();
                    $scope.classinfo = [];
                    $scope.classinfo = response.data;
                    $scope.selectedvalue = editedclass;
                    classinfolength = $scope.classinfo.length;
                    for (m = 0; m < classinfolength; m++) {

                        if ($scope.classinfo[m].ID === editedclass) {
                            $scope.selectedvalue = $scope.classinfo[m]
                            break;
                        }
                    }
                }); 
   
                return classdirectivepromise.promise;
            };
            classapi.getdata = function () {
                if ($scope.selectedvalue != undefined)
                    return $scope.selectedvalue.ID;
            };
            classapi.clear = function () {if($scope.classinfo!=undefined)
                $scope.classinfo.length = 0;
            }
            if (ctrl.onReady != undefined)
                ctrl.onReady(classapi);

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<select ng-model="selectedvalue" ng-options="x as x.Name for x in classinfo track by x.ID" ></select > '
    };
});

app.directive("majorDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "="},
        controller: function ($scope, $http, $q,$rootScope) {
            var ctrl = this;

            var majorapi = {};
            majorapi.load = function (payload) {
                var majordirectivepromise = $q.defer();
                var editedmajor = payload.major;
                var collageid = payload.collageid;
                var onscope;
                var majorinfolength;
                $http({
                    method: "GET",
                    url: 'api/Student/MajorGetInfo?collageid=' + collageid
                }).then(function mySuccess(response) {

                    majordirectivepromise.resolve();
                    $scope.majorinfo = [];
                    $scope.majorinfo = response.data;
                    majorinfolength = $scope.majorinfo.length;
                    ;
                    for (m = 0; m < majorinfolength; m++) {

                        if ($scope.majorinfo[m].ID === editedmajor) {
                            $scope.selectedvalue = $scope.majorinfo[m]
                            break;
                        }
                    }
                });
                return majordirectivepromise.promise;
            };
            majorapi.getdata = function () {
                if ($scope.selectedvalue != undefined)
                    return $scope.selectedvalue.ID;
            };
            majorapi.clear = function () {
                if ($scope.majorinfo != undefined)
                    $scope.majorinfo.length = 0;
            }
            if (ctrl.onReady != undefined)
                ctrl.onReady(majorapi);

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<select ng-model="selectedvalue" ng-options="x as x.Name for x in majorinfo track by x.ID" ></select > '
    };
});

app.directive("collageDrillDownOnUniversity", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            onvaluechange: '='
        },
        controller: function ($scope, $http) {
            var ctrl = this;
            $scope.collageinfo = [];
            var drilldownapi = {};
            drilldownapi.load = function (payload) {

                var svalue = "";
                var collageelements = [];
   
                $http({
                    method: "GET",
                    url: 'api/Student/ColGetFiltered?cname=' + svalue
                }).then(function mySuccess(response) {
                    $scope.collageinfo.length = 0;

                    collageelements.length = 0;
                    collageelements = response.data.collage
                    collageelementslength = collageelements.length;

                    for (n = 0; n < collageelementslength; n++) {
                        var collagetrial = collageelements[n]; 
                        if (collagetrial.uniID == payload) { $scope.collageinfo.push(collagetrial);}
                    } 
                });
            };
 
            if (ctrl.onReady != undefined)
                ctrl.onReady(drilldownapi);

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<table><tr><td>ID</td><td>&nbsp;&nbsp;&nbsp;&nbsp;</td><td> Collage</td></tr><tr ng-repeat="x in collageinfo track by $index"><td>{{ x.id }}</td><td>&nbsp;&nbsp;&nbsp;&nbsp;<td>{{ x.faculty }}</td></tr></table>'
    };
});
app.directive("currencyDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $http, $q, $rootScope) {
            var ctrl = this;

            var currencyapi = {};
            var currencyDirectivePromise = $q.defer();
            currencyapi.load = function (payload) {
                ctrl.currencyInfo = [];
                var currencyInfoLength;
                $http({
                    method: "GET",
                    url: 'api/Student/CurrencyGetInfo?'
                }).then(function mySuccess(response) {
                    setTimeout(function () {
                        currencyDirectivePromise.resolve();
                    },5000);
                    ctrl.currencyInfo = response.data;

                    currencyInfoLength = ctrl.currencyInfo.length;
                    for (b = 0; b < ctrl.currencyInfo.length; b++) {

                        if (payload != undefined) {
                            if (ctrl.currencyInfo[b].currency === payload.currency) {
                                $scope.currencySelection = ctrl.currencyInfo[b];
                            }
                        }
                    }
                });

                return currencyDirectivePromise.promise;

            }

            currencyapi.getdata = function () {
                if (ctrl.currencyInfo != undefined)

                    return $scope.currencySelection.currency;
            }


            if (ctrl.onReady != undefined)
                ctrl.onReady(currencyapi);

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<select ng-model="currencySelection" ng-options="y as y.currency for y in ctrl.currencyInfo track by y.currency"></select>'
    };
});


app.directive("cashPaymentMethodDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },


        controller: function ($scope, $http, $q, $rootScope) {
            WaitMultiplePromises = function (deferarray1) {
                var tester = 0;
                var waitdefer = $q.defer();
                var deferlength = deferarray1.length;
                for (var i = 0; i < deferlength; i++) {
                    var d = deferarray1[i];
                    d.then(function (response) {
                        tester++;
                        if (tester === deferlength) waitdefer.resolve();
                    }); d.catch(function (error) {
                        waitdefer.reject();
                    });

                } return waitdefer.promise;
            };


            var currencyDirectiveapi;
            var currencyReadyDirectivePromise = $q.defer();

            var ctrl = this;
            var cashapi = {};

            cashapi.load = function (payload) {
                var promises = [];
                promises.push(loadCurrencyDirective());

                function loadCurrencyDirective() {
                    if (payload != undefined) {
                        var currencyPayload = {
                            currency: payload.Currency
                        };
                    }
                    return currencyDirectiveapi.load(currencyPayload);
                }
                if (payload !== undefined) {
                    $scope.Amount = payload.Amount;
                }

                return WaitMultiplePromises(promises);
            };
            cashapi.getdata = function () {
                if ($scope.Amount != undefined)
                    var paymentobject = {
                        "$type": "MohammadProject.Controllers.CashPaymentMethod,MohammadProject",
                        Amount: $scope.Amount,
                        Currency: currencyDirectiveapi.getdata()
                    };

                return paymentobject;
            };

            $scope.currencyOnDirectiveReady = function (api) {
                currencyDirectiveapi = api;
                currencyReadyDirectivePromise.resolve();
            };
            currencyReadyDirectivePromise.promise.then(function (response) {
                if (ctrl.onReady != undefined)
                    ctrl.onReady(cashapi);
            });
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<p>Amount:</p><input type="text" ng-model="Amount"><currency-directive on-ready="currencyOnDirectiveReady"></currency-directive>'
    };
});

app.directive("checkPaymentMethodDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $http, $q, $rootScope) {
            var ctrl = this;
            var checkapi = {};
 
            checkapi.load = function (payload) {
                var checkdirectivepromise = $q.defer();
                if (payload !== undefined) {
                    $scope.Amount = payload.Amount;
                    $scope.CheckNumber = payload.CheckNumber;
                }
                checkdirectivepromise.resolve();
                return checkdirectivepromise.promise;
            };
            checkapi.getdata = function () {
                if ($scope.Amount != undefined)
                    var paymentobject = {
                        "$type": "MohammadProject.Controllers.CheckPaymentMethod,MohammadProject",
                        Amount: $scope.Amount,
                        CheckNumber: $scope.CheckNumber,
                    }
                return paymentobject;
            };
            if (ctrl.onReady != undefined)
                ctrl.onReady(checkapi);
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<p>Amount:</p><input type="text" ng-model="Amount">&nbsp;&nbsp;&nbsp;&nbsp;<p>Check Number:</p><input type="text" ng-model="CheckNumber">'
    };
});

app.directive("selectPaymentMethodDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            onvaluechange: '='

        },
        controller: function ($scope, $http, $q, $rootScope) {
            var ctrl = this;

            $scope.$watch("paymentMethodSelect", function () {
                if (ctrl.onvaluechange != undefined)
                    ctrl.onvaluechange();
            });

            var selectapi = {};
  
            $scope.paymentMethods = [];
            $scope.paymentMethods.length = 0;

            selectapi.load = function (payload) {
                var selectorDirectivePromise = $q.defer();
                selectorDirectivePromise.resolve();

                var cashpaymentmethod = { configid: 1, method: "cash", directive: "cash-payment-method-directive" }
                var checkpaymentmethod = { configid: 2, method: "check", directive: "check-payment-method-directive" }
                $scope.paymentMethods.push(cashpaymentmethod);
                $scope.paymentMethods.push(checkpaymentmethod);

                if (payload != undefined) {
                    var configid = payload.ConfigurationID; 

                    for (w = 0; w < $scope.paymentMethods.length; w++) {
     
                        if ($scope.paymentMethods[w].configid === configid) {

                            $scope.paymentMethodSelect = $scope.paymentMethods[w]; break;
                        }
                    }
                } return selectorDirectivePromise.promise;
            };

            selectapi.getdata = function () {
                if ($scope.paymentMethodSelect != undefined)
                    return $scope.paymentMethodSelect.directive; 
            };

            selectapi.clear = function () {
                if ($scope.paymentMethods != undefined)
                    $scope.paymentMethods.length = 0;
                $scope.paymentMethodSelect = {};
            };
  
            if (ctrl.onReady != undefined)
                ctrl.onReady(selectapi);
  
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<select ng-model="paymentMethodSelect" ng-options="y as y.method for y in paymentMethods track by y.method"></select>'
    };
});

app.directive("countryDirective", function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            onvaluechange: '='
        },
        controller: function ($scope, $http, $q, $rootScope) {
            var ctrl = this;

            $scope.$watch("selectedvalue", function () {
                if (ctrl.onvaluechange != undefined)
                    ctrl.onvaluechange();
            });
            var api = {};
            api.load = function (payload) {

                var countryDirectivePromise = $q.defer();


                var editedCountry = payload;
                var onScopeCountry;

                var defer = $q.defer();
                var countryElements = [];
                $http({
                    method: "GET",
                    url: 'api/Countries/CountriesGetInfo'
                }).then(function mySuccess(response) {

                    countryDirectivePromise.resolve();
                    var trial;
                    $scope.countriesInfo = [];
                    $scope.countriesInfo.length = 0;
                    $scope.countriesInfo = response.data;
                    countriesInfoLength = $scope.countriesInfo.length;

                    for (i = 0; i < countriesInfoLength; i++) {
                        trial = $scope.countriesInfo[i];
                        if (trial.ID == editedCountry) { onScopeCountry = trial; }
                    }

                    $scope.selectedvalue = onScopeCountry;
                });
                return countryDirectivePromise.promise;
            };
            api.getdata = function () {
                if ($scope.selectedvalue != undefined)
                    return $scope.selectedvalue.ID;
            };
            if (ctrl.onReady != undefined)
                ctrl.onReady(api);

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: '<select ng-model="selectedvalue" ng-options="x as x.country for x in countriesInfo track by x.ID" ></select > '
    };
});

app.directive('directivewrapper', ['$compile', '$injector', function ($compile, $injector) {

    var directiveDefinitionObject = {

        restrict: 'E',

        scope: false,

        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

        },

        controllerAs: 'ctrl',

        bindToController: true,

        compile: function (element, attrs) {

            return {

                pre: function ($scope, iElem, iAttrs, ctrl) {

                    var cloneScope = null;

                    $scope.$on("$destroy", function () {

                        iElem.off();

                        directiveWatch();

                    });

                    var directiveWatch = $scope.$watch(iAttrs.directive, function () {

                        var directive = $scope.$eval(iAttrs.directive);

                        if (directive != undefined) {

                            if (!$injector.has(attrs.$normalize(directive) + 'Directive'))

                                console.log('directive: ' + directive + ' not exists');

                        }

                        var newElement = "";

                        if (directive != undefined && directive != "") {

                            newElement = '<' + directive;

                            for (var prop in iAttrs.$attr) {

                                if (iAttrs.$attr[prop] != "directive")

                                    newElement += ' ' + iAttrs.$attr[prop] + '="' + iAttrs[prop] + '"';

                            }

                            newElement += ' ></' + directive + '>';

                        }

                        if (cloneScope) {

                            // ***************************************************

                            // NOTE: We are removing the element BEFORE we are

                            // destroying the scope associated with the element.

                            // ***************************************************

                            cloneScope.$destroy();

                            cloneScope = null;

                        }

                        setTimeout(function () {

                            cloneScope = $scope.$new();

                            iElem.html(newElement);

                            $compile(iElem.contents())(cloneScope);

                        });

                    });

                }

            };

        }

    };

    return directiveDefinitionObject;

}]);