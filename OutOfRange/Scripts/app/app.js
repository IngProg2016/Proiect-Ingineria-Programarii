(function () {
    'use strict';

    angular.module('OutOfRangeApp', ['ngRoute', 'ngComponentRouter', 'ngResource', 'ngStorage'])
    .config(['$locationProvider', function ($locationProvider) {
        $locationProvider.html5Mode(true);
    }])
    .value('$routerRootComponent', 'app')
    ;
})();