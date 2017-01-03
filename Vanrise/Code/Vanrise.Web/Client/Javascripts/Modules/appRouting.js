'use strict';
var appRouting = angular.module('appRouting', ['ngRoute']);
appRouting.config(['$routeProvider',
  function ($routeProvider) {
      var folderPart = '';
      for (var i = 1; i <= 10; i++) {          
          $routeProvider.
                when('/view/:module' + folderPart + '/:pagename', {
                    templateUrl: function (params) {
                        return getTemplateURLFromParams(params);
                    }
                }).
                when('/viewwithparams/:module' + folderPart + '/:pagename/:params', {
                    templateUrl: function (params) {
                        return getTemplateURLFromParams(params);
                    }
                });
          folderPart += '/:folder' + i;
     }

     function getTemplateURLFromParams(params) {
          var templateURL = '/Client/Modules/' + params.module;
          var folder = '';
          var current = 1;
          while (folder != undefined) {
              folder = params['folder' + current];
              if (folder != undefined)
                  templateURL += '/' + folder;
              current++;
          }
          templateURL += '/' + params.pagename + '.html';
          return templateURL;
      }
      $routeProvider.
          when('/Error/:params', {
              templateUrl: '/Client/Modules/Common/Views/Error.html'
          }).
        when('/default', {
            templateUrl: '/Client/Views/Default.html',
            controller: 'DefaultController'
        }).
         when('/Documents', {
             templateUrl: '/Client/Views/Documents.html'
         }).otherwise({
            redirectTo: '/default'
        });
 }]);