(function () {
    'use strict';

    angular.module('OutOfRangeApp', ['ngRoute', 'ngResource', 'ngStorage'])
    .run(['$rootScope', 'authService', '$location', function ($rootScope, authService, $location) {
        $rootScope.$on('$routeChangeStart', function (event, next) {
            var isAuthentificated = authService.getAuthentificationInfo().isAuth;
            
            if (next.requiresLogin && !isAuthentificated) {
                var oldUrl = $location.path()
                $location.path('/login').search('returnUrl',encodeURIComponent(oldUrl));
            }
        });
    }]);
})();