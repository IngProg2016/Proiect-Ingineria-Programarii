(function () {
    'use strict';

    angular.module('OutOfRangeApp', ['ngRoute', 'ngComponentRouter', 'ngResource', 'ngStorage', 'ngSanitize', 'ngMessages', 'ui.tinymce'])
    .config(['$locationProvider', function ($locationProvider) {
        $locationProvider.html5Mode(true);
    }])
    .value('$routerRootComponent', 'app')
    .filter('plainText', PlainTextFilter)
    ;


    function PlainTextFilter() {
        return function (text) {
            return angular.element('<div>' + text + '</div>').text();
        }
    };


})();